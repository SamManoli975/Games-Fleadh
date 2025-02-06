using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public bool allowSprint = false;
    public Camera playerCamera;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpPower = 3f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    public float maxStamina = 3f;
    private float currentStamina;
    public float staminaRegenRate = 1f;
    public float staminaRegenDelay = 2f;
    private float timeSinceLastSprint = 0f;
    public bool isSprinting = false;
    public UnityEngine.UI.Image staminaBar;
    public AudioSource exhaustionSound;

    public AudioSource footstepSound; // New footstep AudioSource
    public AudioClip[] footstepClips; // Array of footstep sounds
    private bool isMoving = false;
    private bool isPlayingFootsteps = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentStamina = maxStamina;

        if (exhaustionSound != null)
        {
            exhaustionSound.volume = 0f;
        }
        animator = GetComponent<Animator>();

        // Debug check for footstepClips
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogError("FootstepClips is not assigned or empty!");
        }
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && allowSprint;

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Update Animator Parameters
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 3f;
            runSpeed = 6f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Sprinting logic (stamina handling)
        if (isRunning)
        {
            isSprinting = true;
            currentStamina -= Time.deltaTime;
            timeSinceLastSprint = 0f;

            if (!exhaustionSound.isPlaying)
            {
                exhaustionSound.Play();
            }
        }
        else
        {
            isSprinting = false;
            timeSinceLastSprint += Time.deltaTime;

            if (timeSinceLastSprint >= staminaRegenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Exhaustion sound volume scaling
        if (exhaustionSound != null)
        {
            if (currentStamina <= 0)
            {
                exhaustionSound.volume = 1.0f;
                if (!exhaustionSound.isPlaying)
                {
                    exhaustionSound.Play();
                }
            }
            else
            {
                exhaustionSound.volume = Mathf.Clamp(1.2f - (currentStamina / maxStamina), 0.1f, 1.0f);
            }

            if (currentStamina >= maxStamina)
            {
                exhaustionSound.Stop();
            }
        }

        // Footstep sounds logic
        // if (isMoving && characterController.isGrounded)
        // {
        //     if (!isPlayingFootsteps)
        //     {
        //         StartCoroutine(PlayFootsteps());
        //     }
        // }
        // else
        // {
        //     isPlayingFootsteps = false;
        //     footstepSound.Stop(); // Immediately stop footstep sound if not moving
        // }

        // Update UI Stamina Bar
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }

    // IEnumerator PlayFootsteps()
    // {
    //     isPlayingFootsteps = true;

    //     while (isMoving && characterController.isGrounded)
    //     {
    //         if (footstepClips.Length > 0)
    //         {
    //             footstepSound.clip = footstepClips[Random.Range(0, footstepClips.Length)];
    //             footstepSound.Play();
    //         }

    //         float stepInterval = isSprinting ? 0.3f : 0.5f; // Faster footsteps when running
    //         yield return new WaitForSeconds(stepInterval);

    //         // Check if movement stopped before continuing
    //         if (!isMoving || !characterController.isGrounded)
    //         {
    //             break; // Immediately exit loop if movement stops
    //         }
    //     }

    //     isPlayingFootsteps = false;
    // }

    // Method to handle footstep sound when triggered by Animation Event
    public void OnFootstep()
    {
        if (footstepClips.Length > 0 && characterController.isGrounded)
        {
            footstepSound.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSound.pitch = 0.8f + 0.1f * Random.Range(-1f, 1f);
            footstepSound.Play();
        }
    }
}