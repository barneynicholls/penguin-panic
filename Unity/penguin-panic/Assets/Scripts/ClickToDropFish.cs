using UnityEngine;

public class ClickToDropFish : MonoBehaviour
{
    [SerializeField]
    private GameObject fishPrefab;

    [SerializeField]
    private GameObject mousePositionPrefab;

    private GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    private GameObject mousePositionInstance = null;

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            var position = hit.point;

            mousePositionInstance ??= Instantiate(mousePositionPrefab, position, Quaternion.identity);
            mousePositionInstance.SetActive(true);
            mousePositionInstance.transform.position = position;

            if (Input.GetMouseButtonDown(0))
            {
                position.y += 20f;

                var fish = Instantiate(fishPrefab, position, Quaternion.identity);

                var fishScript = fish.GetComponent<Fish>();
                if (fishScript is not null) fishScript.setAgents(agents);
            }
        }
        else
        {
            if (mousePositionInstance is not null)
            {
                mousePositionInstance.SetActive(false);
            }
        }


        

    }
}
