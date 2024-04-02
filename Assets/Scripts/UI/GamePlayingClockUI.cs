using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Start()
    {
        timerImage.fillAmount = 0;
    }
    private void Update()
    {
        timerImage.fillAmount =GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
