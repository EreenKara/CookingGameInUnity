using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
    // Asagidaki new kelimesinin kullanilma amaci BaseCounter sinfiinda
    // yer alan ayni isimli fonlsyion yerine bu fonsiyonun kullanilacaginin farkinda oldugumuzu declare etmek icin
    new public static  void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }
}
