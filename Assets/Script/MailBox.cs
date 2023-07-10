using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        AI_Player ai = other.GetComponent<AI_Player>();
        if (ai != null)
        {
            ai.deliveries.Remove(this);
            ai.NextDelivery();
        }
    }
}
