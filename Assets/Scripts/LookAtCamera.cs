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
    [SerializeField] private Mode mode; // Editor arac�l�g� ile yuakr�daki iki se�enekten birini se�ebilrisin.

    private void LateUpdate()
    {
        // Canvas normalde arkas�ndan bak�yormu�sun gibi �al���yor bu y�zden direkt LooakAt dersek canvas�n arkas�ndan bakma g�r�nt�s� elde edece�iz. 
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                // Inverted sayesinde �nden bak�yormu�cas�na bir g�r�nt� elde edebiliriz.
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
