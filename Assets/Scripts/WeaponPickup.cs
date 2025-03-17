using UnityEngine;
using UnityEngine.UI;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Transform weaponHolder;
    public KeyCode pickupKey = KeyCode.F;
    public KeyCode dropKey = KeyCode.G;
    public Text pickupText;
    public Transform playerCamera;
    public Transform aimPosition;

    public float aimSmoothness = 10f;
    private bool isInRange = false;
    private GameObject equippedWeapon;
    private static bool hasWeapon = false;

    // Variables para la alineación del arma con la cámara
    public float moveSmoothness = 5f;
    public float rotationSmoothness = 5f;
    public float maxPitchAngle = 45f;
    public float maxVerticalOffset = 0.1f;

    private Vector3 initialWeaponPosition;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    void Start()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }

        if (weaponHolder != null)
        {
            initialWeaponPosition = weaponHolder.localPosition;
            defaultPosition = weaponHolder.localPosition;
            defaultRotation = weaponHolder.localRotation;
        }
    }

    void Update()
    {
        if (isInRange && !hasWeapon && Input.GetKeyDown(pickupKey))
        {
            PickupWeapon();
        }

        if (hasWeapon && Input.GetKeyDown(dropKey))
        {
            DropWeapon();
        }

        if (hasWeapon)
        {
            AdjustWeaponPosition();
            AimWeapon();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWeapon)
        {
            isInRange = true;
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);
            }
        }
    }

    void PickupWeapon()
    {
        if (weaponPrefab != null && weaponHolder != null)
        {
            equippedWeapon = gameObject;
            equippedWeapon.transform.parent = weaponHolder;
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.Euler(-90, -90, -90);

            Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }

            MeshCollider meshCollider = equippedWeapon.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.enabled = false;
            }

            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);
            }

            WeaponShoot weaponShoot = equippedWeapon.GetComponent<WeaponShoot>();
            if (weaponShoot != null)
            {
                weaponShoot.Equip();
            }

            hasWeapon = true;
        }
    }

    void AimWeapon()
    {
        if (Input.GetMouseButton(1))
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, aimPosition.localPosition, aimSmoothness * Time.deltaTime);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, aimPosition.localRotation, aimSmoothness * Time.deltaTime);
        }
        else
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, defaultPosition, aimSmoothness * Time.deltaTime);
            weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, defaultRotation, aimSmoothness * Time.deltaTime);
        }
    }

    void DropWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.transform.parent = null;

            MeshCollider meshCollider = equippedWeapon.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = equippedWeapon.AddComponent<MeshCollider>();
            }
            meshCollider.convex = true;
            meshCollider.isTrigger = false;
            meshCollider.enabled = true;

            BoxCollider boxCollider = equippedWeapon.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }

            Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = equippedWeapon.AddComponent<Rigidbody>();
            }

            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.AddForce(weaponHolder.forward * 3f, ForceMode.Impulse);

            WeaponShoot weaponShoot = equippedWeapon.GetComponent<WeaponShoot>();
            if (weaponShoot != null)
            {
                weaponShoot.Unequip();
            }

            hasWeapon = false;
            equippedWeapon = null;
        }
    }

    void AdjustWeaponPosition()
    {
        float cameraPitch = playerCamera.eulerAngles.x;

        if (cameraPitch > 180)
        {
            cameraPitch -= 360;
        }

        cameraPitch = Mathf.Clamp(cameraPitch, -maxPitchAngle, maxPitchAngle);
        float verticalOffset = Mathf.Lerp(0, maxVerticalOffset, Mathf.Abs(cameraPitch) / maxPitchAngle);

        // Ajustar posición del arma sin moverla lateralmente
        Vector3 targetPosition = defaultPosition + new Vector3(0, verticalOffset, 0);
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPosition, moveSmoothness * Time.deltaTime);

        // Ajustar la rotación sin inclinar el arma en X
        Quaternion targetRotation = Quaternion.Euler(0, defaultRotation.eulerAngles.y, defaultRotation.eulerAngles.z);
        weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }
}
