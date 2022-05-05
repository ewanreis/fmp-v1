using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public static int playerMoney = 0, attackIndex = 1;
    public static float playerStamina = 100;
    public static bool canMove = true, isAttacking = false;

    private Vector2 movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle, mouseInputX, groundDistance = 0.4f, speed = 4, regenDelay = 0, damageDelay = 2f, playerHealth = 100;
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
            xRotation += mouseInputX;
            staminaBar.value = playerStamina;
            healthBar.value = playerHealth;
            SetAnimations();
        }
    }

    public void StartAttack(int attack) 
    {
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
            damageDelay = 2f;
        }
        TakeDamage(damage);
    }

    private void TakeDamage(float damage) => playerHealth -= damage;

   
        

    void SetAnimations()
    {
        playerAnimator.SetFloat("horizontalMove", movementInput.x * speed / 4);
        playerAnimator.SetFloat("verticalMove", (movementInput.y * speed) / 4);
        playerAnimator.SetFloat("horizontalMouse", mouseInputX);
        playerAnimator.SetBool("crouching", isCrouching);
        playerAnimator.SetBool("attacking", isAttacking);
        playerAnimator.SetBool("jumping", !isGrounded);
        playerAnimator.SetInteger("attackIndex", attackIndex);
    }

    void GetPlayerInput()
    {
        isCrouching = (Input.GetKey(KeyCode.LeftControl)) ? true : false;
        isAttacking = (Input.GetButtonDown("Fire1")) ? true : false;
        if (Input.GetButtonDown("Fire2") == true) attackIndex++;
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
        {
            if (Input.GetKey(KeyCode.LeftShift) && speed < 8) speed += 0.1f;
            else if (!Input.GetKey(KeyCode.LeftShift) && speed > 4) speed -= 0.1f;
        }
        else if(speed > 4) speed -= 0.1f;
        if(direction.magnitude >= 0.1f && canMove)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (speed > 4) playerStamina -= (0.4f);
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        #endregion // Moves the player

        #region Turning
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation * 2, 0);
        isTurning = (mouseInputX == 0) ? false : true;
        #endregion // Gets the input from the mouse and turns the player
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
            playerStamina += 0.3f;

        // Damage Delay
        damageDelay -= 0.1f;
        if(damageDelay <= 0)
            damaged = false;
    }
}