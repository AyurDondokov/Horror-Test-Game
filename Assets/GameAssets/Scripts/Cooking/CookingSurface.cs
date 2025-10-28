using UnityEngine;

public class CookingSurface : MonoBehaviour
{
    [Header("Настройки готовки")]
    public float cookTime = 5f; 
    public Transform spawnPoint; 
    public LayerMask cookableMask;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & cookableMask) == 0) return;

        CookableItem item = other.GetComponentInParent<CookableItem>();
        if (item != null && !item.IsCooking)
        {
            item.StartCooking(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CookableItem item = other.GetComponentInParent<CookableItem>();
        if (item != null && item.IsCooking)
        {
            item.StopCooking();
        }
    }
}
