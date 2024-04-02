using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    // Sahaneler aras� ge�i�te bu s�n�f static oldu�undan ve herahngi bir isntance'� olamd���ndan yok edilmeyecek.
    // Bu y�zden bu asagidaki field sikinti yaratma potansiyeline sahip.
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        // Kulllanici tanimli yapilari kabul etmediginden tostring dedik sadece string ve integer degeleri kabul ediyor.
        Loader.targetScene= targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    public static void LoaderCallBack()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
