using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PenguinAI : MonoBehaviour
{
    private NavMeshAgent agent;

    private GameObject targetFish;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1f)
        {
            if (targetFish is not null)
            {
                var fishScript = targetFish.GetComponent<Fish>();
                if (fishScript is not null)
                {
                    fishScript.eaten();
                    targetFish = null;
                }
                agent.ResetPath();
            }
        }
    }

    public void fishNearby(GameObject fish)
    {
        agent.SetDestination(fish.transform.position);
        targetFish = fish;
    }
}
