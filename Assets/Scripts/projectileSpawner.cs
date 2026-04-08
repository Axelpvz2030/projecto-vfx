using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public bool isActive = true; 
    public float spawnTime = 1f;
    public GameObject projectileModel;
    public GameObject projectilePrefab;

    private float currentTimer = 0f;

    private void Start()
    {
        // Set model visibility based on the initial state of isActive
        if (projectileModel != null) projectileModel.SetActive(isActive);
    }

    public void SetSpawnerActive(bool state)
    {
        if (state && spawnTime <= 0f)
        {
            SpawnProjectile();
            return;
        }

        isActive = state;
        
        if (!isActive) 
        {
            currentTimer = 0f;
            if (projectileModel != null) projectileModel.SetActive(false);
        }
        else 
        {
            if (projectileModel != null) projectileModel.SetActive(true);
        }
    }

    private void Update()
    {
        if (!isActive) return;

        currentTimer += Time.deltaTime;
        if (currentTimer >= spawnTime)
        {
            SpawnProjectile();
            SetSpawnerActive(false); 
        }
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab != null)
        {
            Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }
}