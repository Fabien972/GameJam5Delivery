using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostalBox : MailBox
{

    private void OnTriggerEnter(Collider other)
    {
        // PostalAgent agent = other.GetComponent<PostalAgent>();
        // if (agent != null)
        // {
        //     agent.NextDelivery();
        // }
    }
}
