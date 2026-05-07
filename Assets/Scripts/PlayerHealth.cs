using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Healing Settings")]
    [Tooltip("Maximum number of heals allowed.")]
    public int maxHeals = 3;
    [Tooltip("Wait time between heals (in seconds).")]
    public float healCooldown = 30f;
    
    private int healsRemaining;
    private float cooldownTimer = 0f;

    [Header("Invulnerability Settings")]
    public float invulnerabilityDuration = 1.0f;
    
    [Header("Model References")]
    public Transform playerModel; // <--- This is the magic variable!
    
    // We use an array now so we can blink EVERY part of your 3D model
    private Renderer[] targetRenderers;
    private PlayerMovement playerMovement;

    public bool isInvulnerable { get; private set; }

    private void Start()
    {
        currentHealth = maxHealth;
        healsRemaining = maxHeals;
        isInvulnerable = false;

        playerMovement = GetComponent<PlayerMovement>();

        // Grab ALL renderers attached to whatever model you dragged into the Inspector
        if (playerModel != null)
        {
            targetRenderers = playerModel.GetComponentsInChildren<Renderer>();
            
            if (targetRenderers.Length == 0)
            {
                Debug.LogWarning("PlayerHealth: The assigned playerModel has no Renderer components on it or its children.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerHealth: No playerModel assigned in the Inspector! The player will not blink when hit.");
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryHeal();
        }
    }

    private void TryHeal()
    {
        if (healsRemaining > 0 && cooldownTimer <= 0 && currentHealth < maxHealth)
        {
            currentHealth += (maxHealth * 0.5f);
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            healsRemaining--;
            cooldownTimer = healCooldown;
            
            Debug.Log($"Player healed. Current HP: {currentHealth}. Heals remaining: {healsRemaining}");
        }
        else if (cooldownTimer > 0)
        {
            Debug.Log($"Heal on cooldown. {cooldownTimer:F1} seconds remaining.");
        }
        else if (currentHealth >= maxHealth)
        {
            Debug.Log("Health is already at maximum.");
        }
        else if (healsRemaining <= 0)
        {
            Debug.Log("No heals remaining.");
        }
    }

    public bool TakeDamage(float amount)
    {
        if (isInvulnerable || (playerMovement != null && playerMovement.isDashing) || currentHealth <= 0) 
        {
            return false;
        }

        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            HandleDeath();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }

        return true; 
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float timer = 0f;
        float flickerSpeed = 0.1f; 

        while (timer < invulnerabilityDuration)
        {
            if (targetRenderers != null)
            {
                // Loop through every single piece of the 3D model and toggle it
                foreach (Renderer r in targetRenderers)
                {
                    if (r != null) r.enabled = !r.enabled;
                }
            }
            
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;
        }

        if (targetRenderers != null)
        {
            // Make sure everything is turned back on at the end
            foreach (Renderer r in targetRenderers)
            {
                if (r != null) r.enabled = true;
            }
        }
        isInvulnerable = false;
    }

    private void HandleDeath()
    {
        transform.position = new Vector3(0f, transform.position.y, 0f);
        Debug.Log($"Health reset to {currentHealth} after death.");
        
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        if (cc != null) cc.enabled = true;
    }
}