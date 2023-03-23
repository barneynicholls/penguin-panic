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
                position.y += 20f;

                var fish = Instantiate(fishPrefab, position, Quaternion.identity);

                var fishScript = fish.GetComponent<Fish>();
                if (fishScript is not null) fishScript.setAgents(agents);
            }
        }
    }
}
