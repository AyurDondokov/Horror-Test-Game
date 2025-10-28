using UnityEngine;

public class NPCAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NPCController controller;
    private void Update()
    {
        animator.SetBool("isWalk", (controller.GetState() == NPCController.State.Moving));
    }
}
