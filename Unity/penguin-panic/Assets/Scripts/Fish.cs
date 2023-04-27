using UnityEngine;
using System;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 20f;

    private GameObject[] agents = new GameObject[0];

    private bool hasLanded = false;

    public Guid guid;

    void Start() => guid = Guid.NewGuid();

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

    internal void eaten(GameObject eatenBy)
    {
        Debug.Log(nameof(eaten));
        hasLanded = false;
        foreach (var agent in agents)

        {
            if(agent == eatenBy)
            {
                continue;
            }
            var agentScript = agent.GetComponent<PenguinAI>(); 
            if (agentScript is not null)
            {
                agentScript.fishEaten(gameObject);
            }
        }
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
