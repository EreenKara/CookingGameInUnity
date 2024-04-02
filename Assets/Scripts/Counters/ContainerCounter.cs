using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player)
    {

        // Burada yeni bir kitchen object olu�turarak ona CounterTopPoint parentini ( Asl�nda transform atay�nca otomatik oalrak parent oalrakta atam�� olyurouz. ) 
        // Local position'u s�f�ra atamak �nemli ��nk� defualt oalrak 0'lanm�� bir e�kilde gelmyioralr.
        //Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, GetKitchenObjectFollowTransform()); // Noramlde b�yleydi ancak a�a��daki atama i�elminde zaten bunu yap�yoruz diye tansform'u sildik
        if(!player.HasKitchenObject())
        {

            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);

        }
    }



}
