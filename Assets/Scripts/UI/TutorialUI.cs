using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveUpKeyText;
    [SerializeField] private TextMeshProUGUI moveDownKeyText;
    [SerializeField] private TextMeshProUGUI moveLeftKeyText;
    [SerializeField] private TextMeshProUGUI moveRightKeyText;
    [SerializeField] private TextMeshProUGUI interactKeyText;
    [SerializeField] private TextMeshProUGUI interactAltKeyText;
    [SerializeField] private TextMeshProUGUI pauseKeyText;
    [SerializeField] private TextMeshProUGUI interactGamePadKeyText;
    [SerializeField] private TextMeshProUGUI interactAltGamePadKeyText;
    [SerializeField] private TextMeshProUGUI pauseGamePadKeyText;


    private void Start()
    {
        GameInput.Instace.OnBindingRebind += GameInput_OnBindingRebind;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        UpdateVisual();
        Show();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountDownToStartActive()){
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        moveUpKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveUp);
        moveDownKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveRight);
        interactKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.Interact);
        interactAltKeyText.text = GameInput.Instace.GetBindingText(GameInput.Binding.InteractAlt);

        // ubraya gamepad kontrollerini de güncelleyen kodlar gelecekti þnedim yapamdým

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

