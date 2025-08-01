using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class CutsceneEnemyController : MonoBehaviour
{
    [Header("Chase Settings")]
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

    private bool hasAttacked;

    public static bool IsChasing { get; private set; }

    private const float ImpulseDuration = 4f;
    private const float ImpulseInterval = 0.3f;
    private const float ExitDelay1 = 2.5f;
    private const float ExitDelay2 = 1f;

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
        if (!IsChasing || player == null || hasAttacked) return;

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

    /// <summary>
    /// Begins the enemy chase.
    /// </summary>
    public void BeginChase()
    {
        Debug.Log("[CutsceneEnemyController] BeginChase()");
        IsChasing = true;
        hasAttacked = false;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
    }

    /// <summary>
    /// Stops the enemy chase.
    /// </summary>
    public void StopChase()
    {
        IsChasing = false;
        agent.isStopped = true;
        animator.SetBool("Run", false);
    }

    /// <summary>
    /// Starts attack animation and halts movement.
    /// </summary>
    private void PerformAttack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        animator.SetBool("Run", false);
        animator.SetTrigger("Attack1");
        hasAttacked = true;
    }

    /// <summary>
    /// Called by animation event to apply damage and effects.
    /// </summary>
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

    /// <summary>
    /// Plays camera shake for dramatic effect.
    /// </summary>
    private IEnumerator PlayImpulseShake()
    {
        float time = 0f;
        while (time < ImpulseDuration)
        {
            impulseSource.GenerateImpulseWithVelocity(new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.3f),
                0.2f
            ));
            yield return new WaitForSeconds(ImpulseInterval);
            time += ImpulseInterval;
        }
    }

    /// <summary>
    /// Cleans up post-attack effects and returns control.
    /// </summary>
    private IEnumerator CutsceneExit()
    {
        yield return new WaitForSeconds(ExitDelay1);
        redLight.enabled = false;
        bloodEffect.SetActive(false);

        yield return new WaitForSeconds(ExitDelay2);
        CameraManager.SwitchCamera(cameraPlayer);
    }
}
