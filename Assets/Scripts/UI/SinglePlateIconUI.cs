using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlateIconUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetSpriteUsingKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }

}
