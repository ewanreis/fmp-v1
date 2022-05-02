using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    public SphereCollider playerAttackCollider;
    public GameObject player;
    public static int playerAttackDamage = 0;
    private bool alreadyAttacked;
    private int staminaCost;

    private void Start()
    {
        playerAttackCollider.radius = 0;
    }
    private void Update() 
    {
        this.transform.position = player.transform.position;
        if(playerController.isAttacking == true)
        {
            int attack = playerController.attackIndex;
            staminaCost = attack switch
            {
                1 => 10, 2 => 10, 3 => 20,
                4 => 20, 5 => 20, 6 => 15,
                7 => 25, 8 => 30, 9 => 100,
                _ => 0
            };
            float duration = attack switch
            {
                1 => 1, 2 => 0.1f, 3 => 1,
                4 => 1, 5 => 1, 6 => 3,
                7 => 2, 8 => 3, 9 => 2,
                _ => 0
            };
            string printAttack = attack switch
            {
                1 => "Swirling Chaos", 2 => "Gravity Pull", 3 => "Amplified Gravity", 
                4 => "Beam of Light", 5 => "Solar Flare", 6 => "Heatwave",
                7 => "Chaotic Pull", 8 => "Chaotic Push", 9 => "Self Regeneration",
                _ => "Error"
            };
            playerAttackDamage = attack switch
            {
                1 => 5, 2 => 10, 3 => 25,
                4 => 10, 5 => 20, 6 => 50,
                7 => 25, 8 => 50, 9 => 75,
                _ => 0
            };
            
            float attackRadius = 10f;
            playerAttackCollider.enabled = true;
            playerAttackCollider.radius = attackRadius;
            
            if(!alreadyAttacked)
            {
                alreadyAttacked = true;
                playerController.playerStamina -= staminaCost;
                
                Invoke(nameof(ResetAttack), duration);
            }
            print(attack);
            playerController.canMove = true;
        }
    }

    private void ResetAttack() 
    {
        playerAttackCollider.enabled = false;
        alreadyAttacked = false;
        playerController.isAttacking = false;
        playerAttackDamage = 0;
    } 
}
