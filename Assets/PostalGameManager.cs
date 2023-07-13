using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostalGameManager : MonoBehaviour
{
    public bool pauseRandom = false;
    
    public Transform environment;
    public Transform minPosition;
    public Transform maxPosition;
    
    public GameObject postalBoxPrefab;
    
    public GameObject agentPrefab;
    public int boxcount = 20;
    public int agentCount = 20;
    public float randomRate = 15;
    private List<GameObject> agents = new List<GameObject>();
    
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failureText;
    
    public List<Transform> postalBoxes = new List<Transform>();
    
    
    private List<ColorPath> colorRoads = new List<ColorPath>();

    public static PostalGameManager Instance { get; private set; }

    private int success;
    private int failure;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        colorRoads.Clear();
        var roads = GameObject.FindGameObjectsWithTag("Road");
        colorRoads.AddRange(roads.Where(r => r.GetComponent<ColorPath>()).Select(r => r.GetComponent<ColorPath>()));

       

        postalBoxes.Clear();
        for (int i = 0; i <  boxcount ; i++)
        {
            var box = Instantiate(postalBoxPrefab, environment.transform);
            postalBoxes.Add(box.transform);
        }

       

        for (int i = 0; i < agentCount; i++)
        {
            var agent = Instantiate(agentPrefab, environment.transform);
            agents.Add(agent);
        }
        
        InvokeRepeating(nameof(RandomizeEnv), 0, randomRate);
    }
    
    public void RandomizeEnv()
    {
        if (pauseRandom)
        {
            return;
        }
        
        foreach (var box in postalBoxes)
        {
            box.localPosition = GetRandomSpawnPos();
        }
        
        foreach (var e in colorRoads)
        {
            e.SetColor((ColorState) Random.Range(0, 4));
        }
    }

    public Vector3 GetRandomSpawnPos()
    {
        var position = minPosition.localPosition;
        var position1 = maxPosition.localPosition;
        return new Vector3(Random.Range(position.x, position1.x),
            Random.Range(position.y, position1.y),
            Random.Range(position.z, position1.z));
    }


    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void AddSuccess()
    {
        success++;
        successText.SetText(success.ToString());
    }
    
    public void AddFailure()
    {
        failure++;
        failureText.SetText(failure.ToString());
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
