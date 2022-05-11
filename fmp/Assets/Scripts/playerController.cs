using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class playerController : MonoBehaviour
{
    private const float gravity = -9.8f, turnSmoothTime = 0.1f;

    public CharacterController controller;
    public Animator playerAnimator;
    public Slider staminaBar, healthBar;
    public Transform cam, groundCheck, playerBody;
    public LayerMask groundMask;
    public float mouseSensitivity;
    public TMP_Text moneyCounter;

    public static int playerMoney = 1000, attackIndex = 1, playerClass = 1;
    public static float playerStamina = 100;
    public static bool canMove = true, isAttacking = false;

    private Vector2 movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle, mouseInputX;
    private bool[] attackButtonPressed = new bool[3];

    private float groundDistance = 0.4f, speed = 4, regenDelay = 0, damageDelay = 2f, playerHealth = 100;
    private bool isGrounded, isCrouching, isWalking, isTurning, damaged = false;

    void Start() 
    { 
        Application.targetFrameRate = -1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        moneyCounter.text = $"{playerMoney}";
        if(!PauseMenu.isPaused)
        {
            GetPlayerInput();
            AttackInputSwitcher();
            xRotation += mouseInputX;
            staminaBar.value = playerStamina;
            healthBar.value = playerHealth;
            SetAnimations();
        }

        if(playerHealth <= 0)
            SceneManager.LoadScene("DeathScene", LoadSceneMode.Single);
    }

    public void StartAttack(int attack) 
    {
        if (attackIndex >= 0 && PlayerAttackSystem.attackCooldown[attackIndex] <= 0 && isAttacking == false && !PlayerVFXManager.isPlaying && playerStamina >= PlayerAttackSystem.staminaCost)
            isAttacking = true;
        attackIndex = attack;
        print($"{attack}, {attackIndex}");
    } 

    private void OnTriggerEnter(Collider other)
    {
        float damage = 0;
        if(damaged != true)
        {
            if(other.gameObject.tag == "nGhoul")
                damage = SpawningManager.ghoulDamage;

            if(other.gameObject.tag == "nGoblin")
                damage = SpawningManager.goblinDamage;

            if(other.gameObject.tag == "nKnight")
                damage = SpawningManager.goblinDamage;
        }
        if(damage > 0)
        {
            damaged = true;
            damageDelay = 1f;
        }
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        regenDelay = 0;
        playerHealth -= damage;
        if(damage > 0)
            PlayerSFXManager.damageSFX = true;
    }

    void SetAnimations()
    {
        playerAnimator.SetFloat("horizontalMove", movementInput.x * speed / 4);
        playerAnimator.SetFloat("verticalMove", (movementInput.y * speed) / 4);
        playerAnimator.SetFloat("horizontalMouse", mouseInputX);
        playerAnimator.SetBool("crouching", isCrouching);
        playerAnimator.SetBool("jumping", !isGrounded);
        playerAnimator.SetBool("attacking", isAttacking);
        playerAnimator.SetInteger("attackIndex", attackIndex);
    }

    private void AttackInputSwitcher()
    {
        bool canAttack = false;
        attackIndex = -1;
        int attackButtonIndex = playerClass switch
        {
            1 => 0,
            2 => 3,
            3 => 6,
            _ => 0
        };
        if (!isAttacking && !PlayerVFXManager.isPlaying)
        {
            if (attackButtonPressed[0]) attackIndex = 0 + attackButtonIndex;
            if (attackButtonPressed[1]) attackIndex = 1 + attackButtonIndex;
            if (attackButtonPressed[2]) attackIndex = 2 + attackButtonIndex;
            if(attackButtonPressed[0] || attackButtonPressed[1] || attackButtonPressed[2])
                canAttack = (attackIndex >= 0 && PlayerAttackSystem.attackCooldown[attackIndex] <= 0 && isAttacking == false && !PlayerVFXManager.isPlaying && playerStamina >= PlayerAttackSystem.staminaCost) ? true : false;
        }
        if (canAttack)
            isAttacking = true;
        //print($"is attacking:{isAttacking}\nattack Index: {attackIndex}\nplayer Stamina: {playerStamina}\nstamina cost {PlayerAttackSystem.staminaCost}\ncooldown {PlayerAttackSystem.attackCooldown}");
    }
    

    void GetPlayerInput()
    {
        isCrouching = (Input.GetKey(KeyCode.LeftControl)) ? true : false;
        if (!isAttacking && !PlayerVFXManager.isPlaying)
        {
            attackButtonPressed[0] = (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0)) ? true : false;
            attackButtonPressed[1] = (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(2)) ? true : false;
            attackButtonPressed[2] = (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1)) ? true : false;
        }

        if (!attackButtonPressed[0] && !attackButtonPressed[0] && !attackButtonPressed[0])
            isAttacking = false;

        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementInput.x = (Input.GetAxisRaw("Horizontal") == 0) ? 0 : movementInput.x;
        movementInput.y = (Input.GetAxisRaw("Vertical") == 0) ? 0 : movementInput.y;
        mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
    }

    // Player Physics + Movement
    void FixedUpdate()
    {
        #region Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion // Applies Gravity value at constant rate

        #region Walking
        isWalking = (movementInput.x != 0 || movementInput.y != 0) ? true : false;
        direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        if (playerStamina > 0)
        {   // Check if the player can sprint
            if (Input.GetKey(KeyCode.LeftShift) && speed < 8) speed += 0.1f;
            else if (!Input.GetKey(KeyCode.LeftShift) && speed > 4) speed -= 0.1f;
        }

        else if(speed > 4) 
            speed -= 0.1f;

        if(direction.magnitude >= 0.1f && canMove)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (speed > 4) 
                playerStamina -= (0.4f);

            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        #endregion // Moves the player

        #region Turning
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation * 2, 0);
        isTurning = (mouseInputX == 0) ? false : true;
        #endregion // Gets the input from the mouse and turns the player

        // Check if the player is touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Health Regen
        if(playerHealth < 100 && regenDelay < 30f)
            regenDelay += 0.1f;
        if(playerHealth < 100 && regenDelay >= 30f)
            playerHealth++;
        if(playerHealth == 100)
            regenDelay = 0;

        // Stamina Regen
        if (playerStamina < 100) 
            playerStamina += 0.5f;

        // Damage Delay
        damageDelay -= 0.1f;
        if(damageDelay <= 0)
            damaged = false;
    }
}