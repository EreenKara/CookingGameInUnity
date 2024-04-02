using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            // There is no kitchen object here
            if (player.HasKitchenObject())
            {
                // Player holdin ktichen object
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player holding nothing
            }
        }
        else
        {
            // There is kitchenObjectSO object here
            if (player.HasKitchenObject())
            {
                // Player holding something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player Holding Plate

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {

                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    // Player holding something but plate
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Counter Holding Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // Player holding nothing
                GetKitchenObject().SetKitchenObjectParent(player);

            }


        }

    }

}
