using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]    
    [SerializeField] private float _respawnRange;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask whatIsGround;

    private EnemyAI _enemyAI;
    private bool _respawnPointSet;
    private bool _respawn;
    private Vector3 _respawnPoint;

    private void Start()
    {
        _enemyAI = _enemy.GetComponent<EnemyAI>();
        RespawnDelay();
    }

    private void Update()
    {
        if (_respawn) Respawn();   
    }

    public void RespawnDelay()
    {
        _enemy.SetActive(false);
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(5f);
        _respawn = true;
    }

    private void Respawn()
    {
        if (!_respawnPointSet) SearchRespawnPoint();

        if (_respawnPointSet)
        {
            _enemy.transform.position = _respawnPoint;
            _enemy.SetActive(true);
            _enemyAI.respawn = false;
            _respawn = false;
        }

    }

    private void SearchRespawnPoint()
    {

        float randomZ = Random.Range(-_respawnRange, _respawnRange);
        float randomX = Random.Range(-_respawnRange, _respawnRange);
        randomZ = Mathf.Abs(randomZ) < 25f ? 25f : randomZ; // Ensure minimum distance
        randomX = Mathf.Abs(randomX) < 25f ? 25f : randomX; // Ensure minimum distance
        _respawnPoint = new Vector3(_player.position.x + randomX, _enemy.transform.position.y, _player.position.z + randomZ);
        if (Physics.Raycast(_respawnPoint, -transform.up, 2f, whatIsGround))
        {
            Debug.Log("Respawn point found: ");
            _respawnPointSet = true;
        }
    }
}
