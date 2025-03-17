using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public Transform cameraTransform; // Referencia a la transformaci�n de la c�mara

    void Update()
    {
        // Hacer que el arma siga la rotaci�n de la c�mara
        if (cameraTransform != null)
        {
            transform.rotation = cameraTransform.rotation;
        }
        else
        {
            Debug.LogError("No se ha asignado la c�mara en el script WeaponAim.");
        }
    }
}