using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipSO audioClipSO;
    private float volume = 1f;
    private void Awake()
    {
        Instance= this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlaced += BaseCounter_OnAnyObjectPlaced;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter=  (TrashCounter)sender;
        PlaySound(audioClipSO.trash, trashCounter.transform.position);

    }

    private void BaseCounter_OnAnyObjectPlaced(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter =  (BaseCounter)sender;
        PlaySound(audioClipSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, System.EventArgs e)
    {
        
        PlaySound(audioClipSO.objectPickUp, Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        // Bu event'i çaðýraný sender sayesinde alabiliyoruz böylece ne kadar uzaklýkta olduðnu bilebileceðiz.
        CuttingCounter cuttingCounter = sender as CuttingCounter;// (CuttingCounter)sender;
        PlaySound(audioClipSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        // Delivery manager tarafýndaki event'e atanmýþ bir fonksiyon.
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipSO.deliveryFail, deliveryCounter.transform.position);

    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * this.volume);

    }
    private void PlaySound(AudioClip[] audioCliparray,Vector3 position,float volumeMultiplier=1f)
    {
        PlaySound(audioCliparray[Random.Range(0, audioCliparray.Length)],position, volumeMultiplier * this.volume);
        
    }
    public void PlayFootStepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipSO.footstep,position,volume);

    }
    public void PlayCountDownSound()
    {
        PlaySound(audioClipSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipSO.warning, position);
    }
    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume>1f)
        {
            volume= 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME,volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }

}
