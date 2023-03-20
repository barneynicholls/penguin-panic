using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PenguinAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField]
    private float detectionRadius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO
    }

    public void fishDropped(Vector3 position)
    {
        var distance = Vector3.Distance(position, transform.position);
        if (distance > detectionRadius)
        {
            Debug.Log("too far away");
            return;
        }

        agent.SetDestination(position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
