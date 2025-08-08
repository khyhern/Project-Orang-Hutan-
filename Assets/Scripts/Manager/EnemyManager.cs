using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _respawnDelay = 10f;
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
        yield return new WaitForSeconds(_respawnDelay);
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
            _enemyAI.ResetRunAwayStatus();
            _respawn = false;
            _respawnPointSet = false;
        }

    }

    private void SearchRespawnPoint()
    {

        float randomZ = Random.Range(-_respawnRange, _respawnRange);
        float randomX = Random.Range(-_respawnRange, _respawnRange);
        if (Mathf.Abs(randomZ) < 18f || Mathf.Abs(randomX) < 18f) return;

        _respawnPoint = new Vector3(_player.position.x + randomX, _enemy.transform.position.y, _player.position.z + randomZ);
        if (Physics.Raycast(_respawnPoint, -transform.up, 2f, whatIsGround))
        {
            Debug.Log("Respawn point found: ");
            _respawnPointSet = true;
        }
    }
}
