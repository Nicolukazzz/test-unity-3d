using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public Transform cameraTransform; // Referencia a la transformación de la cámara

    void Update()
    {
        // Hacer que el arma siga la rotación de la cámara
        if (cameraTransform != null)
        {
            transform.rotation = cameraTransform.rotation;
        }
        else
        {
            Debug.LogError("No se ha asignado la cámara en el script WeaponAim.");
        }
    }
}