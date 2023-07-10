using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<MailBox> mailBoxes = new List<MailBox>();

    public List<MailBox> GetLevelMailBoxes() 
    {
        return mailBoxes;
    } 
}
