// ...existing code...
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack, Dead }
    public enum Phase { Phase1, Phase2, Phase3 }

    [Header("Health")]
    [SerializeField] float maxHealth = 400f;
    [SerializeField] float phase2Threshold = 0.66f; // fraction of maxHealth
    [SerializeField] float phase3Threshold = 0.33f;

    [Header("Sensing / Movement")]
    [SerializeField] float detectRadius = 25f;
    [SerializeField] float attackRange = 6f;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float patrolWait = 2f;
    [SerializeField] float moveSpeed = 3.5f;

    [Header("Attacks")]
    [SerializeField] float baseAttackCooldown = 2f;
    [SerializeField] GameObject minionPrefab;
    [SerializeField] Transform[] minionSpawnPoints;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyWeaponControls weaponControls;

    float currentHealth;
    State state = State.Idle;
    Phase phase = Phase.Phase1;
    int patrolIndex = 0;
    bool isAttacking = false;
    Coroutine attackRoutine;
    BossEnemySetup bossSetup;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Setup(BossEnemySetup setup)
    {
        bossSetup = setup;
        weaponControls.Setup(bossSetup);

        currentHealth = maxHealth;
        agent = agent ? agent : GetComponent<NavMeshAgent>();
        if (agent) agent.speed = moveSpeed;
        ChoosePhase();
        if (patrolPoints != null && patrolPoints.Length > 0) state = State.Patrol; else state = State.Idle;
    }

    void Update()
    {
        if (state == State.Dead) return;

        var player = PlayerManager.Instance.GetLocalPlayer();
        if(player == null)
            return;

        float distToPlayer = player ? Vector3.Distance(transform.position, player.transform.position) : Mathf.Infinity;

        // State transitions
        if (player != null && distToPlayer <= detectRadius)
        {
            if (distToPlayer <= attackRange)
            {
                ChangeState(State.Attack);
            }
            else
            {
                ChangeState(State.Chase);
            }
        }
        else
        {
            if (patrolPoints != null && patrolPoints.Length > 0) 
                ChangeState(State.Patrol);
            else
                ChangeState(State.Idle);
        }

        // State behaviours
        switch (state)
        {
            case State.Idle:
                if (animator) animator.SetBool("isMoving", false);
                if (agent) agent.isStopped = true;
                break;

            case State.Patrol:
                PatrolBehavior();
                break;

            case State.Chase:
                if (animator) animator.SetBool("isMoving", true);
                if (agent && player)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                }
                break;

            case State.Attack:
                if (!isAttacking)
                {
                    attackRoutine = StartCoroutine(AttackRoutine());
                }
                if (agent) agent.isStopped = true;
                break;
        }
    }

    void ChangeState(State newState)
    {
        if (state == State.Dead) return;
        if (state == newState) return;

        // exit logic
        if (state == State.Attack && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
            isAttacking = false;
        }

        state = newState;
    }

    void PatrolBehavior()
    {
        if (agent == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.speed = moveSpeed;
        Transform target = patrolPoints[patrolIndex];
        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            StartCoroutine(AdvancePatrolAfterWait());
        }

        if (animator) animator.SetBool("isMoving", true);
    }

    IEnumerator AdvancePatrolAfterWait()
    {
        agent.isStopped = true;
        if (animator) animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(patrolWait);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.isStopped = false;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        while (state == State.Attack && state != State.Dead)
        {
            switch (phase)
            {
                case Phase.Phase1:
                    yield return StartCoroutine(Phase1_Attack());
                    break;
                case Phase.Phase2:
                    yield return StartCoroutine(Phase2_Attack());
                    break;
                case Phase.Phase3:
                    yield return StartCoroutine(Phase3_Attack());
                    break;
            }
            float cooldown = baseAttackCooldown * (phase == Phase.Phase2 ? 0.9f : phase == Phase.Phase3 ? 0.7f : 1f);
            yield return new WaitForSeconds(cooldown);
        }
        isAttacking = false;
    }

    IEnumerator Phase1_Attack()
    {
        if (animator) animator.SetTrigger("attack");
        weaponControls.Fire();
        yield return null;
    }

    IEnumerator Phase2_Attack()
    {
        // burst of 3 projectiles
        if (animator) animator.SetTrigger("attack");
        for (int i = 0; i < 3; i++)
        {
            weaponControls.Fire();

            yield return new WaitForSeconds(0.18f);
        }
    }

    IEnumerator Phase3_Attack()
    {
        // heavy attack + spawn minions
        if (animator) animator.SetTrigger("heavyAttack");

        // heavy projectile(s)
        weaponControls.Fire();

        // spawn minions if configured
        if (minionPrefab != null && minionSpawnPoints != null && minionSpawnPoints.Length > 0)
        {
            foreach (var s in minionSpawnPoints)
            {
                Instantiate(minionPrefab, s.position, s.rotation);
            }
        }

        yield return null;
    }

    public void TakeDamage(float amount)
    {
        if (state == State.Dead) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (animator) animator.SetTrigger("hit");
        ChoosePhase();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void ChoosePhase()
    {
        float ratio = currentHealth / maxHealth;
        Phase newPhase = Phase.Phase1;
        if (ratio <= phase3Threshold) newPhase = Phase.Phase3;
        else if (ratio <= phase2Threshold) newPhase = Phase.Phase2;

        if (newPhase != phase)
        {
            phase = newPhase;
            OnPhaseChanged(phase);
        }
    }

    void OnPhaseChanged(Phase newPhase)
    {
        // adjust AI parameters by phase
        switch (newPhase)
        {
            case Phase.Phase1:
                baseAttackCooldown = Mathf.Max(0.8f, baseAttackCooldown);
                break;
            case Phase.Phase2:
                baseAttackCooldown *= 0.85f;
                if (agent) agent.speed = moveSpeed * 1.05f;
                break;
            case Phase.Phase3:
                baseAttackCooldown *= 0.8f;
                if (agent) agent.speed = moveSpeed * 1.15f;
                break;
        }
    }

    void Die()
    {
        state = State.Dead;
        if (animator) animator.SetTrigger("die");
        if (agent) agent.isStopped = true;
        // stop coroutines
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        // optional: drop loot, play VFX, disable collider
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        // destroy after delay
        Destroy(gameObject, 6f);
    }

    // Editor visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}