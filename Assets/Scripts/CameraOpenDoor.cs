using DoorScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CameraDoorScript
{
    public class CameraOpenDoor : MonoBehaviour
    {
        public float DistanceOpen = 3;
        public GameObject text;
        public GameObject text2;
        public PlayerMovement playerMovement;
        public PlayerCamera playerCamera;
        public float cameraMoveSpeed = 5f;

        [Header("Foto Settings")]
        public Canvas photoCanvas; // Canvas con la foto
        public Image photoImage; // Imagen que se mostrará
        public float fadeInDuration = 0.8f; // Duración del fade in
        public float fadeOutDuration = 0.6f; // Duración del fade out
        public GameObject blackBackground; // Fondo negro para cubrir toda la pantalla

        [Header("Timings")]
        public float delayBeforePhoto = 1.0f; // ⬅️ Tiempo para esperar tras abrir la puerta

        private Door currentDoor;
        private bool isPeeking = false;
        private bool isReturning = false;
        private bool isInCooldown = false;
        private float cooldownTimer = 0f;
        public float cooldownTime = 0.5f;

        private Vector3 originalWorldPosition;
        private Quaternion originalWorldRotation;
        private Transform cameraPeekPosition;
        private Transform originalParent;
        private Coroutine fadeCoroutine;

        void Start()
        {
            originalParent = transform.parent;

            // Ocultar foto y fondo al inicio
            if (photoCanvas != null)
                photoCanvas.gameObject.SetActive(false);

            if (blackBackground != null)
                blackBackground.SetActive(false);

            if (photoImage != null)
            {
                Color transparent = photoImage.color;
                transparent.a = 0f;
                photoImage.color = transparent;
            }
        }

        void Update()
        {
            if (isInCooldown)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    isInCooldown = false;
                }
            }

            if (isReturning)
            {
                transform.position = Vector3.Lerp(transform.position, originalWorldPosition, Time.deltaTime * cameraMoveSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, originalWorldRotation, Time.deltaTime * cameraMoveSpeed);

                if (Vector3.Distance(transform.position, originalWorldPosition) < 0.01f &&
                    Quaternion.Angle(transform.rotation, originalWorldRotation) < 1f)
                {
                    isReturning = false;
                    FinishRestore();
                }
                return;
            }

            if (isPeeking)
            {
                // Movimiento suave a la posición de peek
                if (cameraPeekPosition != null)
                {
                    transform.position = Vector3.Lerp(transform.position, cameraPeekPosition.position, Time.deltaTime * cameraMoveSpeed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, cameraPeekPosition.rotation, Time.deltaTime * cameraMoveSpeed);
                }

                // Salir con Q o Escape
                if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape)) && !isInCooldown)
                {
                    StartCooldown();
                    StartFadeOut();
                }
                return;
            }

            if (isInCooldown) return;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, DistanceOpen))
            {
                Door door = hit.transform.GetComponent<DoorScript.Door>();
                if (door != null)
                {
                    text.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && !isInCooldown)
                    {
                        StartCooldown();
                        currentDoor = door;
                        cameraPeekPosition = door.GetCameraPeekPosition();

                        if (!currentDoor.open && cameraPeekPosition != null)
                        {
                            // Guardar posición actual
                            originalWorldPosition = transform.position;
                            originalWorldRotation = transform.rotation;

                            currentDoor.OpenDoor();
                            playerMovement.canMove = false;
                            playerCamera.enabled = false;

                            transform.SetParent(null, true);

                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            isPeeking = true;
                            text.SetActive(false);
                            text2.SetActive(true);

                            // ⬅️ Esperar antes de mostrar la foto
                            StartCoroutine(ShowPhotoAfterDelay());
                        }
                    }
                }
                else
                {
                    text.SetActive(false);
                    text2.SetActive(false);
                }
            }
            else
            {
                text.SetActive(false);
                text2.SetActive(false);
            }
        }

        private IEnumerator ShowPhotoAfterDelay()
        {
            // espera antes de mostrar la foto
            yield return new WaitForSeconds(delayBeforePhoto);
            StartFadeIn();
        }

        private void StartCooldown()
        {
            isInCooldown = true;
            cooldownTimer = cooldownTime;
        }

        private void StartFadeIn()
        {
            // Mostrar fondo negro inmediatamente
            if (blackBackground != null)
                blackBackground.SetActive(true);

            // Mostrar el canvas y empezar fade in
            if (photoCanvas != null)
            {
                photoCanvas.gameObject.SetActive(true);

                // Cargar imagen específica para esta puerta
                DoorPhotoContent doorContent = currentDoor.GetComponent<DoorPhotoContent>();
                if (doorContent != null && doorContent.photoImage != null && photoImage != null)
                {
                    photoImage.sprite = doorContent.photoImage;
                }

                // Iniciar fade in
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadePhoto(0f, 1f, fadeInDuration));
            }
        }

        private void StartFadeOut()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAndReturn());
        }

        private IEnumerator FadeOutAndReturn()
        {
            // Fade out de la foto
            yield return FadePhoto(1f, 0f, fadeOutDuration);

            // Ocultar canvas y fondo después del fade out
            if (photoCanvas != null)
                photoCanvas.gameObject.SetActive(false);

            if (blackBackground != null)
                blackBackground.SetActive(false);

            // Iniciar retorno
            StartReturn();
        }

        private IEnumerator FadePhoto(float startAlpha, float targetAlpha, float duration)
        {
            if (photoImage != null)
            {
                Color startColor = photoImage.color;
                Color targetColor = startColor;
                startColor.a = startAlpha;
                targetColor.a = targetAlpha;

                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    photoImage.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                    yield return null;
                }

                // Asegurar el valor final exacto
                photoImage.color = targetColor;
            }
        }

        private void StartReturn()
        {
            if (currentDoor != null && currentDoor.open)
            {
                currentDoor.OpenDoor();
            }

            isPeeking = false;
            isReturning = true;
            text2.SetActive(false);
        }

        private void FinishRestore()
        {
            transform.SetParent(originalParent, true);
            transform.position = originalWorldPosition;
            transform.rotation = originalWorldRotation;

            playerMovement.canMove = true;
            playerCamera.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            currentDoor = null;
            cameraPeekPosition = null;
        }

        private void FixedUpdate()
        {
            if ((isPeeking || isReturning) && playerMovement.canMove)
            {
                playerMovement.canMove = false;
            }
        }
    }
}
