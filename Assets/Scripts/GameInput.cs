using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    #region Save System

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
   
    #endregion





    // Static prop'lar sahneler arasý geçiþte yok olurlar ancak static event'ler yok olmazlar.
    // ChatGPT bunun tam tersini söylüyor ve bu static prop'un içerisindeki deðer kalacak diyor.
    // Ama ben ce yok olabilir cünkü sonucta atadýgýmýz instance yok olmus olacak. ve tekrar'dan atama yapacagýz.
    public static GameInput Instace { get; private set; }

    public event EventHandler OnInteractAction; // Baska sýnýflarýn direkt olarak asýl event'e deðil de bizim belirlediðimiz baska bir event üzerinden
                                                // istediklerini tanýmlamalarý için örneðin Player sýnýfý buna bir þey attach edecek bizde bunu kendi
                                                // event'imiz içinde çaðýracaðýz.
    public event EventHandler OnInteractAlteranteAction; 
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    private PlayerInputActions playerInputActions;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlt,
    }


    private void Awake()
    {
        Instace = this;
        playerInputActions = new PlayerInputActions();

        // Egerki kullancii herhangi bir kaydedilmis ayara sahipse nu yukelemk icin bunu yaptik
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        }

        playerInputActions.Player.Enable(); // Burada istediðin bir mapping'i aktifleþtirmelisin default oalrak aktif gelmyiyor.

        playerInputActions.Player.Interact.performed += Interact_performed; // burada bu methodu bu event'e subcribe ettik.
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;


        

    }
    private void OnDestroy()
    {

        // 
        playerInputActions.Player.Interact.performed -= Interact_performed; // burada bu methodu bu event'e subcribe ettik.
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();

    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlteranteAction?.Invoke(this, EventArgs.Empty);

    }

    public void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) // E'ye basýldýðýnda playerInputACtions'lara abone ettiðimiz için bu method çaðrýlacak.
    {
        //if (OnInteractAction!=null)       // BUNU böyle yapmak ile aþaðýdaki soru iþareti ile yapmak arasýnda hiçbir fark yok
        //{
        //    OnInteractAction(this, EventArgs.Empty);
        //}

        OnInteractAction?.Invoke(this, EventArgs.Empty); // Yukarýdakinin aynýsý.
    }
    public Vector2 GetMovementVectorNormalized()
    {


        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        /* OLD STYLE */
        //Vector2 inputVector = new Vector2(0, 0);

        //if (Input.GetKey(KeyCode.W))
        //{
        //    inputVector.y += 1;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    inputVector.y += -1;

        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    inputVector.x += -1;

        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    inputVector.x += 1;

        //}
        inputVector = inputVector.normalized; // diagonal yurummede hiz duzgun olsun diye nromalize ettik.
                                              // sqrt(x^2+y^2) gibi dsuun hipotenus aldýk 
        return inputVector;
    }
    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.MoveUp:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.InteractAlt:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            default:
                return null;
        }
    }
    public void RebindBinding(Binding binding,Action onActionRebind)
    {

        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlt:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
        }


        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => // Bu callback nesnesi binding'lenmis key ve rebindinglenecek key hakkinda bir cok bilgiler iceriyor
            {
                // biz bu nesnenin action prop'unu kullaniyoruz.


                // Eski surumlerinde elle dispose ediliyormus onlem olmasi acisindan yine elle yaptik
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebind();
                // Egerki kullancii herhangi bir kaydedilmis ayara sahipse nu kaydetmek icin bunu yaptik
                // playerprefs bunu kaydetmek icin gerekli iken. Playerinput bizim icin otomatik json olusturuyor.
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS,playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                OnBindingRebind?.Invoke(this, EventArgs.Empty);

            }).Start();
    }
}
