using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    public void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged; // Bu kodun start içersiinde yazýlacak oluyor olmasý önemli çünkü Playerinstance'ýný awake'te verdik.
                                                                                     // Awake'te yapamayýz bua tamayý.
        

    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if(e.selectedCounter==baseCounter)
        {
            this.Show();
        }
        else
        {
            this.Hide();
        }
    }
    private void Show()
    {
        visualGameObjectArray.ToList().ForEach((obj) =>
        {
            obj.SetActive(true);
        });
    }
    private void Hide()
    {
        visualGameObjectArray.ToList().ForEach((obj) =>
        {
            obj.SetActive(false);
        });
    }



}
