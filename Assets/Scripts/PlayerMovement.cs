using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f; 
    public float runSpeed = 20f; 
    public float gravity = -9.81f; // Gravedad base
    public CharacterController controller; // Referencia al CharacterController
    public Transform groundCheck; // Verificación del suelo
    public float groundDistance = 0.25f; // Distancia al suelo
    public float jumpHeight = 4f; // Altura del salto
    public LayerMask groundMask; // Detección del suelo
    private Vector3 velocity; // Velocidad del jugador
    private Vector3 moveDirection; // Dirección de movimiento
    private bool isGrounded; // Estado del suelo

    public Animator animator;
    public float airFriction = 0.98f; // Fricción en el aire cuando sueltas la tecla (más bajo = pierde velocidad más rápido)
    public float airControlLerp = 0.1f; // Control en el aire (más alto = más responsivo)
                                        // NUEVO: bandera para desactivar movimiento
    public bool canMove = true;
    
    void Update()
    {

        if (!canMove)
        {
            // si está bloqueado, cancelar inputs y gravedad
            moveDirection = Vector3.zero;
            velocity = Vector3.zero;
            animator.SetFloat("moveSpeed", 0);
            return; // salimos del Update
        }

        // Verificar si el jugador está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) || controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reinicia la velocidad de caída
        }

        // Obtener la entrada del teclado
        float moveX = Input.GetAxisRaw("Horizontal"); // Movimiento lateral
        float moveZ = Input.GetAxisRaw("Vertical");   // Movimiento hacia adelante

        // Ajustar la velocidad al correr
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        if (isGrounded)
        {
            // Si está en el suelo, actualizar la dirección de movimiento solo cuando hay entrada
            if (moveX != 0 || moveZ != 0)
            {
                moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * currentSpeed;
            }
            else
            {
                moveDirection = Vector3.zero; // Se detiene inmediatamente en el suelo
            }
        }
        else
        {
            // En el aire, si hay input, modificar la dirección, pero sin perder la inercia
            Vector3 airControl = (transform.right * moveX + transform.forward * moveZ).normalized * currentSpeed;

            if (moveX != 0 || moveZ != 0)
            {
                // Si el jugador sigue presionando la tecla, responde rápido
                moveDirection = Vector3.Lerp(moveDirection, airControl, airControlLerp);
            }
            else
            {
                // Si el jugador suelta la tecla en el aire, pierde inercia gradualmente
                moveDirection *= airFriction;
            }
        }

        // Aplicar el movimiento
        controller.Move(moveDirection * Time.deltaTime);

        // Saltar solo si está en el suelo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimiento vertical (gravedad y salto)
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }
}
