using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Mailbox Settings")]
    [SerializeField]
    private List<MailBox> mailBoxes = new List<MailBox>();

    [SerializeField]
    private List<GameObject> mailBoxSpawn = new List<GameObject>();

    [SerializeField]
    private GameObject mailBoxPrefab;

    [SerializeField]
    [Range(0, 8)]
    private int maxMailBoxOnLevel = 6;

    [Header("Roads Settings")]
    [SerializeField] [Range(0, 10)]
    private int maxRoadBlockedOnLevel = 3;

    [SerializeField]
    private List<GameObject> roads = new List<GameObject>();



    private void Start()
    {
        GenerateMailBoxs();
        GenerateRoads();
    }

    public List<MailBox> GetLevelMailBoxes() 
    {
        return mailBoxes;
    } 

    private void GenerateRoads()
    {
        for (int i = 0; i <= maxRoadBlockedOnLevel; i++) 
        { 
            GameObject selectedRoad = roads[Random.Range(0, roads.Count)];
            if (selectedRoad != null)
            {
                selectedRoad.SetActive(false);
                roads.Remove(selectedRoad);
            }

        }
    }

    private void GenerateMailBoxs()
    {
        for (int i = 0; i <= maxMailBoxOnLevel; i++)
        {
            int index = Random.Range(0, mailBoxSpawn.Count);
            Transform trans = mailBoxSpawn[index].transform;
            if (trans != null)
            {
                MailBox tempMail = Instantiate(mailBoxPrefab, trans.position, Quaternion.identity).GetComponent<MailBox>();
                Debug.Log(tempMail);
                if(tempMail != null)
                {
                    mailBoxes.Add(tempMail);
                    mailBoxSpawn.RemoveAt(index);
                }
            }

        }
    }
}
