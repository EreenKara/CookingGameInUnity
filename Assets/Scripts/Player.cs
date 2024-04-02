using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent

{
    // Singleton deseni i�in set'i private yapt�k.
    public static Player Instance { get; private set; }


    public event EventHandler OnPickedSomething;
    // herhangi bir selected coutner  de�iikli�inde sleected coutner'lar�n kendini update edeiblmesi i�in.
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }



    // Unity UI taraf�ndan okunmas�n� sagliyar SerializeField Alan�
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;



    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        // SINGLETON ICIN YAPTIK.
        if(Instance!=null)
        {
            Debug.LogError("There is more than one player.");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlteranteAction += GameInput_OnInteractAlteranteAction;
    }

    private void GameInput_OnInteractAlteranteAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }

    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }


    /// <summary>
    /// Selected Nesneyi degistirmeye yariyor. duzgunune erisebilirsek o nesne hakkinda islemler yapbilirzi.
    /// </summary>
    private void HandleInteractions() // fonksiyon �uan sadece etkile�imde bulunaca��m�z nesneye do�ru bakt���mz�da i�e yar�yor.
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir= moveDir; // bunu yapmam�z�n sebebi hareket etmeyi b�rakt���m�zda bile etkile�imde kalabilmek i�in.
        }

        float interactDistance = 2f;

        //Raycaste mask uygulad�k b�ylece �n�ne COUNTER olmayan baska bir obje ciksa bile coutner'lara Raycast uygulayacaginda istediimiz sonucu alip etkiseleme girebilecegiz.
        // layermask'i unity tarafinda layer olusturduk ilk �nce ondan sorna coutner'lara bu layer'� ekledik. Surukle b�rakla coutner'lara serialize object field'�na ekledik.
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) // Burada method out sayesinde bize raycastHit'in i�ini doldurup geri d�nd�recek.
        {
            // �n�m�zde obje var m� yok mu anlad�ktan sonra bunun ne oldu�unu bulamm�z gerek.
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // HAS CLEAR COUNTER 
                if (baseCounter != selectedCounter)
                {
                    this.SetSelectedCounter(baseCounter);

                }
                //ClearCounter clearCounter = raycastHit.transform.GetComponent<ClearCounter>(); // TryGetComponent ile bu ayn� i�i yap�yor
                //if (clearCounter != null)                                                      //Ancak try olan null check'i bizim i�in kendi handle ediyor. 
                //{
                //    // HAS CLEAR COUNTER 
                //}
            }
            else
            {
                // OYUNCUNUN ONUNDE OBJE VARSA COUNTER DEG�SLE

                this.SetSelectedCounter(null);
            }

        }
        else 
        {
            // EGERK� OYUNCUNUN ONUNDE HERHANG� BIR OBJE YOKSA

            this.SetSelectedCounter(null);
        }
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();


        // transform nesnesi script'in bagli oldugu objeyi temsil eder.
        // transform.position = (Vector3)inputVector;    // Bu sekilde atama yaparsak X ve Y  konumlar�nda hareket edecek.

        // ilk �nce keyboard'dan okuyup ondan sonra hareket ettirme islemini yapmak
        // clean kod yazmak acisindan daha kullanisli ve refactor islemlerinde bize yardimci olacak
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // E�erki yolda bir engel varsa �arp��may� s�namak i�in burada pyhisic k�t�phanesinin bir fonksiyonun kullan�caz.
        // Raycast() methodu player'�n merkezinden bir ���n yollarak �n�nde bir engel olup olmad���n� test ediyor. Bu �ok kullan��l� de�il.
        // ��nk� karakterin as�l fiziksel �eklini yanst�ma�yr.
        float moveDistance = moveSpeed * Time.deltaTime; // ne kadar yol ald�g�n� bulduk h�z ile zaman� �arparak.
        float playerRadius = 0.7f;  // Karakterin size'�, ancak Capsule kulland���m�zdan radius olarak d���nd�k.
        float playerHeight = 2f;
        //bool canMove = !Physics.Raycast(transform.position, moveDir, playerSize); // Bu fonksiyon �n�nde bir engel varsa true d�nd�r�yor yani �n�nde engel yoksa y�r�bebilirz dememiz gerekyior.

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);


        // Onunde engel olsa dahi bir taraf� a��ksa diagonal olarak y�r�mesine izin vermek icin.
        if (!canMove)
        {
            // Cannot move toaard moveDir

            // Attempt only X 
            Vector3 moveDirX = new Vector3(inputVector.x, 0, 0).normalized; // Normalized yapmam�z�n sebebi duvara kar�� ayn� anda iki tu�a bakasark
                                                                            // ancak sadece bir y�nde y�r�rken h�z�m�z�n sadece tek tu�a basarkenki ile ayn� kalmas� i�in.
            canMove = (moveDir.x<-0.5f || moveDir.x > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                // Can move only X 
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move towards X 

                // Attempt only Z 
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                // bu 0.5f ten buyuk olma ve kuck olma muhabbeti gamepad'de duzgun veya daha rahat hissettirmesi i�in.
                canMove = (moveDir.z < -0.5f || moveDir.z > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else
                {
                    // cannot move in any direction

                }
            }
        }



        if (canMove)
        {
            transform.position += moveDir * moveDistance; // Time.deltaTime field'� herbir frame aras�nda ge�en s�reyi bize veriyor
                                                          // b�ylece daha �ok frame alan oyuncu update'i daha �ok �a��rabildi�inden daha h�zl� hareket etmeyecek
                                                          // transform.forward = moveDir;  // B�yle yaparsak �ok h�zl� d�n�yor. O y�zden Slerp() ekliyoruz.
        }
        isWalking = moveDir != Vector3.zero; // b�t�n degerlere s�f�r vermenin kolay bir yolu.


        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedBaseCounter)
    {
        this.selectedCounter= selectedBaseCounter;
        this.OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter=selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return this.kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject!=null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject!= null;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }
}
