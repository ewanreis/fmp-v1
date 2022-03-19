using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class playerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator playerAnimator;
    public Transform cam, groundCheck, playerBody;
    public LayerMask groundMask;
    public float speed = 4, gravity = -9.81f, jumpHeight = 3, turnSmoothTime = 0.1f, groundDistance = 0.4f, mouseSensitivity, playerStamina = 100;

    private Vector2 movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle, mouseInputX;
    private int attackIndex = 1;
    private bool isGrounded, isCrouching, isAttacking, isWalking, isTurning, canMove;

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
        isCrouching = (Input.GetKey(KeyCode.LeftControl)) ? true : false;
        isAttacking = (Input.GetButtonDown("Fire1")) ? true : false;
        if (isAttacking) canMove = false;
        if (Input.GetButtonDown("Fire2") == true) attackIndex++;
        if (playerStamina < 100) playerStamina += (1 * Time.deltaTime);
        mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        xRotation += mouseInputX;
        SetAnimations();
    }

    public void StartAttack(int attack)
    {
        switch (attack)
        {
            case 1:
            print("Swirling Chaos");
            canMove = true;
            break;
            case 2:
            print("Gravity Pull");
            canMove = true;
            break;
            case 3:
            print("Amplified Gravity");
            canMove = true;
            break;
            case 4:
            print("Black Hole");
            canMove = true;
            break;
            case 5:
            print("Galactic Pull");
            canMove = true;
            break;
            case 6:
            print("Gravity Well");
            canMove = true;
            break;
        }
    }

    void SetAnimations()
    {
        playerAnimator.SetFloat("horizontalMove", movementInput.x * speed / 4);
        playerAnimator.SetFloat("verticalMove", (movementInput.y * speed) / 4);
        playerAnimator.SetFloat("horizontalMouse", mouseInputX);
        playerAnimator.SetBool("crouching", isCrouching);
        playerAnimator.SetBool("attacking", isAttacking);
        playerAnimator.SetInteger("attackIndex", attackIndex);
        playerAnimator.SetBool("jumping", !isGrounded);
    }

    void FixedUpdate()
    {
        #region Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion // Applies Gravity value at constant rate

        #region Walking
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isWalking = (movementInput.x != 0 || movementInput.y != 0) ? true : false;
        direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        if(direction.magnitude >= 0.1f && canMove)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            speed = (Input.GetKey(KeyCode.LeftShift)) ? 8 : 4;
            if (speed > 4) playerStamina -= (3 * Time.deltaTime);
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        #endregion // Moves the player

        #region Turning
        mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        xRotation += mouseInputX;
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation, 0);
        isTurning = (mouseInputX == 0) ? false : true;
        //print(mouseInputX);
        #endregion // Gets the input from the mouse and turns the player

         #region Jumping
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        #endregion // Checks if the player is grounded, then jumps when assigned Jump button is pressed
    }
}