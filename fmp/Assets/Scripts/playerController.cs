using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator playerAnimator;
    public Slider staminaBar, healthBar;
    public Transform cam, groundCheck, playerBody;
    public LayerMask groundMask;
    public float turnSmoothTime = 0.1f, mouseSensitivity, playerStamina = 100, playerHealth = 100;

    private const float gravity = -9.8f, jumpHeight = 3;

    private Vector2 movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle, mouseInputX, groundDistance = 0.4f, speed = 4;
    private int attackIndex = 1;
    private bool isGrounded, isCrouching, isAttacking, isWalking, isTurning, canMove;

    void Start() 
    { 
        Application.targetFrameRate = -1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GetPlayerInput();
        if (isAttacking) canMove = false;
        if (playerStamina < 100) playerStamina += (3 * Time.deltaTime);
        xRotation += mouseInputX;
        staminaBar.value = playerStamina;
        healthBar.value = playerHealth;
        print(playerStamina);
        SetAnimations();
    }

    public void StartAttack(int attack)
    {
        int staminaCost = attack switch
        {
            1 => 25,
            2 => 10,
            3 => 30,
            4 => 50,
            5 => 20,
            6 => 60,
            _ => 0
        };
        playerStamina -= staminaCost;
        string printAttack = attack switch
        {
            1 => "Swirling Chaos",
            2 => "Gravity Pull",
            3 => "Amplified Gravity",
            4 => "Black Hole",
            5 => "Galactic Pull",
            6 => "Gravity Well",
            _ => "Error"
        };
        print(printAttack);
        canMove = true;
    }

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
            if (speed > 4) playerStamina -= (6 * Time.deltaTime);
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        #endregion // Moves the player

        #region Turning
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation, 0);
        isTurning = (mouseInputX == 0) ? false : true;
        #endregion // Gets the input from the mouse and turns the player

         #region Jumping
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        #endregion // Checks if the player is grounded, then jumps when assigned Jump button is pressed
    }
}