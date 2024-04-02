using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            // PLAY CLICKED
            Loader.Load(Loader.Scene.GameScene);

        });
        quitButton.onClick.AddListener(() =>
        {
            // QUIT CLICKED
            // Application quit editorde ise yaramiyor ise yaramsi icin build almak gerekiyor.
            Application.Quit();
        });

        Time.timeScale= 1.0f;
    }
}
