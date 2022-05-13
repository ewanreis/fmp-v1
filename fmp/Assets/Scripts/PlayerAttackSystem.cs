using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    public GameObject player, vfxManager;
    public TMP_Text infoText;
    public static int playerAttackDamage = 0, staminaCost, damageModifier = 0;
    public static float attackDuration;
    public static float[] attackCooldown = new float[10], maxCooldown = new float[10];
    public static bool attackState = false;
    public static ForceMode forceMode = ForceMode.none;
    private bool alreadyAttacked;
    private int attack;

    private PlayerVFXManager vfxScript;

    private void Start() 
    {
        for(int i = 0; i < 9; i++)
            maxCooldown[i] = GetAttackCooldown(i);
        playerAreaCollider.radius = 0;
        playerAreaCollider.enabled = false;
        playerLineCollider.enabled = false;
        vfxScript = vfxManager.GetComponent<PlayerVFXManager>();
    } 

    private void FixedUpdate()
    {
        //print($"{attackCooldown[1]}, {attackCooldown[2]}, {attackCooldown[3]}");
        for(int i = 0; i < 9; i++)
        {
            attackCooldown[i] -= 0.1f;
        }
    }

    private void Update() 
    {
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
        attack = playerController.attackIndex;
        staminaCost = GetStaminaCost(attack);

        if(playerController.playerStamina < staminaCost && playerController.isAttacking == true)
            StartCoroutine(displayText("No Stamina!"));

        if (playerController.isAttacking == true && playerController.playerStamina >= staminaCost)
            StartAttack();
    }



    private void StartAttack()
    {
        int tempCoolDown = GetAttackCooldown(attack);

        if(attack == -1)
            StartCoroutine(displayText("No Stamina!"));
            
        if(!(attackCooldown[attack] > 0) && attack >= 0)
        {
            attackState = true;
            attackCooldown[attack] = GetAttackCooldown(attack);
            playerController.playerStamina -= staminaCost;
            float attackDuration = GetAttackDuration(attack), attackRadius = GetAttackRadius(attack);
            vfxScript.StartAttackVFX(attackDuration);
            PlayerSFXManager.attackSFX = true;
            PlayerSFXManager.attackIndexSFX = attack;
            playerAttackDamage = GetAttackDamage(attack) + damageModifier;
            forceMode = GetForceMode(attack);
            playerAreaCollider.radius = attackRadius;
            AccuracyMode accuracyMode = GetAccuracyMode(attack);
            if (accuracyMode == AccuracyMode.sphere)
                playerAreaCollider.enabled = true;

            if (accuracyMode == AccuracyMode.line)
                playerLineCollider.enabled = true;

            playerController.canMove = false;

            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                playerController.isAttacking = false;
                Invoke(nameof(ResetAttack), attackDuration);
            }
        }
        
    }

    private void ResetAttack() 
    {
        playerAreaCollider.enabled = false;
        playerLineCollider.enabled = false;
        playerController.canMove = true;
        forceMode = ForceMode.none;
        attackState = false;
        alreadyAttacked = false;
        playerAttackDamage = 0;
    }

    private IEnumerator displayText(string textToDisplay)
    {
        infoText.text = textToDisplay;
        playerController.isAttacking = false;
        yield return new WaitForSeconds(0.5f);
        attack = 0;
        yield return new WaitForSeconds(1.5f);
        infoText.text = "";
    }

    public int GetStaminaCost(int attack) => attack switch
    {
        0 => 10,
        1 => 15,
        2 => 30,
        3 => 20,
        4 => 20,
        5 => 15,
        6 => 25,
        7 => 30,
        8 => 100,
        _ => 0
    };

    public int GetAttackCooldown(int attack) => attack switch
    {
        0 => 45,
        1 => 55,
        2 => 65,
        3 => 3,
        4 => 5,
        5 => 10,
        6 => 3,
        7 => 5,
        8 => 10,
        _ => 0
    };
    
    public float GetAttackDuration(int attack) => attack switch
    {
        0 => 2.3f,
        1 => 2.3f,
        2 => 2.267f,
        3 => 2.6f,
        4 => 3.267f,
        5 => 2.7f,
        6 => 2.167f,
        7 => 2.2f,
        8 => 4.3f,
        _ => 0
    };

    public float GetAttackRadius(int attack) => attack switch
    {
        0 => 5,
        2 => 5,
        4 => 5,
        5 => 5,
        7 => 5,
        8 => 5,
        _ => 0
    };

    public int GetAttackDamage(int attack) => attack switch
    {
        0 => 2,
        1 => 4,
        2 => 6,
        3 => 4,
        4 => 8,
        5 => 12,
        6 => 8,
        7 => 16,
        8 => 24,
        _ => 0
    };

    public ForceMode GetForceMode(int attack) => attack switch
    {
        0 => ForceMode.pull, 
        1 => ForceMode.pull, 
        2 => ForceMode.freeze,
        3 => ForceMode.push, 
        4 => ForceMode.push, 
        5 => ForceMode.freeze,
        6 => ForceMode.pull, 
        7 => ForceMode.push, 
        8 => ForceMode.freeze,
        _ => ForceMode.none
    };

    public AccuracyMode GetAccuracyMode(int attack) => attack switch
    {
        0 => AccuracyMode.sphere,
        1 => AccuracyMode.line,
        2 => AccuracyMode.sphere,
        3 => AccuracyMode.line,
        4 => AccuracyMode.sphere,
        5 => AccuracyMode.sphere,
        6 => AccuracyMode.line,
        7 => AccuracyMode.sphere,
        8 => AccuracyMode.sphere,
        _ => AccuracyMode.sphere
    }; 
}