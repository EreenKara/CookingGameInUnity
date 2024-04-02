using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent

{
    // Singleton deseni için set'i private yaptýk.
    public static Player Instance { get; private set; }


    public event EventHandler OnPickedSomething;
    // herhangi bir selected coutner  deðiikliðinde sleected coutner'larýn kendini update edeiblmesi için.
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }



    // Unity UI tarafýndan okunmasýný sagliyar SerializeField Alaný
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
    private void HandleInteractions() // fonksiyon þuan sadece etkileþimde bulunacaðýmýz nesneye doðru baktýðýmzýda iþe yarýyor.
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir= moveDir; // bunu yapmamýzýn sebebi hareket etmeyi býraktýðýmýzda bile etkileþimde kalabilmek için.
        }

        float interactDistance = 2f;

        //Raycaste mask uyguladýk böylece önüne COUNTER olmayan baska bir obje ciksa bile coutner'lara Raycast uygulayacaginda istediimiz sonucu alip etkiseleme girebilecegiz.
        // layermask'i unity tarafinda layer olusturduk ilk önce ondan sorna coutner'lara bu layer'ý ekledik. Surukle býrakla coutner'lara serialize object field'ýna ekledik.
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) // Burada method out sayesinde bize raycastHit'in içini doldurup geri döndürecek.
        {
            // Önümüzde obje var mý yok mu anladýktan sonra bunun ne olduðunu bulammýz gerek.
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // HAS CLEAR COUNTER 
                if (baseCounter != selectedCounter)
                {
                    this.SetSelectedCounter(baseCounter);

                }
                //ClearCounter clearCounter = raycastHit.transform.GetComponent<ClearCounter>(); // TryGetComponent ile bu ayný iþi yapýyor
                //if (clearCounter != null)                                                      //Ancak try olan null check'i bizim için kendi handle ediyor. 
                //{
                //    // HAS CLEAR COUNTER 
                //}
            }
            else
            {
                // OYUNCUNUN ONUNDE OBJE VARSA COUNTER DEGÝSLE

                this.SetSelectedCounter(null);
            }

        }
        else 
        {
            // EGERKÝ OYUNCUNUN ONUNDE HERHANGÝ BIR OBJE YOKSA

            this.SetSelectedCounter(null);
        }
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();


        // transform nesnesi script'in bagli oldugu objeyi temsil eder.
        // transform.position = (Vector3)inputVector;    // Bu sekilde atama yaparsak X ve Y  konumlarýnda hareket edecek.

        // ilk önce keyboard'dan okuyup ondan sonra hareket ettirme islemini yapmak
        // clean kod yazmak acisindan daha kullanisli ve refactor islemlerinde bize yardimci olacak
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // Eðerki yolda bir engel varsa çarpýþmayý sýnamak için burada pyhisic kütüphanesinin bir fonksiyonun kullanýcaz.
        // Raycast() methodu player'ýn merkezinden bir ýþýn yollarak önünde bir engel olup olmadýðýný test ediyor. Bu çok kullanýþlý deðil.
        // Çünkü karakterin asýl fiziksel þeklini yanstýmaýyr.
        float moveDistance = moveSpeed * Time.deltaTime; // ne kadar yol aldýgýný bulduk hýz ile zamaný çarparak.
        float playerRadius = 0.7f;  // Karakterin size'ý, ancak Capsule kullandýðýmýzdan radius olarak düþündük.
        float playerHeight = 2f;
        //bool canMove = !Physics.Raycast(transform.position, moveDir, playerSize); // Bu fonksiyon önünde bir engel varsa true döndürüyor yani önünde engel yoksa yürübebilirz dememiz gerekyior.

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);


        // Onunde engel olsa dahi bir tarafý açýksa diagonal olarak yürümesine izin vermek icin.
        if (!canMove)
        {
            // Cannot move toaard moveDir

            // Attempt only X 
            Vector3 moveDirX = new Vector3(inputVector.x, 0, 0).normalized; // Normalized yapmamýzýn sebebi duvara karþý ayný anda iki tuþa bakasark
                                                                            // ancak sadece bir yönde yürürken hýzýmýzýn sadece tek tuþa basarkenki ile ayný kalmasý için.
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
                // bu 0.5f ten buyuk olma ve kuck olma muhabbeti gamepad'de duzgun veya daha rahat hissettirmesi için.
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
            transform.position += moveDir * moveDistance; // Time.deltaTime field'ý herbir frame arasýnda geçen süreyi bize veriyor
                                                          // böylece daha çok frame alan oyuncu update'i daha çok çaðýrabildiðinden daha hýzlý hareket etmeyecek
                                                          // transform.forward = moveDir;  // Böyle yaparsak çok hýzlý dönüyor. O yüzden Slerp() ekliyoruz.
        }
        isWalking = moveDir != Vector3.zero; // bütün degerlere sýfýr vermenin kolay bir yolu.


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
