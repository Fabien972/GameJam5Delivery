using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private LBoxs LBoxs;

    public int count;

    public override void OnEpisodeBegin()
    {
        count = 0;
        target = LBoxs.mailBoxes[count].transform;

        //target.localPosition = new Vector3(Random.Range(10, 18), 0, Random.Range(-3, 3));
        // transform.localPosition = new Vector3(Random.Range(-18,10), 0, Random.Range(-3, 3));

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.localPosition);

        sensor.AddObservation((Vector3)target.localPosition);

        foreach (MailBox item in LBoxs.mailBoxes)
        {
            sensor.AddObservation((Vector3)item.transform.position);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float mouvementSpeed = 5f;

        transform.localPosition += new Vector3(moveX,0, moveY) * Time.deltaTime * mouvementSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
        {
            if (other.name == target.name)
            {
                AddReward(200f);
                count++;
                target = LBoxs.mailBoxes[count % LBoxs.mailBoxes.Count].transform;
            }
            else
            {
                AddReward(-2f);
            }
            if (count >= LBoxs.mailBoxes.Count)
            {
                EndEpisode();
            }
        }
        else if (other.tag == "Wall")
        {
            AddReward(-100f);
            EndEpisode();
        }
    }
}
