using System;
using System.Collections;
//using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IHear
{
    [Header("Settings")]
    [SerializeField] LayerMask whatIsGround, whatIsPlayer, whatIsThis;
    [SerializeField] private Transform _enemyAttackPoint;
    [SerializeField] private float _sightRange, _attackRange;
    [SerializeField] private float _walkPointRange;
    [SerializeField] private float _searchRange;
    [SerializeField] private float _respawnRange;

    [Header("Camera")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private Animator _blink;
    public CinemachineCamera CameraPlayer;
    public CinemachineCamera CameraEnemy;

    #region Internal
    private Transform _player;
    private NavMeshAgent _enemy;
    private float _speed;
    private Vector3 _dirToPlayer;
    private float[] _probs = { 0.2f, 0.2f, 0.2f, 0.2f, 0.15f, 0.05f };

    // Run Away
    private Vector3 _respawnPoint;
    private bool _respawnPointSet;
    private bool _runAway;

    // Searching
    private Vector3 _soundPos;
    private Vector3 _searchPoint;
    private bool _searchPointSet;
    private bool _searching;

    // Patrolling
    private Vector3 _walkPoint;
    private bool _walkPointSet;
    

    // Attacking
    private bool _alreadyAttacked;

    // States
    private bool _playerInSightRange, _playerInAttackRange;

    // Event
    public static Action<bool> OnEnemyAttack;
    #endregion

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _enemy = GetComponent<NavMeshAgent>();
        _speed = _enemy.speed;
    }

    private void Update()
    {
        // Check for sight and attack ranges
        _dirToPlayer = _player.position - transform.position;
        Ray ray = new Ray(transform.position, _dirToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, _sightRange, whatIsThis))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _playerInSightRange = true;
            }
            else
            {
                Debug.Log("player not here");
                _playerInSightRange = false;
            }
        }
       

        _playerInAttackRange = Physics.CheckSphere(_enemyAttackPoint.position, _attackRange, whatIsPlayer);
        if (_runAway) RunAway();
        if (_searching && !_playerInSightRange && !_runAway) SearchSound();
        if (!_playerInSightRange && !_playerInAttackRange && !_searching && !_runAway) Patroling();
        if (_playerInSightRange && !_playerInAttackRange && !_runAway) ChasePlayer();
        if (_playerInAttackRange && _playerInSightRange && !_runAway) AttackPlayer();
        
    }

    private void Patroling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet)
        {
            _enemy.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 1.5f)
        {
            _walkPointSet = false;
        }
        _enemy.speed = _speed;
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = UnityEngine.Random.Range(-_walkPointRange, _walkPointRange);
        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
        {
            _walkPointSet = true;
        }
        
    }

    private void ChasePlayer()
    {
        _enemy.SetDestination(_player.position);
        _enemy.speed = _speed * 2f;
    }

    private void SearchSound()
    {
        if (!_searchPointSet) SearchSoundPoint();

        if (_searchPointSet)
        {
            _enemy.SetDestination(_searchPoint);
        }
        
        Vector3 distanceToSearchPoint = transform.position - _searchPoint;    
        if (distanceToSearchPoint.magnitude < 1.5f)
        {
            _searchPointSet = false;
            _searching = false;
        }
        _enemy.speed = _speed;
    }

    private void SearchSoundPoint()
    {
        float randomZ = UnityEngine.Random.Range(-_searchRange, _searchRange);
        float randomX = UnityEngine.Random.Range(-_searchRange, _searchRange);
        _searchPoint = new Vector3(_soundPos.x + randomX, _soundPos.y, _soundPos.z + randomZ);
        if (Physics.Raycast(_searchPoint, -transform.up, 2f, whatIsGround))
        {
            _searchPointSet = true;
        }
    }

    private void AttackPlayer()
    {
        _enemy.SetDestination(transform.position);

        transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            CameraManager.SwitchCamera(CameraEnemy);
            _impulseSource.GenerateImpulse();
            OnEnemyAttack?.Invoke(false);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.MurdererAttack);
            BodyPart bodyPart = BodyPartsProbability();
            
            _player.GetComponent<PlayerHealth>().DamagePart(bodyPart, 100);
            StartCoroutine(Stop());
        }
    }

    private IEnumerator Stop()
    {
        _alreadyAttacked = true;
        yield return new WaitForSeconds(2.5f);
        _runAway = true;    
        _blink.SetTrigger("Blink");
        yield return new WaitForSeconds(1f);
        CameraManager.SwitchCamera(CameraPlayer);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.Breath);
        yield return new WaitForSeconds(1.5f);
        OnEnemyAttack?.Invoke(true);
    }

    private void RunAway()
    {
        _searching = false;
        if (!_respawnPointSet) SearchRespawnPoint();

        transform.LookAt(_player);
        _enemy.speed = _speed * 4f;
        _enemy.SetDestination(_respawnPoint);
        Vector3 distanceToRespawnPoint = transform.position - _respawnPoint;
        if (distanceToRespawnPoint.magnitude < 1.5f)
        {
            _alreadyAttacked = false;
            _runAway = false;
            _respawnPointSet = false;
            _walkPointSet = false;
            _blink.ResetTrigger("Blink");

        }
    }

    private void SearchRespawnPoint()
    {
        
        float randomZ = UnityEngine.Random.Range(-_respawnRange, _respawnRange);
        float randomX = UnityEngine.Random.Range(-_respawnRange, _respawnRange);
        randomZ = Mathf.Abs(randomZ) < 20f ? 20f : randomZ; // Ensure minimum distance
        randomX = Mathf.Abs(randomX) < 20f ? 20f : randomX; // Ensure minimum distance
        _respawnPoint = new Vector3(_player.transform.position.x + randomX, _player.transform.position.y, _player.transform.position.z + randomZ);
        if (Physics.Raycast(_respawnPoint, -transform.up, 2f, whatIsGround))
        {
            _respawnPointSet = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, _dirToPlayer.normalized * _sightRange, Color.green);
        
        if (_enemyAttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_enemyAttackPoint.position, _attackRange);
        }
    }

    public void RespondToSound(Sound sound)
    {
        
        Debug.Log("Enemy heard sound: " + sound);

        _searchPointSet = false;
        _searching = true;
        _soundPos = sound.Pos;
    }

    private BodyPart BodyPartsProbability()
    {
        float total = 0f;

        foreach (float prob in _probs)
        {
            total += prob;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < _probs.Length; i++)
        {
            if (randomPoint < _probs[i])
            {
                return (BodyPart)i;       
            }
            else 
            { 
                randomPoint -= _probs[i];
            }
        }
        return (BodyPart)_probs.Length - 1;      
    }
}
