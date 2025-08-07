using System;
using System.Collections;
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
    [SerializeField] private float _escapeRange;
    public EnemyManager Respawn;

    [Header("Camera")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _faint;
    private Animator _blink;
    public CinemachineCamera CameraPlayer;
    public CinemachineCamera CameraEnemy;

    [Header("Lights and Effects")]
    public Light RedLight;
    public Light WhiteLight;
    public GameObject Blood;
    public GameObject BloodOverlay;

    [Header("leg and arm models")]
    public GameObject leg;
    public GameObject arm;

    #region Internal
    private Animator _animator;
    private Transform _player;
    private NavMeshAgent _enemy;
    private float _speed;
    private Vector3 _dirToPlayer;
    private float[] _probs = { 0.2f, 0.2f, 0.2f, 0.2f, 0.15f, 0.05f };
    private int _bodyPartLeft = 4;

    // Camera
    private CinemachineInputAxisController _playerCameraController;
    private CinemachinePanTilt _cameraPanTilt;

    // Run Away
    private Vector3 _runAwayPoint;
    private bool _runAwayPointSet;
    private bool _runAway;
    public bool respawn;

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
        _animator = GetComponent<Animator>();
        _playerCameraController = CameraPlayer.GetComponent<CinemachineInputAxisController>();
        _cameraPanTilt = CameraPlayer.GetComponent<CinemachinePanTilt>();
        _blink = _faint.transform.GetChild(0).GetComponent<Animator>();
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
        if (!_alreadyAttacked && respawn) Respawn.RespawnDelay();
        if (_runAway) RunAway();
        if (_searching && !_playerInSightRange && !_runAway) SearchSound();
        if (!_playerInSightRange && !_playerInAttackRange && !_searching && !_runAway) Patroling();
        if (_playerInSightRange && !_playerInAttackRange && !_runAway) ChasePlayer();
        if (_playerInAttackRange && _playerInSightRange && !_runAway) AttackPlayer();
        
    }

    private void Patroling()
    {
        _animator.SetBool("Run", false);
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
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = UnityEngine.Random.Range(-_walkPointRange, _walkPointRange);
        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
        {
            _animator.SetTrigger("Look");
            _walkPointSet = true;
        }
        
    }

    private void ChasePlayer()
    {
        _enemy.SetDestination(_player.position);
        _enemy.speed = _speed * 2f;
        _animator.SetBool("Run", true); 
    }

    private void SearchSound()
    {
        _animator.SetBool("Run", false);
        _animator.SetBool("Look", false);
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
            _animator.SetBool("Look", false);
            _searchPointSet = true;
        }
    }

    private void AttackPlayer()
    {
        _enemy.SetDestination(transform.position);
        _animator.SetBool("Run", false);

        transform.LookAt(_player);

        int attack = UnityEngine.Random.Range(0, 2);

        if (!_alreadyAttacked)
        {
            OnEnemyAttack?.Invoke(false);
            WhiteLight.enabled = true;
            CameraManager.SwitchCamera(CameraEnemy);
            Debug.Log("enemy camera" );
            AudioManager.Instance.PlaySFX(AudioManager.Instance.MurdererAttack);
            StartCoroutine(ScreenShake());

            switch (attack)
            {
                case 0:
                    _animator.SetTrigger("Attack1");
                    _alreadyAttacked = true;
                    break;
                case 1:
                    _animator.SetTrigger("Attack2");
                    _alreadyAttacked = true;
                    break;
            }
        }
    }

    
    private IEnumerator ScreenShake()
    {
        float time = 0;

        while (time < 4f)
        {
            float x = UnityEngine.Random.Range(-0.3f, 0.3f);
            float y = UnityEngine.Random.Range(-0.3f, 0.3f);

            Vector3 shakeVelocity = new Vector3(x, y, 0.2f);

            _impulseSource.GenerateImpulseWithVelocity(shakeVelocity);
            yield return new WaitForSeconds(0.3f);

            time += 0.3f;
        }
    }

    public void DamagePlayer()
    {
        WhiteLight.enabled = false;
        RedLight.enabled = true;

        Blood.SetActive(true);
        AudioManager.Instance.PlaySFXBlood();
        _faint.SetActive(true);

        BodyPart bodyPart = BodyPartsProbability();
        AudioManager.Instance.PlaySFXBreath();
        _player.GetComponent<PlayerHealth>().DamagePart(bodyPart, 100);

        if (bodyPart == BodyPart.LeftArm || bodyPart == BodyPart.RightArm)
        {
            Instantiate(arm, _player.position, Quaternion.identity);
        }
        else if (bodyPart == BodyPart.LeftLeg || bodyPart == BodyPart.RightLeg)
        {
            Instantiate(leg, _player.position, Quaternion.identity);
        }

        StartCoroutine(Stop());
    }

    private IEnumerator Stop()
    {
        yield return new WaitForSeconds(2.5f);
        
        AudioManager.Instance.PlaySFX(AudioManager.Instance.ScaryLaugh);

        _runAway = true;

        _playerCameraController.enabled = false;
        RedLight.enabled = false;
        yield return new WaitForSeconds(2f);
        _blink.SetTrigger("Blink");
        yield return new WaitForSeconds(1f);
        Blood.SetActive(false);

        _cameraPanTilt.TiltAxis.Value = -30f;
        CameraManager.SwitchCamera(CameraPlayer); 
        
        yield return new WaitForSeconds(3f);
        BloodOverlay.SetActive(true);
        _blink.SetTrigger("BlinkOpen");
        AudioManager.Instance.PlaySFXBreath();

        OnEnemyAttack?.Invoke(true);
        yield return new WaitForSeconds(1f);
        _playerCameraController.enabled = true;
        respawn = true;
        _blink.ResetTrigger("Blink");
        _faint.SetActive(false);
    }

    private void RunAway()
    {
        _searching = false;
        if (!_runAwayPointSet) SearchRunAwayPoint();

        transform.LookAt(_player);

        if (_alreadyAttacked == true)
        {
            _enemy.speed = _speed * 2.5f;
            _enemy.SetDestination(_runAwayPoint);
        }

        Vector3 distanceToRespawnPoint = transform.position - _runAwayPoint;
        if (distanceToRespawnPoint.magnitude < 2f)
        {
            _alreadyAttacked = false;
            //_runAway = false;
            //_runAwayPointSet = false;
            //_enemy.SetDestination(_runAwayPoint);
            _walkPointSet = false;
            _animator.SetBool("Run", false);
            Debug.Log("Enemy stay");
        }
    }

    public void ResetRunAwayStatus()
    {
        _searching = false;
        _runAway = false;
        _runAwayPointSet = false;
    }

    private void SearchRunAwayPoint()
    {
        
        float randomZ = UnityEngine.Random.Range(-_escapeRange, 0);
        float randomX = UnityEngine.Random.Range(-_escapeRange, _escapeRange);
        randomZ = Mathf.Abs(randomZ) < 5f ? 5f : randomZ; // Ensure minimum distance
        randomX = Mathf.Abs(randomX) < 5f ? 5f : randomX; // Ensure minimum distance
        _runAwayPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(_runAwayPoint, -transform.up, 2f, whatIsGround))
        {
            _runAwayPointSet = true;
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
                if ((BodyPart)i == BodyPart.LeftLeg ||
                    (BodyPart)i == BodyPart.RightLeg ||
                    (BodyPart)i == BodyPart.LeftArm ||
                    (BodyPart)i == BodyPart.RightArm)
                {
                    float probIncrement = _probs[i] / _bodyPartLeft;
                    for (int j = 0; j < 5; j++)
                    {
                        if (j != i && _probs[j] != 0)
                        {
                            _probs[j] += probIncrement; // Increase the probability of other body parts
                        }
                    }
                    _probs[i] = 0f; // Reset the probability of the selected body part
                    _bodyPartLeft--;
                }

                Debug.Log("Probs for body parts " + string.Join(", ", _probs));
                return (BodyPart)i;       
            }
            else 
            { 
                randomPoint -= _probs[i];
            }
        }
        return (BodyPart)_probs.Length - 1;      
    }

    public void PlayFootsteps()
    {
        AudioManager.Instance.PlayEnemyFootstep();
    }
}
