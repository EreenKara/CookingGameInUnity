using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }
    [SerializeField] private Mode mode; // Editor aracýlýgý ile yuakrýdaki iki seçenekten birini seçebilrisin.

    private void LateUpdate()
    {
        // Canvas normalde arkasýndan bakýyormuþsun gibi çalýþýyor bu yüzden direkt LooakAt dersek canvasýn arkasýndan bakma görüntüsü elde edeceðiz. 
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                // Inverted sayesinde önden bakýyormuþcasýna bir görüntü elde edebiliriz.
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position+ dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward= Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }

    }
}
