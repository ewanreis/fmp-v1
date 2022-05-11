using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator enemyAnimator;
    public Transform player;
    public LayerMask ground, playerMask;
    public Vector3 walkPoint;
    public Slider healthBarSlider;
    public VisualEffect deathEffect, deathEffectInstance;

    private bool walkPointSet, alreadyAttacked, playerInSight, playerInAttack;
    public int damage = 0, currentRound;
    public bool isWalking, isAttacking, isHurt, isIdle;
    private bool damaged;

    [Header("Enemy Statistics")]
    public GameObject enemyBody;
    public int enemyType, moneyDrop;
    [SerializeField]
    public float timeBetweenAttacks, sightRange, attackRange, walkPointRange, health, damageDelay = 2f;

    private void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, playerMask);
        damage = PlayerAttackSystem.playerAttackDamage;
        health = Mathf.Floor(health);
        healthBarSlider.value = health;
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
        deathEffect = GameObject.Find("Death Effect").GetComponent<VisualEffect>();
        agent = GetComponent<NavMeshAgent>();

        currentRound = SpawningManager.round;
        health = enemyType switch
        {
            0 => 20 +((float)currentRound / 10),
            1 => 40 +((float)currentRound / 10),
            2 => 125 +((float)currentRound / 10),
            _ => 0
        };
        damage = enemyType switch
        {
            0 => 1 + currentRound,
            1 => 5 + currentRound,
            2 => 15 + currentRound,
            _ => 0
        };
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
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

    private void EnableCollider() => enemyBody.GetComponent<Collider>().enabled = true;

    private void ResetAttack() => alreadyAttacked = false;

    public void TakeDamage(float damageTaken)
    {
        isHurt = true;
        health -= damageTaken;
        Invoke(nameof(StopHurt), 0.5f);
    }

    void FixedUpdate()
    {
        damageDelay -= 0.1f;
        if(damageDelay <= 0)
            damaged = false;
        if(health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        float damage = 0;
        if(!isHurt)
        {
            if(other.gameObject.tag == "playerAttack")
            {
                damage = PlayerAttackSystem.playerAttackDamage;
                TakeDamage(damage);
            }
        }
        if(damage > 0)
        {
            damaged = true;
            damageDelay = .5f;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        float damage = 0;
        if(!isHurt)
        {
            if(other.gameObject.tag == "playerAttack")
            {
                damage = PlayerAttackSystem.playerAttackDamage;
                TakeDamage(damage);
            }
        }
        if(damage > 0)
        {
            damaged = true;
            damageDelay = .5f;
        }
    }


    //private void TakeDamage(float damage) => health -= damage;
    private void StopHurt() => isHurt = false;
    private void DestroyEnemy() 
    {
        playerController.playerMoney += moneyDrop;
        deathEffectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffectInstance.gameObject, 60f);
        Destroy(gameObject);
    }
}