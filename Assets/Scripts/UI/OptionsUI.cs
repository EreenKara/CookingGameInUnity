using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;


    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;


    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onClosedButtonAction;

    private void Awake()
    {
        Instance= this;
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onClosedButtonAction();
        });
        moveUpButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveUp));
        moveDownButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveDown));
        moveLeftButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveLeft));
        moveRightButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveRight));
        interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
        interactAltButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlt));

    }
    private void Start()
    {
        GameManager.Instance.OnGameContinue += GameManager_OnGameContinue;
        UpdateVisual();
        HidePressToRebindKey();
        Hide();
    }

    private void GameManager_OnGameContinue(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instace.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instace.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instace.GetBindingText(GameInput.Binding.InteractAlt);

    }
    public void Show(Action onClosedButtonAction)
    {
        this.onClosedButtonAction = onClosedButtonAction;
        gameObject.SetActive(true);
        soundEffectsButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);

    }
    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }
    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instace.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }


}
