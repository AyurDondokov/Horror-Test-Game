using UnityEngine;

public class CookableItem : MonoBehaviour
{
    [Header("—сылки")]
    [SerializeField] private GameObject cookedPrefab;
    [SerializeField] private ParticleSystem cookingEffect;
    [SerializeField] private AudioClip cookingSound;
    public bool IsCooking { get; private set; }

    private CookingSurface currentSurface;
    private float cookTimer;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartCooking(CookingSurface surface)
    {
        currentSurface = surface;
        IsCooking = true;
        cookTimer = 0f;

        if (cookingEffect != null)
            cookingEffect.Play();
        if (cookingSound != null)
            AudioManager.Instance.PlaySoundAt(cookingSound, transform.position, 0.1f);
    }

    public void StopCooking()
    {
        IsCooking = false;
        currentSurface = null;

        if (cookingEffect != null)
            cookingEffect.Stop();
    }

    private void Update()
    {
        if (!IsCooking || currentSurface == null)
            return;

        cookTimer += Time.deltaTime;

        if (cookTimer >= currentSurface.cookTime)
        {
            FinishCooking();
        }
    }

    private void FinishCooking()
    {
        IsCooking = false;

        if (cookingEffect != null)
            cookingEffect.Stop();

        Vector3 spawnPos = currentSurface.spawnPoint != null
            ? currentSurface.spawnPoint.position
            : transform.position;

        Quaternion spawnRot = transform.rotation;

        GameObject cookedItem = Instantiate(cookedPrefab, spawnPos, spawnRot);
        Rigidbody newRb = cookedItem.GetComponent<Rigidbody>();
        if (newRb != null)
        {
            newRb.linearVelocity = rb.linearVelocity;
        }

        Destroy(gameObject);
    }
}
