using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open;
        public float smooth = 1.0f;
        float DoorOpenAngle = -90.0f;
        float DoorCloseAngle = 0.0f;
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Camera Peek Settings")]
        public Transform cameraPeekPosition; // Posición fija para la cámara
        public Vector3 peekRotationOffset = Vector3.zero; // Ajuste adicional de rotación

        void Start()
        {
            asource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (open)
            {
                var target = Quaternion.Euler(0, DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(0, DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        public void OpenDoor()
        {
            open = !open;
        }

        public Transform GetCameraPeekPosition()
        {
            return cameraPeekPosition;
        }

        // Nuevo método para obtener la rotación fija
        public Quaternion GetPeekRotation()
        {
            if (cameraPeekPosition != null)
            {
                return cameraPeekPosition.rotation * Quaternion.Euler(peekRotationOffset);
            }
            return Quaternion.identity;
        }
    }
}