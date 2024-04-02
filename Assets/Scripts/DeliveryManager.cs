using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeSOList recipeSOList;
    private List<RecipeSO> waitingRecipeSOList;

    private int successfulDeliveredAmount;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;

    private void Awake()
    {
        waitingRecipeSOList= new List<RecipeSO>();

        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        spawnRecipeTimer-= Time.deltaTime;
        if(GameManager.Instance.IsGamePlaying() && spawnRecipeTimer <= 0f )
        {
            spawnRecipeTimer= spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSo = recipeSOList.recipeSOListesi[UnityEngine.Random.Range(0, recipeSOList.recipeSOListesi.Count)];
                waitingRecipeSOList.Add(waitingRecipeSo);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }

        }
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];


            // foreach gibi uzun sürecek döngüden önce optimizasyon için küçük bir yol.
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentMatch = true;
                // Has the same number of ingredients.
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // cycling all waiting list
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // cycling  all plate lsit 
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            // There is match
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        // There is no match
                        plateContentMatch = false;
                    }
                }
                if(plateContentMatch)
                {
                    // player delivered correct recipe
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    successfulDeliveredAmount++;
                    return;
                }

            }
        }

        // No matches found;
        // Player delivered wrong  recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);

    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }
    public int GetSuccessfulDeliveredAmount()
    {
        return successfulDeliveredAmount;   
    }

}

