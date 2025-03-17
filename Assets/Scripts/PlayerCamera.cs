using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilidad del mouse
    public Transform playerBody; // Referencia al cuerpo del jugador

    private float xRotation = 0f;

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla y hacerlo invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtener la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotar la cámara en el eje Y (arriba/abajo)
        xRotation -= mouseY;    
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación vertical

        // Aplicar la rotación a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotar el cuerpo del jugador en el eje X (izquierda/derecha)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}