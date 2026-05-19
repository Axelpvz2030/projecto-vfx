using UnityEngine;
using System.Collections.Generic;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public bool isActive = true; 
    public float spawnTime = 1f;
    public GameObject projectileModel;
    public GameObject projectilePrefab;

    [Header("Pool Settings")]
    public int poolSize = 10;
    private List<GameObject> projectilePool;

    private float currentTimer = 0f;

    private void Start()
    {
        if (projectileModel != null) projectileModel.SetActive(isActive);

        projectilePool = new List<GameObject>();
        
        if (projectilePrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(projectilePrefab);
                obj.SetActive(false);
                projectilePool.Add(obj);
            }
        }
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
        foreach (GameObject projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
            {
                projectile.transform.position = transform.position;
                projectile.transform.rotation = transform.rotation;
                projectile.SetActive(true);
                
                ProjectileMovement movement = projectile.GetComponent<ProjectileMovement>();
                if (movement != null)
                {
                    movement.Shoot();
                }
                
                return; 
            }
        }
    }
}