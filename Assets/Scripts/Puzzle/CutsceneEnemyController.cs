using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class CutsceneEnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damageAmount = 10;

    [Header("Camera & Effects")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Light redLight;
    [SerializeField] private Light whiteLight;
    [SerializeField] private CinemachineCamera cameraPlayer;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private bool isChasing;
    private bool hasAttacked;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("[CutsceneEnemyController] Player not found.");
    }

    private void Update()
    {
        if (!isChasing || player == null || hasAttacked)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;
            animator.SetBool("Run", true);
        }
        else
        {
            PerformAttack();
        }
    }

    public void BeginChase()
    {
        Debug.Log("[CutsceneEnemyController] BeginChase()");
        isChasing = true;
        hasAttacked = false;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
    }

    public void StopChase()
    {
        isChasing = false;
        agent.isStopped = true;
        animator.SetBool("Run", false);
    }

    private void PerformAttack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        animator.SetBool("Run", false);
        animator.SetTrigger("Attack1");

        hasAttacked = true;
    }

    // CALLED BY ANIMATION EVENT ONLY
    public void DamagePlayer()
    {
        Debug.Log($"[CutsceneEnemyController] DamagePlayer() → {damageAmount}");

        whiteLight.enabled = false;
        redLight.enabled = true;
        bloodEffect.SetActive(true);

        AudioManager.Instance.PlaySFXBlood();
        AudioManager.Instance.PlaySFXBreath();

        PlayerHealth.Instance?.DamagePlayer(damageAmount);

        StartCoroutine(PlayImpulseShake());
        StartCoroutine(CutsceneExit());
    }

    private IEnumerator PlayImpulseShake()
    {
        float time = 0f;
        while (time < 4f)
        {
            impulseSource.GenerateImpulseWithVelocity(new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.3f),
                0.2f
            ));
            yield return new WaitForSeconds(0.3f);
            time += 0.3f;
        }
    }

    private IEnumerator CutsceneExit()
    {
        yield return new WaitForSeconds(2.5f);
        redLight.enabled = false;
        bloodEffect.SetActive(false);

        yield return new WaitForSeconds(1f);
        CameraManager.SwitchCamera(cameraPlayer);
    }
}
