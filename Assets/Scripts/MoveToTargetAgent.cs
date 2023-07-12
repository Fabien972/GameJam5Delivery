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
    [SerializeField] private MeshRenderer visualAgent;


    public int count;

    public override void OnEpisodeBegin()
    {
        count = 0;
        target = LBoxs.mailBoxes[count].transform;

        //target.localPosition = new Vector3(Random.Range(10, 18), 0, Random.Range(-3, 3));
        transform.localPosition = new Vector3(0, 0, 0);

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
            sensor.AddObservation((Vector3)item.transform.localPosition
                + LBoxs.transform.localPosition);
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
            Debug.Log("target");
            if (other.name == target.name)
            {
                AddReward(2000f);
                visualAgent.material.color = Color.green;
                count++;
                target = LBoxs.mailBoxes[count % LBoxs.mailBoxes.Count].transform;
            }
            else
            {
                visualAgent.material.color = Color.gray;
                AddReward(-100f);
            }
            if (count >= LBoxs.mailBoxes.Count)
            {
                EndEpisode();
            }
        }
        else if (other.tag == "Wall")
        {
            Debug.Log("wall");
            visualAgent.material.color = Color.red;
            AddReward(-100f);
            EndEpisode();
        }
    }
}
