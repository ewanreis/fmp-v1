using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask ground, playerMask;
    public Vector3 walkPoint;

    private bool walkPointSet, alreadyAttacked, playerInSight, playerInAttack;

    [Header("Enemy Statistics")]
    public int enemyType;
    [SerializeField]
    private float timeBetweenAttacks, sightRange, attackRange, walkPointRange, health;

    private void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(!playerInSight && !playerInAttack) Patrolling();
        if(playerInSight && !playerInAttack) ChasePlayer();
        if(playerInSight && playerInAttack) AttackPlayer();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Patrolling()
    {
        if(!walkPointSet) SearchWalkPoint();
        if(walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange,walkPointRange);
        float randomX = Random.Range(-walkPointRange,walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
            walkPointSet = true;
    }

    private void ChasePlayer() => agent.SetDestination(player.position);

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if(!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() => alreadyAttacked = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy() => Destroy(gameObject);
}