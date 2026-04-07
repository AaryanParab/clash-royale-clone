using UnityEngine;
using UnityEngine.AI;

public class TroopAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public float attackRange = 2.5f;
    public float damagePerSecond = 1f;
    public float attackCooldown = 1f;

    protected NavMeshAgent agent;
    protected Transform currentTarget;
    protected float lastAttackTime = 0f;

    protected enum State { Moving, Attacking }
    protected State currentState = State.Moving;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = attackRange - 0.5f;
            agent.updateRotation = true;
            agent.autoBraking = false;           // ← Important
            agent.acceleration = 8f;             // ← Helps movement start
        }
    }

    protected virtual void Start()
    {
        Invoke("ForceMove", 0.4f);               // Small delay to ensure NavMesh is ready
    }

    private void ForceMove()
    {
        if (agent != null && currentTarget != null && agent.isOnNavMesh)
        {
            agent.SetDestination(currentTarget.position);
            agent.isStopped = false;
        }
    }

    protected virtual void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        FindNearestEnemyTower();

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange)
        {
            if (currentState != State.Attacking)
            {
                currentState = State.Attacking;
                agent.isStopped = true;
            }
            AttackTarget();
        }
        else
        {
            if (currentState != State.Moving)
            {
                currentState = State.Moving;
                agent.isStopped = false;
            }
            MoveTowardsTarget();
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
                agent.isStopped = false;                    // Force it to move
                Debug.Log(gameObject.name + " → moving to " + currentTarget.name);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == null || agent == null || !agent.isOnNavMesh) 
            return;

        agent.SetDestination(currentTarget.position);
        agent.isStopped = false;
    }

    protected virtual void AttackTarget()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        Tower tower = currentTarget.GetComponent<Tower>();
        if (tower != null)
            tower.TakeDamage(damagePerSecond * attackCooldown);
    }
}