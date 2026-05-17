using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Dependencies")]
    public CameraController cameraManager;

    [Header("Environment")]
    public BoxCollider arenaBounds;
    public float fallThreshold = 2f;

    [Header("Movement Settings")]
    public float moveSpeed = 7.0f;
    public float rotationSpeed = 10.0f;

    [Header("Dash Settings")]
    public float dashForce = 25.0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    [Header("Components")]
    public Transform model;
    public Animator animator; 
    
    [Header("State")]
    public bool canMove = true;
    public bool isShielding = false; 

    private CharacterController controller;
    private PlayerHealth playerHealth; 
    
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection; 
    private Vector3 dashDirection;
    private Vector3 lastMoveDirection = Vector3.forward; 
    
    private float currentCooldown;
    public bool isDashing;
    private float dashTimer;

    private Vector3 knockbackVelocity = Vector3.zero;
    private float knockbackTimer = 0f;

    private float verticalVelocity;
    private float gravity = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>(); 
    }

    private void Update()
    {
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;

        HandleInput();
        ApplyMovementAndRotation();
        
        CheckFallOutBounds(); 
    }

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        if (!canMove && !isShielding)
        {
            moveDirection = Vector3.zero;
            targetRotationDirection = Vector3.zero;
            return;
        }

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
        if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
        if (Keyboard.current.wKey.isPressed) verticalInput += 1f;
        if (Keyboard.current.sKey.isPressed) verticalInput -= 1f;

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (cameraManager != null && cameraManager.isOrbitalActive && Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 camRelativeMovement = (camForward * verticalInput + camRight * horizontalInput).normalized;

            if (isShielding)
            {
                moveDirection = Vector3.zero; 
                targetRotationDirection = camRelativeMovement; 
            }
            else
            {
                moveDirection = camRelativeMovement;
                targetRotationDirection = moveDirection; 
            }
        }
        else 
        {
            if (isShielding)
            {
                moveDirection = Vector3.zero; 
                targetRotationDirection = inputDirection; 
            }
            else
            {
                moveDirection = inputDirection;
                targetRotationDirection = moveDirection; 
            }
        }

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveDirection;
        }

        if (!isShielding && Keyboard.current.spaceKey.wasPressedThisFrame && currentCooldown <= 0 && !isDashing)
        {
            dashDirection = lastMoveDirection;
            isDashing = true;
            dashTimer = dashDuration;
            currentCooldown = dashCooldown; 
        }
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        knockbackVelocity = direction * force;
        knockbackTimer = duration;
    }

    private void ApplyMovementAndRotation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; 
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMovement;

        if (isDashing)
        {
            finalMovement = dashDirection * dashForce;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) isDashing = false;
        }
        else
        {
            finalMovement = moveDirection * moveSpeed;
        }

        if (knockbackTimer > 0)
        {
            finalMovement += knockbackVelocity;
            knockbackTimer -= Time.deltaTime;
        }

        finalMovement.y = verticalVelocity;
        controller.Move(finalMovement * Time.deltaTime);

        if (model != null && targetRotationDirection.sqrMagnitude > 0.1f && !isDashing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetRotationDirection);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetBool("isDashing", isDashing);
            animator.SetBool("isShielding", isShielding);
            animator.SetBool("isMoving", moveDirection.sqrMagnitude > 0.1f);
        }
    }

    private void CheckFallOutBounds()
    {
        if (arenaBounds != null && playerHealth != null)
        {
            float lowestPoint = arenaBounds.bounds.min.y;

            if (transform.position.y < lowestPoint - fallThreshold)
            {
                Debug.Log("Player fell out of bounds!");

                controller.enabled = false;
                
                transform.position = new Vector3(arenaBounds.bounds.center.x, arenaBounds.bounds.max.y + 1f, arenaBounds.bounds.center.z);
                
                controller.enabled = true;

                playerHealth.TakeDamage(99999f); 
            }
        }
    }
}