using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AccuracyMode
{
    sphere,
    line
}

public enum ForceMode
{
    pull,
    push,
    freeze,
    none
}

public class PlayerAttackSystem : MonoBehaviour
{
    public SphereCollider playerAreaCollider;
    public Collider playerLineCollider;
    public GameObject player;
    public static int playerAttackDamage = 0;
    public static float attackDuration, attackCooldown = 0;
    public static bool attackState = false;
    private bool alreadyAttacked;
    private int staminaCost, attack;

    private void Start() => playerAreaCollider.radius = 0;

    private void FixedUpdate()
    {
        if (attackCooldown != 0f) 
            attackCooldown -= 0.1f;
    }

    private void Update() 
    {
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
        attack = playerController.attackIndex;

        if (playerController.isAttacking == true && attackCooldown <= 0)
                StartAttack();
    }

    private void StartAttack()
    {
        attackState = true;
        int staminaCost = GetStaminaCost(attack);
        float attackDuration = GetAttackDuration(attack), attackRadius = GetAttackRadius(attack);
        playerAttackDamage = GetAttackDamage(attack);
        ForceMode forceMode = GetForceMode(attack);
        AccuracyMode accuracyMode = GetAccuracyMode(attack);
        playerAreaCollider.radius = attackRadius;

        if (accuracyMode == AccuracyMode.sphere)
            playerAreaCollider.enabled = true;

        if (accuracyMode == AccuracyMode.line)
            playerLineCollider.enabled = true;

        playerController.canMove = false;

        if (!alreadyAttacked)
        {
            attackCooldown = GetAttackCooldown(attack);
            alreadyAttacked = true;
            playerController.playerStamina -= staminaCost;
            playerController.isAttacking = false;
            Invoke(nameof(ResetAttack), attackDuration);
        }
    }

    private void ResetAttack() 
    {
        playerAreaCollider.enabled = false;
        playerLineCollider.enabled = true;
        playerController.canMove = true;
        attackState = false;
        alreadyAttacked = false;
        playerAttackDamage = 0;
    }

    public int GetStaminaCost(int attack) => attack switch
    {
        1 => 10,
        2 => 10,
        3 => 20,
        4 => 20,
        5 => 20,
        6 => 15,
        7 => 25,
        8 => 30,
        9 => 100,
        _ => 0
    };
    public int GetAttackCooldown(int attack) => attack switch
    {
        1 => 3,
        2 => 5,
        3 => 10,
        4 => 3,
        5 => 5,
        6 => 10,
        7 => 3,
        8 => 5,
        9 => 10,
        _ => 0
    };
    public float GetAttackDuration(int attack) => attack switch
    {
        1 => 1f,
        2 => .2f,
        3 => 1,
        4 => 1,
        5 => 1,
        6 => 3,
        7 => 2,
        8 => 3,
        9 => 2,
        _ => 0
    };
    public float GetAttackRadius(int attack) => attack switch
    {
        1 => 5,
        3 => 5,
        5 => 5,
        6 => 5,
        8 => 5,
        9 => 5,
        _ => 0
    };
    public int GetAttackDamage(int attack) => attack switch
    {
        1 => 5,
        2 => 10,
        3 => 25,
        4 => 10,
        5 => 20,
        6 => 50,
        7 => 25,
        8 => 50,
        9 => 75,
        _ => 0
    };
    public ForceMode GetForceMode(int attack) => attack switch
    {
        1 => ForceMode.pull, 
        2 => ForceMode.pull, 
        3 => ForceMode.freeze,
        4 => ForceMode.push, 
        5 => ForceMode.push, 
        6 => ForceMode.freeze,
        7 => ForceMode.pull, 
        8 => ForceMode.push, 
        9 => ForceMode.freeze,
        _ => ForceMode.none
    };
    public AccuracyMode GetAccuracyMode(int attack) => attack switch
    {
        1 => AccuracyMode.sphere,
        2 => AccuracyMode.line,
        3 => AccuracyMode.sphere,
        4 => AccuracyMode.line,
        5 => AccuracyMode.sphere,
        6 => AccuracyMode.sphere,
        7 => AccuracyMode.line,
        8 => AccuracyMode.sphere,
        9 => AccuracyMode.sphere,
        _ => AccuracyMode.sphere
    }; 
}

            /*  
            1 => "Swirling Chaos", 2 => "Gravity Pull", 3 => "Amplified Gravity", 
            4 => "Beam of Light", 5 => "Solar Flare", 6 => "Heatwave",
            7 => "Chaotic Pull", 8 => "Chaotic Push", 9 => "Self Regeneration",
            _ => "Error"
            */