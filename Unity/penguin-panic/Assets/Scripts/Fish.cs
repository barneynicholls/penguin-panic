using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 20f;

    private GameObject[] agents = new GameObject[0];

    private bool hasLanded = false;

    // Update is called once per frame
    void Update()
    {
        if (!hasLanded)
            return;

        foreach (var agent in agents)
        {
            var agentScript = agent.GetComponent<PenguinAI>();
            if (agentScript is not null)
            {
                var distance = Vector3.Distance(agent.transform.position, transform.position);

                if (distance > detectionRadius)
                {
                    continue;
                }
                agentScript.fishNearby(gameObject);
            } 
        }
    }

    internal void eaten()
    {
        hasLanded = false;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasLanded = true;
    }

    public void setAgents(GameObject[] agentList)
    {
        agents = agentList;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
