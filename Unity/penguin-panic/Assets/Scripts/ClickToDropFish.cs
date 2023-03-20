using UnityEngine;

public class ClickToDropFish : MonoBehaviour
{
    [SerializeField]
    private GameObject fishPrefab;

    private GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                var position = hit.point;
                position.y += 10f;

                var fish = Instantiate(fishPrefab, position, Quaternion.identity);

                foreach (var agent in agents)
                {
                    var agentScript = agent.GetComponent<PenguinAI>();
                    if (agentScript is not null)
                    {
                        agentScript.fishDropped(hit.point);
                    }
                }
            }
        }
    }
}
