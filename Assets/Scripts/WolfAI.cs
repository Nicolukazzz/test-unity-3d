using UnityEngine;
using UnityEngine.AI;

public class WolfAI : MonoBehaviour
{
    public NavMeshAgent agent; // Referencia al NavMeshAgent
    public float wanderRadius = 10f; // Radio de movimiento aleatorio
    public float wanderTimer = 5f; // Tiempo entre cada movimiento
    public Animator animator; // Referencia al componente Animator
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Obtiene el NavMeshAgent
        timer = wanderTimer;
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            MoveToRandomLocation();
            timer = 0;
        }
    }

    void MoveToRandomLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
