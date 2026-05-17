using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAfterImage : MonoBehaviour
{
    [Header("Afterimage Settings")]
    public float timeBetweenSpawns = 0.05f;
    public float afterImageLifetime = 0.5f;
    public Material afterImageMaterial;

    [Header("AfterImage Movement (Drift)")]
    public Vector3 driftDirection = Vector3.zero;
    public float driftSpeed = 0f;

    [Header("Optimization (Pooling)")]
    public int poolSize = 10;

    private SkinnedMeshRenderer[] skinnedRenderers;
    private MeshRenderer[] staticRenderers;
    private Coroutine spawnCoroutine;

    private List<AfterImageBehavior> afterImagePool = new List<AfterImageBehavior>();

    private void Start()
    {
        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        staticRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < poolSize; i++)
        {
            ExpandPool();
        }

        StartSpawningAfterImages();
    }

    private AfterImageBehavior ExpandPool()
    {
        GameObject afterImageObj = new GameObject("boss_afterImage");
        AfterImageBehavior behavior = afterImageObj.AddComponent<AfterImageBehavior>();
        behavior.Initialize(skinnedRenderers, staticRenderers, afterImageMaterial);
        
        afterImageObj.SetActive(false);
        afterImagePool.Add(behavior);
        return behavior;
    }

    public void StartSpawningAfterImages()
    {
        if (spawnCoroutine == null) spawnCoroutine = StartCoroutine(SpawnAfterImagesRoutine());
    }

    public void StopSpawningAfterImages()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnAfterImagesRoutine()
    {
        while (true)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void CreateAfterImage()
    {
        AfterImageBehavior afterImage = GetAvailableAfterImage();
        Vector3 normalizedDir = driftDirection.normalized;
        afterImage.Activate(transform, afterImageLifetime, normalizedDir, driftSpeed);
    }

    private AfterImageBehavior GetAvailableAfterImage()
    {
        foreach (AfterImageBehavior afterImage in afterImagePool)
        {
            if (!afterImage.gameObject.activeInHierarchy) return afterImage;
        }
        return ExpandPool();
    }
}

public class AfterImageBehavior : MonoBehaviour
{
    private List<SkinnedMeshRenderer> targetSkinned = new List<SkinnedMeshRenderer>();
    private List<Transform> skinnedTransforms = new List<Transform>();
    private List<MeshFilter> skinnedFilters = new List<MeshFilter>();

    private List<MeshRenderer> targetStatic = new List<MeshRenderer>();
    private List<Transform> staticTransforms = new List<Transform>();

    private List<Material> allMaterials = new List<Material>();

    public void Initialize(SkinnedMeshRenderer[] origSkinned, MeshRenderer[] origStatic, Material afterImageMat)
    {
        foreach (SkinnedMeshRenderer smr in origSkinned)
        {
            string objName = smr.gameObject.name.ToLower();
            if (objName.Contains("shadow") || objName.Contains("quad") || objName.Contains("plane")) continue;

            targetSkinned.Add(smr);

            GameObject part = new GameObject(smr.gameObject.name + "_Part");
            part.transform.SetParent(this.transform);
            skinnedTransforms.Add(part.transform);

            MeshFilter filter = part.AddComponent<MeshFilter>();
            filter.mesh = new Mesh();
            skinnedFilters.Add(filter);

            MeshRenderer renderer = part.AddComponent<MeshRenderer>();
            Material thiefMat = CreateThiefMaterial(afterImageMat, smr.material);
            renderer.material = thiefMat;
            allMaterials.Add(thiefMat);
        }

        foreach (MeshRenderer mr in origStatic)
        {
            string objName = mr.gameObject.name.ToLower();
            if (objName.Contains("shadow") || objName.Contains("quad") || objName.Contains("plane")) continue;

            targetStatic.Add(mr);

            MeshFilter origFilter = mr.GetComponent<MeshFilter>();

            GameObject part = new GameObject(mr.gameObject.name + "_Part");
            part.transform.SetParent(this.transform);
            staticTransforms.Add(part.transform);

            MeshFilter filter = part.AddComponent<MeshFilter>();
            if (origFilter != null) filter.mesh = origFilter.sharedMesh;

            MeshRenderer renderer = part.AddComponent<MeshRenderer>();
            Material thiefMat = CreateThiefMaterial(afterImageMat, mr.material);
            renderer.material = thiefMat;
            allMaterials.Add(thiefMat);
        }
    }

    private Material CreateThiefMaterial(Material afterImageMat, Material originalMaterial)
    {
        Material matInstance = new Material(afterImageMat);

        if (originalMaterial.mainTexture != null)
            matInstance.mainTexture = originalMaterial.mainTexture;
        else if (originalMaterial.HasProperty("_BaseMap"))
            matInstance.SetTexture("_BaseMap", originalMaterial.GetTexture("_BaseMap"));

        if (originalMaterial.HasProperty("_Color"))
        {
            Color origColor = originalMaterial.color;
            origColor.a = afterImageMat.color.a;
            matInstance.color = origColor;
        }
        else if (originalMaterial.HasProperty("_BaseColor"))
        {
            Color origColor = originalMaterial.GetColor("_BaseColor");
            origColor.a = afterImageMat.color.a;
            matInstance.SetColor("_BaseColor", origColor);
        }

        return matInstance;
    }

    public void Activate(Transform bossRoot, float duration, Vector3 dir, float speed)
    {
        transform.position = bossRoot.position;
        transform.rotation = bossRoot.rotation;

        for (int i = 0; i < targetSkinned.Count; i++)
        {
            skinnedTransforms[i].position = targetSkinned[i].transform.position;
            skinnedTransforms[i].rotation = targetSkinned[i].transform.rotation;
            skinnedTransforms[i].localScale = targetSkinned[i].transform.localScale;

            targetSkinned[i].BakeMesh(skinnedFilters[i].mesh);
        }

        for (int i = 0; i < targetStatic.Count; i++)
        {
            staticTransforms[i].position = targetStatic[i].transform.position;
            staticTransforms[i].rotation = targetStatic[i].transform.rotation;
            staticTransforms[i].localScale = targetStatic[i].transform.localScale;
        }

        gameObject.SetActive(true);
        StartCoroutine(FadeAndMoveRoutine(duration, dir, speed));
    }

    private IEnumerator FadeAndMoveRoutine(float duration, Vector3 dir, float speed)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.position += dir * speed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            foreach (Material mat in allMaterials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
                else if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = alpha;
                    mat.SetColor("_BaseColor", c);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false); 
    }
}