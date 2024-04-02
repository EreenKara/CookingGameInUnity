using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;

    private Animator animator;
    private void Awake() // sanýrým animation arasýndaki geçiþler için kullanýlan özel bir method.
    {
        // Bu script'in ait olduðu objeye ait olan animator'u döndürüyor.
        animator = GetComponent<Animator>(); // Animator ile Animation'ý birbirlerine karþtýrma.
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
