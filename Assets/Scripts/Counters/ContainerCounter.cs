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

        // Burada yeni bir kitchen object oluþturarak ona CounterTopPoint parentini ( Aslýnda transform atayýnca otomatik oalrak parent oalrakta atamýþ olyurouz. ) 
        // Local position'u sýfýra atamak önemli çünkü defualt oalrak 0'lanmýþ bir eþkilde gelmyioralr.
        //Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, GetKitchenObjectFollowTransform()); // Noramlde böyleydi ancak aþaðýdaki atama iþelminde zaten bunu yapýyoruz diye tansform'u sildik
        if(!player.HasKitchenObject())
        {

            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);

        }
    }



}
