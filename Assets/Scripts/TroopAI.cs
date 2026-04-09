using UnityEngine;
using UnityEngine.AI;

public class TroopAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 1.5f;

    [Header("Combat")]
    public float attackRange = 2.5f;
    public float damagePerSecond = 1f;
    public float attackCooldown = 1f;
    public float attackAnimationDelay = 0.2f;     // ← Time before attack animation plays

    protected NavMeshAgent agent;
    protected Transform currentTarget;
    protected float lastAttackTime = 0f;
    protected Animator animator;

    protected enum State { Idle, Walking, Attacking }
    protected State currentState = State.Idle;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
            agent.autoBraking = true;
            agent.acceleration = 8f;
        }
    }

    protected virtual void Start()
    {
        if (agent != null)
            agent.stoppingDistance = stoppingDistance;
    }

    protected virtual void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        FindNearestEnemyTower();

        if (currentTarget == null)
        {
            ChangeState(State.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange)
        {
            if (currentState != State.Attacking)
            {
                ChangeState(State.Attacking);
            }
            AttackTarget();
        }
        else
        {
            if (currentState != State.Walking)
            {
                ChangeState(State.Walking);
            }
            MoveTowardsTarget();
        }
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (animator == null) return;

        switch (newState)
        {
            case State.Idle:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", false);
                break;

            case State.Walking:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsAttacking", false);
                break;

            case State.Attacking:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", true);
                break;
        }
    }

    private void FindNearestEnemyTower()
    {
        GameObject[] enemyTowers = GameObject.FindGameObjectsWithTag("EnemyTower");
        if (enemyTowers.Length == 0) return;

        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject tower in enemyTowers)
        {
            float dist = Vector3.Distance(transform.position, tower.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = tower.transform;
            }
        }

        if (nearest != currentTarget && nearest != null)
        {
            currentTarget = nearest;
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(currentTarget.position);
                agent.isStopped = false;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == null || agent == null || !agent.isOnNavMesh) return;
        agent.SetDestination(currentTarget.position);
    }

    protected virtual void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown) 
            return;

        lastAttackTime = Time.time;

        // Deal damage immediately
        Tower tower = currentTarget.GetComponent<Tower>();
        if (tower != null)
            tower.TakeDamage(damagePerSecond * attackCooldown);

        // Trigger attack animation AFTER a small delay
        if (animator != null)
        {
            Invoke("TriggerAttackAnimation", attackAnimationDelay);
        }
    }

    // This function is called after the delay
    private void TriggerAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }
}