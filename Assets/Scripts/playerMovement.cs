using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Dependencies")]
    [Tooltip("Arrastra el CameraManager aquí para saber qué cámara está activa.")]
    public CameraController cameraManager;

    [Header("Movement Settings")]
    public float moveSpeed = 7.0f;
    public float rotationSpeed = 10.0f;

    [Header("Dash Settings")]
    public float dashForce = 25.0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    [Header("Components")]
    public Transform model;
    
    [Header("State")]
    public bool canMove = true;
    public bool isShielding = false; 

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection; 
    private Vector3 dashDirection;
    private Vector3 lastMoveDirection = Vector3.forward; 
    
    private float currentCooldown;
    public bool isDashing;
    private float dashTimer;

    private float verticalVelocity;
    private float gravity = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        HandleInput();
        ApplyMovementAndRotation();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        // Si no puede moverse por un ataque, detenemos todo
        if (!canMove && !isShielding)
        {
            moveDirection = Vector3.zero;
            targetRotationDirection = Vector3.zero;
            return;
        }

        // 1. Procesar input puro (WASD)
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
        if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
        if (Keyboard.current.wKey.isPressed) verticalInput += 1f;
        if (Keyboard.current.sKey.isPressed) verticalInput -= 1f;

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Configuración de movimiento y rotación basada en cámara y escudo
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
                moveDirection = Vector3.zero; // No moverse
                
                // CHANGED: Usar el input WASD relativo a la cámara para rotar mientras te cubres
                targetRotationDirection = camRelativeMovement; 
            }
            else
            {
                moveDirection = camRelativeMovement;
                targetRotationDirection = moveDirection; // Mirar hacia donde se camina
            }
        }
        else // Top-Down Mode
        {
            if (isShielding)
            {
                moveDirection = Vector3.zero; // No moverse
                targetRotationDirection = inputDirection; // Rotar usando WASD libremente
            }
            else
            {
                moveDirection = inputDirection;
                targetRotationDirection = moveDirection; // Mirar hacia donde se camina
            }
        }

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveDirection;
        }

        // 2. Procesar Dash (Solo si no estamos usando el escudo)
        if (!isShielding && Keyboard.current.spaceKey.wasPressedThisFrame && currentCooldown <= 0 && !isDashing)
        {
            dashDirection = lastMoveDirection;
            isDashing = true;
            dashTimer = dashDuration;
            currentCooldown = dashCooldown; 
        }
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

        finalMovement.y = verticalVelocity;
        controller.Move(finalMovement * Time.deltaTime);

        // Usamos targetRotationDirection en lugar de moveDirection para rotar el modelo
        if (model != null && targetRotationDirection.sqrMagnitude > 0.1f && !isDashing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetRotationDirection);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}