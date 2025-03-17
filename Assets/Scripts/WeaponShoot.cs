using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab del proyectil
    public Transform firePoint; // Punto de origen del disparo
    public float bulletForce = 20f; // Fuerza del disparo
    public float fireRate = 0.5f; // Tiempo entre disparos
    private float nextFireTime = 0f; // Tiempo para el pr�ximo disparo
    private bool isEquipped = false; // Indica si el arma est� equipada

    void Update()
    {
        // Solo disparar si el arma est� equipada
        if (isEquipped && Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Actualizar el tiempo para el pr�ximo disparo
        }
    }

    void Shoot()
    {
        // Instanciar el proyectil en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Obtener el componente Rigidbody del proyectil
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Configurar el Rigidbody para mejorar la detecci�n de colisiones
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Mejorar la detecci�n de colisiones
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse); // Aplicar fuerza al proyectil
        }
        else
        {
            Debug.LogError("El prefab del proyectil no tiene un componente Rigidbody.");
        }

        Debug.Log("Disparo realizado.");
        Destroy(bullet, 5f); // Destruir el proyectil despu�s de 5 segundos
    }

    // M�todo para equipar el arma
    public void Equip()
    {
        isEquipped = true;
    }

    // M�todo para desequipar el arma
    public void Unequip()
    {
        isEquipped = false;
    }
}