using UnityEngine;
using UnityEngine.AI;

public class TroopAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float stoppingDistance = 1.6f;

    [Header("Combat")]
    public float attackRange = 2.3f;
    public float damagePerSecond = 1f;
    public float attackCooldown = 1.2f;
    public float attackAnimationDelay = 0.15f;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected Transform currentTarget;
    protected float lastAttackTime = 0f;
    protected Animator animator;

    protected enum State { Idle, Moving, Attacking }
    protected State currentState = State.Idle;

    private bool isPlayerTroop = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
            agent.autoBraking = true;
            agent.acceleration = 15f;

            //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            agent.radius = 0.25f;
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        int troopLayer = LayerMask.NameToLayer("troop");
        if (troopLayer != -1)
        {
            gameObject.layer = troopLayer;
            Physics.IgnoreLayerCollision(troopLayer, troopLayer, true);
        }
    }

    protected virtual void Start()
    {
        if (agent != null)
            agent.stoppingDistance = stoppingDistance;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
    }

    protected virtual void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        if (!isPlayerTroop && gameObject.CompareTag("PlayerTroop"))
            isPlayerTroop = true;

        // Re-check for nearest target ONLY when moving or when current target dies
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy || currentState == State.Moving)
        {
            FindNearestTarget();
        }

        if (currentTarget == null)
        {
            EnterIdleState();
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange)
        {
            if (currentState != State.Attacking)
                EnterAttackState();

            AttackTarget();
        }
        else
        {
            if (currentState != State.Moving)
                EnterMovingState();

            MoveToTarget();
        }
    }

    // Finds the absolute nearest target (troop or tower)
    private void FindNearestTarget()
    {
        string enemyTroopTag = isPlayerTroop ? "EnemyTroop" : "PlayerTroop";
        string enemyTowerTag = isPlayerTroop ? "EnemyTower" : "PlayerTower";

        Transform bestTarget = null;
        float bestDistance = Mathf.Infinity;

        // Check enemy troops
        GameObject[] enemyTroops = GameObject.FindGameObjectsWithTag(enemyTroopTag);
        foreach (GameObject troop in enemyTroops)
        {
            if (!troop.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, troop.transform.position);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestTarget = troop.transform;
            }
        }

        // Check enemy tower
        GameObject towerObj = GameObject.FindGameObjectWithTag(enemyTowerTag);
        if (towerObj != null)
        {
            float towerDist = Vector3.Distance(transform.position, towerObj.transform.position);
            if (towerDist < bestDistance)
            {
                bestDistance = towerDist;
                bestTarget = towerObj.transform;
            }
        }

        currentTarget = bestTarget;
    }

    private void EnterIdleState()
    {
        if (currentState == State.Idle) return;
        currentState = State.Idle;
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
    }

    private void EnterMovingState()
    {
        if (currentState == State.Moving) return;
        currentState = State.Moving;
        agent.isStopped = false;
        agent.updateRotation = true;

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);
    }

    private void EnterAttackState()
    {
        if (currentState == State.Attacking) return;
        currentState = State.Attacking;
        agent.isStopped = true;
        agent.updateRotation = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", true);
    }

    private void MoveToTarget()
    {
        if (currentTarget != null)
            agent.SetDestination(currentTarget.position);
    }

    protected virtual void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        Health health = currentTarget.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damagePerSecond * attackCooldown);
        else
        {
            Tower tower = currentTarget.GetComponent<Tower>();
            if (tower != null)
                tower.TakeDamage(damagePerSecond * attackCooldown);
        }

        if (animator != null)
            Invoke("TriggerAttackAnimation", attackAnimationDelay);
    }

    private void TriggerAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}