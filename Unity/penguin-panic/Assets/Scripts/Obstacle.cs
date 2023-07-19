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
    private GameObject dangerPrefab;

    [SerializeField]
    private float dangerRadius = 10f;

    private SphereCollider detectionCollider;

    void Start()
    {
        currentState = initialState;

        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.enabled = true;
        detectionCollider.radius = dangerRadius;
        detectionCollider.isTrigger = true;
    }

    void Update()
    {
        safePrefab.SetActive(currentState == ObstacleState.Safe);
        dangerPrefab.SetActive(currentState == ObstacleState.Danger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("agent"))
        {
            Debug.Log(nameof(OnTriggerEnter));
            currentState = ObstacleState.Danger;
            var agentScript = other.gameObject.GetComponent<PenguinAI>();
                agentScript.Flee(transform.position);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("agent"))
        {
            Debug.Log(nameof(OnTriggerExit));

            currentState = ObstacleState.Safe;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("agent"))
        {
            Debug.Log(nameof(OnTriggerStay));

            currentState = ObstacleState.Danger;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}

public enum ObstacleState
{
    Safe,
    Danger
}
