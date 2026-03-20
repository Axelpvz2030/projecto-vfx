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
    [Tooltip("Duration of i-frames after taking damage.")]
    public float invulnerabilityDuration = 1.0f;
    
    // We will only store the specific Renderer of "playerModel"
    private Renderer targetRenderer;
    
    // Reference to the movement script to check if dashing
    private PlayerMovement playerMovement;

    public bool isInvulnerable { get; private set; }

    private void Start()
    {
        currentHealth = maxHealth;
        healsRemaining = maxHeals;
        isInvulnerable = false;

        // Grab the movement script so we can check for dash i-frames
        playerMovement = GetComponent<PlayerMovement>();

        // AUTOMATIC SETUP: Search through all children and grandchildren
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        
        foreach (Transform child in allChildren)
        {
            if (child.name == "playerModel")
            {
                targetRenderer = child.GetComponent<Renderer>();
                break; // Stop searching once we find it
            }
        }

        if (targetRenderer == null)
        {
            Debug.LogWarning("PlayerHealth: Could not find an object named 'playerModel' or it lacks a Renderer component.");
        }
    }

    private void Update()
    {
        // Reduce the healing cooldown if active
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Detect healing input (E Key)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryHeal();
        }
    }

    private void TryHeal()
    {
        if (healsRemaining > 0 && cooldownTimer <= 0 && currentHealth < maxHealth)
        {
            // Heal 50% of maximum health
            currentHealth += (maxHealth * 0.5f);
            
            // Ensure we don't exceed maximum health
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
        // If the player is invulnerable from a hit OR currently dashing, ignore the damage completely!
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
            // Toggle ONLY the playerModel renderer on or off
            if (targetRenderer != null) targetRenderer.enabled = !targetRenderer.enabled;
            
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;
        }

        // Ensure it remains visible when finished
        if (targetRenderer != null) targetRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void HandleDeath()
    {
        
        transform.position = new Vector3(0f, transform.position.y, 0f);
        
        Debug.Log($"Health reset to {currentHealth} after death.");
        
        // Temporarily disable the CharacterController
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        // Move to X=0, Z=0, but keep current Y height
        
        // Re-enable it
        if (cc != null) cc.enabled = true;
    }
}