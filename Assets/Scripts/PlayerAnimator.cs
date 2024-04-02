using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;

    private Animator animator;
    private void Awake() // san�r�m animation aras�ndaki ge�i�ler i�in kullan�lan �zel bir method.
    {
        // Bu script'in ait oldu�u objeye ait olan animator'u d�nd�r�yor.
        animator = GetComponent<Animator>(); // Animator ile Animation'� birbirlerine kar�t�rma.
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
