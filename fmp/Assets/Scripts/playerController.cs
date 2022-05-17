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
    public Transform cam, groundCheck, playerBody, playerHead;
    public LayerMask groundMask;
    public float mouseSensitivity;
    public TMP_Text moneyCounter;
    public GameObject damageText;
    public Rigidbody playerRB;

    public static int playerMoney = 0, attackIndex = 0, playerClass = 1, maxHealth = 100;
    public static float playerStamina = 100;
    public static bool canMove = true, isAttacking = false, isSliding = false;

    private Vector2 movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle, mouseInputX;
    private bool[] attackButtonPressed = new bool[3];

    private float groundDistance = 0.4f, speed = 4, regenDelay = 0, damageDelay = 2f, playerHealth = 100, slideDelay = 0;
    private bool isGrounded, isWalking, isTurning;
    private bool damaged = false;

    void Start()
    {
        playerMoney = 0;
        playerClass = 1;
        maxHealth = 100;
        playerStamina = 100;
        playerRB = GetComponent<Rigidbody>();
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

            if(isAttacking == true && attackIndex < 0)
                isAttacking = false;

            if(isSliding)
                Invoke(nameof(StopSliding), 1.5f);

            xRotation += mouseInputX;
            staminaBar.value = playerStamina;
            healthBar.maxValue = maxHealth;
            healthBar.value = playerHealth;

            SetAnimations();
        }

        if(playerHealth <= 0)
        {
            PlayerPrefs.SetInt("lastRoundReached", SpawningManager.round);
            SceneManager.LoadScene("DeathScene", LoadSceneMode.Single);
        }
    }

    public void StartAttack(int attack)
    {
        bool canAttack = (attackIndex >= 0
                          && PlayerAttackSystem.attackCooldown[attackIndex] <= 0
                          && isAttacking == false
                          && isSliding == false
                          && !PlayerVFXManager.isPlaying
                          && playerStamina >= PlayerAttackSystem.staminaCost)
                          ? true : false;

        if (canAttack)
            isAttacking = true;

        attackIndex = attack;
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
        {
            PlayerSFXManager.damageSFX = true;
            DamageIndicator indicator = Instantiate(damageText,
                                                playerHead.position,
                                                Quaternion.identity)
                                                .GetComponent<DamageIndicator>();
            indicator.SetDamageText(damage);
        }
    }

    void SetAnimations()
    {
        playerAnimator.SetFloat("horizontalMove", movementInput.x * speed / 4);
        playerAnimator.SetFloat("verticalMove", (movementInput.y * speed) / 4);
        playerAnimator.SetFloat("horizontalMouse", mouseInputX);
        playerAnimator.SetBool("sliding", isSliding);
        playerAnimator.SetBool("attacking", isAttacking);
        playerAnimator.SetInteger("attackIndex", attackIndex);
    }

    private void AttackInputSwitcher()
    {
        // Check if the player can attack, what class the player is and what button corresponds to what attack
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
            if (attackButtonPressed[0]) 
                attackIndex = 0 + attackButtonIndex;

            if (attackButtonPressed[1]) 
                attackIndex = 1 + attackButtonIndex;

            if (attackButtonPressed[2]) 
                attackIndex = 2 + attackButtonIndex;

            if(attackButtonPressed[0] || attackButtonPressed[1] || attackButtonPressed[2])
                canAttack = (attackIndex >= 0
                             && PlayerAttackSystem.attackCooldown[attackIndex] <= 0
                             && isAttacking == false
                             && !PlayerVFXManager.isPlaying
                             && playerStamina >= PlayerAttackSystem.staminaCost)
                             ? true : false;
        }
        if (canAttack)
            isAttacking = true;
    }
    

    void GetPlayerInput()
    {
        if (!isAttacking && !PlayerVFXManager.isPlaying)
        {
            attackButtonPressed[0] = (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0)) ? true : false;
            attackButtonPressed[1] = (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(2)) ? true : false;
            attackButtonPressed[2] = (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1)) ? true : false;
        }

        if (!attackButtonPressed[0] && !attackButtonPressed[0] && !attackButtonPressed[0])
            isAttacking = false;

        if(Input.GetKeyDown(KeyCode.LeftControl) && speed > 4 && slideDelay <= 0)
            isSliding = true;

        movementInput = new Vector2(Input.GetAxis("Horizontal"),
                                    Input.GetAxis("Vertical"));

        movementInput.x = (Input.GetAxisRaw("Horizontal") == 0) ? 0 : movementInput.x;
        movementInput.y = (Input.GetAxisRaw("Vertical") == 0) ? 0 : movementInput.y;

        mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation * 2, 0);
    }

    void FixedUpdate() // Player Physics + Movement
    {
        #region Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion // Applies Gravity value at constant rate

        #region Walking
        isWalking = (movementInput.x != 0 || movementInput.y != 0) ? true : false;

        direction = new Vector3(movementInput.x,
                                0f,
                                movementInput.y).normalized;

        if (playerStamina > 0) // Check if the player can sprint
        {
            if(isSliding && speed < 10)
                speed += 0.1f;

            if (Input.GetKey(KeyCode.LeftShift) && speed < 8)
                speed += 0.1f;

            else if (!Input.GetKey(KeyCode.LeftShift) && speed > 4)
                speed -= 0.1f;
        }

        else if(speed > 4)
            speed -= 0.1f;

        if(direction.magnitude >= 0.01f && canMove)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z)
                            * Mathf.Rad2Deg
                            + cam.eulerAngles.y;

            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                                          targetAngle,
                                          ref turnSmoothVelocity,
                                          turnSmoothTime);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);

            if (speed > 4)
                playerStamina -= (0.4f);
        }
        #endregion // Moves the player

        #region Turning
        isTurning = (mouseInputX == 0) ? false : true;
        #endregion // Gets the input from the mouse and turns the player

        // Check if the player is touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Health Regen
        if(playerHealth < maxHealth && regenDelay < 30f)
            regenDelay += 0.1f;

        if(playerHealth < maxHealth && regenDelay >= 30f)
            playerHealth++;

        if(playerHealth == maxHealth)
            regenDelay = 0;

        // Stamina Regen
        if (playerStamina < 100) 
            playerStamina += 0.3f;

        // Damage Delay
        damageDelay -= 0.1f;

        // Slide Delay
        slideDelay -= 0.1f;

        if(damageDelay <= 0)
            damaged = false;
    }

    private void StopSliding() 
    {
        slideDelay = 3f;
        isSliding = false;
    }
}

/*
* Planned features
? May add player damage immunity while sliding

* Issues
! Player Animations still play when the player has no stamina when attacking
*/

/*
        
*/