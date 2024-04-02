using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter,IHasProgress
{
    // Static eventler sahneler arasi geciste yok olmazlar.
    // Eðerki static event  kllancaksan manuel oalrak bu event'in resetlenmesinden sorumlu odlguunu untuam.
    public static event EventHandler OnAnyCut;

    // Asagidaki new kelimesinin kullanilma amaci BaseCounter sinfiinda
    // yer alan ayni isimli fonlsyion yerine bu fonsiyonun kullanilacaginin farkinda oldugumuzu declare etmek icin
    new public static void ResetStaticData()
    {
        OnAnyCut= null;
    }

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    
    public event EventHandler OnCut;

    private int cuttingProgress;

    public override void Interact(Player player)
    {

        if (!HasKitchenObject())
        {
            // There is no kitchen object here
            if (player.HasKitchenObject())
            {
                // Player holdin ktichen boject
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0; // Her yeni obje konulduðunda cutting prgoress i sýfýrlyýrouz.
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs  // Progress bar'ýn abone olduðu event yani cutting progress ne akdar ilerledi. Burada çaýrýyruz.
                    {
                        progressNormalized = cuttingProgress/ (float)cuttingRecipeSO.cuttingProgressMax
                    });
                }

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

            }
            else
            {
                // Player holding nothing
                GetKitchenObject().SetKitchenObjectParent(player);

            }


        }


    }
    public override void InteractAlternate(Player player)
    {

        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // There is object on cutting counter and it can be cut.
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs  // Progress bar'ýn abone olduðu event yani cutting progress ne akdar ilerledi. Burada çaýrýyruz.
            {
                progressNormalized = cuttingProgress / (float)cuttingRecipeSO.cuttingProgressMax
            });
            if ( cuttingProgress>=cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();
                //Transform kitchenObjectTransform = Instantiate(cutKitchenObjectSO.prefab);  // BURDA KOD TEKRARI OLMASIN DIYE BUNU KITCHEN OBJECT ICERISINE KOYACAGIZ
                //kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

                // Ustteki kod bir kaç yerde tekrarlandýðý için bunun sorumluluðnuu kitchen object sýnýfýna veridk.
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO= GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO!= null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO= GetCuttingRecipeSOWithInput(kitchenObjectSO);
        if (cuttingRecipeSO!=null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

}
