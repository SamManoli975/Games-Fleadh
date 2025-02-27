using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum MovementState
{
    idle,
    walking,
    running,
    crouching,
    crouchingMoving
}

[RequireComponent(typeof(CharacterController))]
public class Movement : NetworkBehaviour
{
    
    public Transform orientationTransform;

    public float movementSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 3f;

    public bool allowSprint = false;

    public float jumpPower = 3f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    public float maxStamina = 3f;
    public float staminaRegenRate = 1f;
    public float staminaRegenDelay = 2f;


    public Image staminaBar;
    public AudioSource exhaustionSound;

    float timeSinceLastSprint = 0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    [SerializeField] private Animator animator;

    [SerializeField] private FootstepPlayer footstepPlayer;

    private NetworkVariable<float> currentStamina;
    private NetworkVariable<MovementState> movementState = new NetworkVariable<MovementState>(MovementState.idle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    CapsuleCollider capsuleCollider;
    Vector3 originalCcCenter;
    float orientationOriginalY;

    [SerializeField] bool isStunned;

    void Awake()
    {
        currentStamina = new NetworkVariable<float>(maxStamina, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();
        originalCcCenter = characterController.center;
        orientationOriginalY = orientationTransform.localPosition.y;

        if (exhaustionSound != null)
        {
            exhaustionSound.volume = 0f;
        }

        if (IsOwner)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.onGameStarted += LockCursor;
                GameManager.instance.onGameEnded += UnlockCursor;
            }
            else
            {
                LockCursor();
            }
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
            GetComponent<CharacterController>().enabled = true;

        if (!IsOwner)
        {
            currentStamina.OnValueChanged += (float previous, float current) => HandleExhaustionSound();
            movementState.OnValueChanged += (MovementState previous, MovementState current) => HandleExhaustionSound();
        }

        movementState.OnValueChanged += HandleMovementStateChange;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (GameManager.instance != null && !GameManager.instance.IsGameRunning())
            return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        orientationTransform.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        if(isStunned)
                return;

        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        bool isRunning = allowSprint && Input.GetKey(KeyCode.LeftShift) && currentStamina.Value > 0;
        bool isCrouching = allowSprint && Input.GetKey(KeyCode.C);

        
        MovementState prevState = movementState.Value;
        if (isMoving && isRunning)
        {
            movementState.Value = MovementState.running;
            if (prevState != movementState.Value)
                animator.SetTrigger("goRunning");
        }
        else if (isMoving && isCrouching)
        {
            movementState.Value = MovementState.crouchingMoving;
            if (prevState != movementState.Value)
                animator.SetTrigger("goCrouchingMoving");
        }
        else if (isMoving)
        {
            movementState.Value = MovementState.walking;
            if (prevState != movementState.Value)
                animator.SetTrigger("goWalking");
        }
        else if (isCrouching)
        {
            movementState.Value = MovementState.crouching;
            if (prevState != movementState.Value)
                animator.SetTrigger("goCrouching");
        }
        else
        {
            movementState.Value = MovementState.idle;
            if (prevState != movementState.Value)
                animator.SetTrigger("goIdle");
        }

        float curSpeed = movementSpeed;
        if (movementState.Value == MovementState.running)
        {
            curSpeed = runSpeed;
        }
        else if (movementState.Value == MovementState.crouching || movementState.Value == MovementState.crouchingMoving)
        {
            curSpeed = crouchSpeed;
        }

        float curSpeedX = curSpeed * Input.GetAxis("Vertical");
        float curSpeedY = curSpeed * Input.GetAxis("Horizontal");

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        
        if (Input.GetButton("Jump") && characterController.isGrounded)
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

        characterController.Move(moveDirection * Time.deltaTime);

        
        if (movementState.Value == MovementState.running)
        {
            currentStamina.Value -= Time.deltaTime;
            timeSinceLastSprint = 0f;
        }
        else
        {
            timeSinceLastSprint += Time.deltaTime;

            if (timeSinceLastSprint >= staminaRegenDelay && currentStamina.Value < maxStamina)
            {
                currentStamina.Value += staminaRegenRate * Time.deltaTime;
            }
        }

        currentStamina.Value = Mathf.Clamp(currentStamina.Value, 0, maxStamina);

        
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina.Value / maxStamina;
        }

        HandleExhaustionSound();
    }

    void HandleMovementStateChange(MovementState previous, MovementState current) {
        
        bool nowCrouching = current == MovementState.crouching || current == MovementState.crouchingMoving;
        bool wasCrouching = previous == MovementState.crouching || previous == MovementState.crouchingMoving;
        if (nowCrouching)
        {
            if (!wasCrouching)
            {
                characterController.height = crouchHeight;
                characterController.center = originalCcCenter * crouchHeight / defaultHeight;
                capsuleCollider.height = characterController.height;
                capsuleCollider.center = characterController.center;

                if(IsOwner) 
                {
                    Vector3 orientationTransformPos = orientationTransform.localPosition;
                    orientationTransformPos.y = orientationOriginalY * crouchHeight / defaultHeight;
                    orientationTransform.localPosition = orientationTransformPos;
                }
            }
        }
        else
        {
            if (wasCrouching)
            {
                if(IsOwner) 
                {
                    float yDiff = (defaultHeight - crouchHeight) / 2;
                    characterController.enabled = false;
                    transform.position += new Vector3(0, yDiff * transform.localScale.y, 0);
                    characterController.enabled = true;
                }

                characterController.height = defaultHeight;
                characterController.center = originalCcCenter;
                capsuleCollider.height = characterController.height;
                capsuleCollider.center = characterController.center;

                 if(IsOwner) {
                    Vector3 orientationTransformPos = orientationTransform.localPosition;
                    orientationTransformPos.y = orientationOriginalY;
                    orientationTransform.localPosition = orientationTransformPos;
                }
            }
        }
    }

    void HandleExhaustionSound()
    {
        if (exhaustionSound != null)
        {
            if (movementState.Value == MovementState.running && !exhaustionSound.isPlaying)
            {
                exhaustionSound.Play();
            }

            if (currentStamina.Value <= 0)
            {
                exhaustionSound.volume = 1.0f;
                if (!exhaustionSound.isPlaying)
                {
                    exhaustionSound.Play();
                }
            }
            else
            {
                exhaustionSound.volume = Mathf.Clamp(1.2f - (currentStamina.Value / maxStamina), 0.1f, 1.0f);
            }

            if (currentStamina.Value >= maxStamina)
            {
                exhaustionSound.Stop();
            }
        }
    }

    
    public void OnFootstep()
    {
        if(!IsOwner)
            return;

        if (characterController.isGrounded && movementState.Value != MovementState.idle)
        {
            footstepPlayer.PlayFootstep(movementState.Value == MovementState.crouching || movementState.Value == MovementState.crouchingMoving);
        }
    }

    public void SetIsStunned(bool value) {
        isStunned = value;
    }

    public void OnEndTaunt() {
        SetIsStunned(false);
    }

    public void OnStartTaunt() {
        SetIsStunned(true);
    }
}