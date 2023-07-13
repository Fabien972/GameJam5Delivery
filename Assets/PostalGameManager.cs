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
    public int agentCount = 20;
    public float randomRate = 15;
    private List<GameObject> agents = new List<GameObject>();
    
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failureText;
    
    [HideInInspector] public List<Transform> postalBoxes = new List<Transform>();
    
    public List<Transform> destroyableRoads1 = new List<Transform>();
    public List<Transform> destroyableRoads2 = new List<Transform>();
    public List<Transform> destroyableRoads3 = new List<Transform>();
    public List<Transform> destroyableRoads4 = new List<Transform>();
    public List<Transform> destroyableRoads5 = new List<Transform>();
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

        // var keep = postalBoxes[Random.Range(0, postalBoxes.Count)];
        // foreach (var e in postalBoxes)
        // {
        //     if (e != keep)
        //     {
        //         Destroy(e.gameObject);
        //     }
        // }
        // postalBoxes.Add(keep);

        postalBoxes.Clear();
        for (int i = 0; i < 1 /* boxcount */; i++)
        {
            var box = Instantiate(postalBoxPrefab, environment.transform);
            box.transform.localPosition = GetRandomSpawnPos();
            postalBoxes.Add(box.transform);
        }

        // int index;
        
        // index = Random.Range(0, destroyableRoads1.Count);
        // Destroy(destroyableRoads1[index].gameObject);
        //
        // if (Random.value <= 0.5)
        // {
        //     index = Random.Range(0, destroyableRoads2.Count);
        //     Destroy(destroyableRoads2[index].gameObject);
        //
        //     index = Random.Range(0, destroyableRoads5.Count);
        //     Destroy(destroyableRoads5[index].gameObject);
        // }
        // else
        // {
        //     index = Random.Range(0, destroyableRoads3.Count);
        //     Destroy(destroyableRoads3[index].gameObject);
        //     
        //     index = Random.Range(0, destroyableRoads4.Count);
        //     Destroy(destroyableRoads4[index].gameObject);
        // }

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
        
        // foreach (var box in postalBoxes)
        // {
        //     box.localPosition = GetRandomSpawnPos();
        // }
        
        foreach (var e in colorRoads)
        {
            e.SetColor((ColorState) Random.Range(1, 5));
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

    public void CheckAgents()
    {
        // if (success + failure >= agents.Count)
        // {
        //     Invoke(nameof(ReloadScene), 0.5f);
        // }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void AddSuccess()
    {
        success++;
        successText.SetText(success.ToString());
        CheckAgents();
    }
    
    public void AddFailure()
    {
        failure++;
        failureText.SetText(failure.ToString());
        CheckAgents();
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
