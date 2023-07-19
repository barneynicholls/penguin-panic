using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    private float wanderSpeed = .5f;

    [SerializeField]
    private float fleeRadius = 20f;
    [SerializeField]
    private float fleeSpeed = 15f;
    [SerializeField]
    private float fleeAngularSpeed = 300f;
    [SerializeField]
    private float fleeDetectionRadius = 5f;

    [SerializeField]
    private float smellDistance = 5.0f;

    [SerializeField]
    private GameObject eyes;

    private NavMeshAgent agent;
    private GameObject targetFish;

    private Vector3 wanderTarget = Vector3.zero;
    private bool isWandering = false;

    private DateTime fleeTime;
    private bool isFleeing;

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
        else if (isFleeing)
        {
            if (agent.remainingDistance < 1 || (DateTime.Now - fleeTime) > TimeSpan.FromSeconds(5))
            {
                Debug.Log("reached flee point");

                isFleeing = false;
                Wander();
            }
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
        // TOOD this is getting called all the time
        // Debug.Log($"wandering: {gameObject.name}");
        isWandering = true;

        wanderTarget += new Vector3(
            Random.Range(-1.0f, 1.0f) * wanderJitter,
            0,
            Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        agent.speed = normalSpeed * wanderSpeed;

        Seek(targetWorld);
    }

    void Seek(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void Flee(Vector3 point)
    {
        if (isFleeing)
        {
            Debug.Log("already fleeing");
            return;
        }

        Debug.Log("flee");
        var distance = Vector3.Distance(point, transform.position);
        if (distance > fleeDetectionRadius)
        {
            Debug.Log("too far away");
            return;
        }

        var fleeDirection = (transform.position - point).normalized;
        var newGoal = transform.position + fleeDirection * fleeRadius;

        var path = new NavMeshPath();
        agent.CalculatePath(newGoal, path);

        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("fleeing");
            isFleeing = true;
            fleeTime = DateTime.Now;
            agent.speed = fleeSpeed;
            agent.angularSpeed = fleeAngularSpeed;
            agent.SetDestination(path.corners[path.corners.Length - 1]);
        }
        else
        {
            Debug.Log("invalid path");
        }
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
