using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento estándar del jugador.")]
    public float moveSpeed = 7.0f;
    [Tooltip("Velocidad de rotación del modelo hijo hacia la dirección de movimiento.")]
    public float rotationSpeed = 10.0f;

    [Header("Dash Settings")]
    [Tooltip("Fuerza/Distancia del dash.")]
    public float dashForce = 25.0f;
    [Tooltip("Tiempo en segundos que dura la acción del dash.")]
    public float dashDuration = 0.2f;
    [Tooltip("Tiempo en segundos que el jugador debe esperar entre dashes.")]
    public float dashCooldown = 1.0f;

    [Header("Components")]
    [Tooltip("El Transform hijo que representa el modelo visual que rotará.")]
    public Transform model;

    // Componentes y variables de estado
    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 dashDirection;
    
    // NUEVO: Guardamos la última dirección. Iniciamos en Vector3.forward por si hace dash nada más empezar.
    private Vector3 lastMoveDirection = Vector3.forward; 
    
    private float currentCooldown;
    private bool isDashing;
    private float dashTimer;

    // Variables básicas de gravedad
    private float verticalVelocity;
    private float gravity = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (model == null)
        {
            Debug.LogWarning("No has asignado el 'Model' en el script. La rotación no funcionará.");
        }
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

        // 1. Procesar movimiento (WASD)
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
        if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
        if (Keyboard.current.wKey.isPressed) verticalInput += 1f;
        if (Keyboard.current.sKey.isPressed) verticalInput -= 1f;

        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // NUEVO: Si nos estamos moviendo, actualizamos la última dirección conocida
        if (moveDirection.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveDirection;
        }

        // 2. Procesar Dash (Barra Espaciadora)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentCooldown <= 0 && !isDashing)
        {
            // Ahora el dash SIEMPRE usa la última dirección registrada,
            // asegurando que vaya hacia donde mira el modelo, incluso si estás quieto.
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
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            finalMovement = moveDirection * moveSpeed;
        }

        finalMovement.y = verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);

        
        if (model != null && moveDirection.sqrMagnitude > 0.1f && !isDashing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}