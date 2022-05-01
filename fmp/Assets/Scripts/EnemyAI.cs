using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator enemyAnimator;
    public Transform player;
    public LayerMask ground, playerMask;
    public Vector3 walkPoint;

    private bool walkPointSet, alreadyAttacked, playerInSight, playerInAttack;
    public int damage = 0;
    public bool isWalking, isAttacking, isHurt, isIdle;

    [Header("Enemy Statistics")]
    public int enemyType;
    public GameObject enemyBody;
    [SerializeField]
    public float timeBetweenAttacks, sightRange, attackRange, walkPointRange, health;

    private void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, playerMask);
        damage = playerController.playerAttackDamage;
        if(!playerInSight && !playerInAttack) Patrolling();
        if(playerInSight && !playerInAttack) ChasePlayer();
        if(playerInSight && playerInAttack) AttackPlayer();
        SetAnimations();
    }

    private void SetAnimations()
    {
        enemyAnimator.SetBool("isWalking", isWalking);
        enemyAnimator.SetBool("isAttacking", isAttacking);
        enemyAnimator.SetBool("isHurt", isHurt);
        enemyAnimator.SetBool("isIdle", isIdle);
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Patrolling()
    {
        if(!walkPointSet)
        {
            SearchWalkPoint();
            isIdle = true;
            isWalking = false;
            isAttacking = false;
        }
        if(walkPointSet) 
        {
            agent.SetDestination(walkPoint);
            isIdle = false;
            isWalking = true;
        }
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

    private void ChasePlayer() 
    {
        agent.SetDestination(player.position);
        isAttacking = false;
        isIdle = false;
        isWalking = true;
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        isWalking = false;
        isIdle = false;
        isAttacking = true;
        Invoke(nameof(EnableCollider), 2f);
        if(!alreadyAttacked)
        {
            alreadyAttacked = true;
            isAttacking = false;
            enemyBody.GetComponent<Collider>().enabled = false;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "PlayerAttack")
        {
            TakeDamage(damage);
        }
    }
    private void EnableCollider() => enemyBody.GetComponent<Collider>().enabled = true;

    private void ResetAttack() => alreadyAttacked = false;

    public void TakeDamage(int damageTaken)
    {
        isHurt = true;
        health -= damageTaken;
        Invoke(nameof(StopHurt), 5f);
        if(health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void StopHurt() => isHurt = false;
    private void DestroyEnemy() => Destroy(gameObject);
}