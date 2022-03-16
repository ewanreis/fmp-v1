using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class playerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator playerAnimator;
    public Transform cam, groundCheck, playerBody;
    public LayerMask groundMask;
    public float speed = 6, gravity = -9.81f, jumpHeight = 3, turnSmoothTime = 0.1f, groundDistance = 0.4f, mouseSensitivity;

    private Vector2 mouseInput, movementInput;
    private Vector3 moveDir, velocity, direction;
    private float xRotation, turnSmoothVelocity, targetAngle, angle;
    public bool isGrounded;

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void FixedUpdate()
    {
        #region Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion // Applies Gravity value at constant rate

        #region Walking
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isWalking = (movementInput.x != 0 || movementInput.y != 0) ? true : false;
        playerAnimator.SetFloat("horizontalMove", movementInput.x);
        
        direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        if(direction.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            speed = (Input.GetKey(KeyCode.LeftShift)) ? 8 : 4;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        playerAnimator.SetFloat("verticalMove", (movementInput.y * speed) / 4);
        #endregion // Moves the player

        #region Turning
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;
        xRotation += mouseInput.x;
        playerBody.transform.rotation = Quaternion.Euler(0, xRotation, 0);
        bool isTurning = (mouseInput.x == 0) ? false : true;
        playerAnimator.SetFloat("horizontalMouse", mouseInput.x);
        #endregion // Gets the input from the mouse and turns the player

         #region Jumping
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        #endregion // Checks if the player is grounded, then jumps when assigned Jump button is pressed
    }
}