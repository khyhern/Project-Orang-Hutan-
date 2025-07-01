using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask whatIsGround, whatIsPlayer, whatIsThis;
    [SerializeField] private Transform _enemyAttackPoint;
    [SerializeField] private float _sightRange, _attackRange;
    [SerializeField] private float _walkPointRange;

    #region Internal
    private Transform _player;
    private NavMeshAgent _enemy;
    private float _speed;
    private Vector3 dirToPlayer;

    // Patrolling
    private Vector3 _walkPoint;
    private bool _walkPointSet;
    

    // Attacking
    private float _timeBetweenAttacks;
    private bool _alreadyAttacked;

    // States
    private bool _playerInSightRange, _playerInAttackRange;
    private bool _smtgInSightRange;
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
        dirToPlayer = _player.position - transform.position;
        Ray ray = new Ray(transform.position, dirToPlayer.normalized);
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
        if (!_playerInSightRange && !_playerInAttackRange) Patroling();
        if (_playerInSightRange && !_playerInAttackRange) ChasePlayer();
        if (_playerInAttackRange && _playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet)
        {
            _enemy.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
        Debug.Log("Patrolling");
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = Random.Range(-_walkPointRange, _walkPointRange);
        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
        {
            _walkPointSet = true;
        }
        _enemy.speed = _speed;
    }

    private void ChasePlayer()
    {
        _enemy.SetDestination(_player.position);
        _enemy.speed = _speed * 2f;
        Debug.Log("Chasing Player");
    }

    private void AttackPlayer()
    {
        _enemy.SetDestination(transform.position);

        transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            // Attack code here (Ryan)
            Debug.Log("Attack!");
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }

        Debug.Log("Attacking Player");
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, dirToPlayer.normalized * _sightRange, Color.green);
        
        if (_enemyAttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_enemyAttackPoint.position, _attackRange);
        }
    }
}
