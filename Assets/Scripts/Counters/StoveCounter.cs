using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle, // Tava Bosta
        Frying, // Kýzarýyor 
        Fried, // Kýzardý ve kullanýcý ensneyi eline almadý 
        Burned // Yandi ve tava durdu
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State currenState;
    private FryingRecipeSO fryingRecipeSO;
    private float fryingTimer; // Kodu daha clean bir þekilde oluþturabilmek için iki tane ayrý time oluþturduk 
    private BurningRecipeSO burningRecipeSO;
    private float burningTimer;

    private void Start()
    {
        currenState = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (currenState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimeMax
                    });
                    if (fryingTimer > fryingRecipeSO.fryingTimeMax)
                    {
                        // Fried 
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(this.fryingRecipeSO.output, this);
                        currenState = State.Fried;
                        burningTimer= 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currenState
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimeMax
                    }) ;
                    if (burningTimer > burningRecipeSO.burningTimeMax)
                    {
                        // Fried 
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(this.burningRecipeSO.output, this);
                        currenState = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currenState
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }

    }

    // ZAMANA BAGLI ISLEMLER ICIN KULLANALILABILECEK BIR YONTEM ANCAK BIZ BUNU KULLANMADIK

    //private void Start()
    //{
    //    StartCoroutine(HandleFryTimer());
    //}
    //private IEnumerator HandleFryTimer()
    //{
    //    // Yield sayesinde herbir fonksiyon çaðrýmýnda o anki yield neyse o dönecek. Bir andabütün bir dizi oluþturmak yerine Inumerator ile oaraçlara gölerek diziler oluþturmuþ olyurouz.
    //    yield return new WaitForSeconds(1f);  
    //}

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchen object here
            if (player.HasKitchenObject())
            {
                // Player holdin ktichen boject
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {

                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    this.fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    this.currenState = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = currenState
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimeMax
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

                        currenState = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = currenState
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }

            }
            else
            {
                // Player holding nothing
                GetKitchenObject().SetKitchenObjectParent(player);
                currenState = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = currenState
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }


        }


    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == kitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == kitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
    public bool IsFried()
    {
        return currenState == State.Fried;
    }

}
