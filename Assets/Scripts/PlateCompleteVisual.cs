using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]  // SERIALIZABLE :  E�erki kendi tan�mlad���n tiplerin edit�re ekran�nda g�r�nmesini isteiyorsan serializable olarak i�aretlemen gerek.
    // A�a��daki tan�malma yerine string'ler KitchenObjectSO'ya kar��l�k gelen gameobject'i bulabilridik ama b�yle yaparak clean code yazd���n� s�yledi felan.
    public struct KitchenObjectSO_GameObject 
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject GameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        // Ba�lang��ta hepsini false yapmak i�in Yani g�r�nmemeleri i�in
        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList)
        {
                kitchenObjectSO_GameObject.GameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {


        //Debug.Log(e.kitchenObjectSO.prefab);
        //Debug.Log(transform.Find("Bread"));


        

        

        // B�t�n KitchenOBjectSO'larda dola� ve uygun e�le�mede o objeyi aktif et.
        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList) 
        {
            if (kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                //kitchenObjectSO_GameObject.kitchenObjectSO.prefab.gameObject.SetActive(true);
                kitchenObjectSO_GameObject.GameObject.SetActive(true);
            }
        }

    }
}
