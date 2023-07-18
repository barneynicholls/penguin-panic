using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private ObstacleState initialState = ObstacleState.Safe;

    private ObstacleState currentState = ObstacleState.Safe;

    [SerializeField]
    private GameObject safePrefab;
    [SerializeField]
    private GameObject warningPrefab;
    [SerializeField]
    private GameObject dangerPrefab;

    [SerializeField]
    private float safeRadius = 15f;
    [SerializeField]
    private float warningRadius = 10f;
    [SerializeField]
    private float dangerRadius = 5f;

    [SerializeField]
    private SphereCollider safeCollider;
    [SerializeField]
    private SphereCollider warningCollider;
    [SerializeField]
    private SphereCollider dangerCollider;

    // nested colliders are a no go
    // might need to script and register each one with parent script

    void Start()
    {
        currentState = initialState;
        safeCollider.radius = safeRadius;
        safeCollider.isTrigger = true;
        warningCollider.radius = safeRadius;
        warningCollider.isTrigger = true;
        dangerCollider.radius = safeRadius;
        dangerCollider.isTrigger = true;
    }

    void Update()
    {
        safePrefab.SetActive(currentState == ObstacleState.Safe);
        warningPrefab.SetActive(currentState == ObstacleState.Warning);
        dangerPrefab.SetActive(currentState == ObstacleState.Danger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "agent")
        {
            Debug.Log(other.gameObject.name);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, warningRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}

public enum ObstacleState
{
    Safe,
    Warning,
    Danger
}
