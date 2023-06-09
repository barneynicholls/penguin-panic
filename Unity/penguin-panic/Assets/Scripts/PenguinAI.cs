using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PenguinAI : MonoBehaviour
{
    [SerializeField]
    private float wanderRadius = 10f;
    [SerializeField]
    private float wanderDistance = 20f;
    [SerializeField]
    private float wanderJitter = 1f;

    [SerializeField]
    private float smellDistance = 5.0f;

    [SerializeField]
    private GameObject eyes;

    private NavMeshAgent agent;
    private GameObject targetFish;
    private Vector3 wanderTarget = Vector3.zero;
    private bool isWandering = false;
    private float normalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetFish == null && !isWandering)
        {
            Wander();
        }

        if (agent.remainingDistance < 1f)
        {
            if (targetFish != null && !isWandering)
            {
                var distanceToFish = Vector3.Distance(transform.position, targetFish.transform.position);

                if (distanceToFish < 0.75f && targetFish.TryGetComponent<Fish>(out var fishScript))
                {
                    fishScript.eaten(gameObject);
                    targetFish = null;
                }
                Wander();
            }
            isWandering = false;
        }
    }

    void Wander()
    {
        isWandering = true;

        wanderTarget += new Vector3(
            Random.Range(-1.0f, 1.0f) * wanderJitter,
            0,
            Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        agent.speed = normalSpeed * 0.3f;

        Seek(targetWorld);
    }

    void Seek(Vector3 position)
    {
        agent.SetDestination(position);
    }

    bool canSeeFish(GameObject target)
    {
        Vector3 rayToTarget = target.transform.position - eyes.transform.position;
        float lookAngle = Vector3.Angle(eyes.transform.forward, rayToTarget);
        if (lookAngle < 60f && Physics.Raycast(eyes.transform.position, rayToTarget, out var hitInfo))
        {
            Debug.DrawRay(eyes.transform.position, rayToTarget, Color.green);
            return hitInfo.transform.gameObject.CompareTag("fish");
        }
        return false;
    }

    public void fishEaten(GameObject fish)
    {
        targetFish = null;
        Wander();
    }

    public void fishNearby(GameObject fish)
    {
        var distance = Vector3.Distance(transform.position, fish.transform.position);

        bool fishIsVisible = canSeeFish(fish);

        bool canSmellFish = distance < smellDistance;

      //  Debug.Log($"distance:{distance:F2}/{smellDistance:F2} canSmell: {canSmellFish} canSee:{fishIsVisible}");

        if (fishIsVisible || canSmellFish)
        {

            if (targetFish != null)
            {
                var distanceToCurrentTarget = Vector3.Distance(transform.position, targetFish.transform.position);
                if (distanceToCurrentTarget < distance)
                {
                    return;
                }
            }

            agent.speed = normalSpeed;
            targetFish = fish;
            Seek(fish.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, smellDistance);
    }
}
