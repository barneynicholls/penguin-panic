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
    private float dangerRadius = 5f;

    private SphereCollider detectionCollider;

    void Start()
    {
        currentState = initialState;

        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.enabled = true;
        detectionCollider.radius = safeRadius;
        detectionCollider.isTrigger = true;
    }

    void Update()
    {
        safePrefab.SetActive(currentState == ObstacleState.Safe);
        warningPrefab.SetActive(currentState == ObstacleState.Warning);
        dangerPrefab.SetActive(currentState == ObstacleState.Danger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "agent")
        {
            currentState = ObstacleState.Warning;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "agent")
        {
            var distanceToAgent = Vector3.Distance(transform.position, other.gameObject.transform.position);
            Debug.Log($"{distanceToAgent}");

            if (distanceToAgent > safeRadius)
                currentState = ObstacleState.Safe;

            if (distanceToAgent <= safeRadius)
                currentState = ObstacleState.Warning;

            if (distanceToAgent <= dangerRadius)
            {
                currentState = ObstacleState.Danger;
                var agentScript = other.gameObject.GetComponent<PenguinAI>();
                agentScript.Flee(transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeRadius);
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
