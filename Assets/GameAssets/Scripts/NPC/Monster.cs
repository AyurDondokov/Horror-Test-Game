using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float needDistance;

    private void Update()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= needDistance)
            {
                player.Lose(this);
            }
        }
    }
}
