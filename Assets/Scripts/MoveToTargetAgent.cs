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
    [SerializeField] private List<Transform> hub = new List<Transform>();
    [SerializeField] private Transform env;
    [SerializeField] private MeshRenderer visualAgent;
    [SerializeField] private int count;


    public override void OnEpisodeBegin()
    {
        count = 0;

        target = hub[count];

        hub[0].GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        hub[1].GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        hub[3].GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        hub[3].GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        target.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

        transform.localPosition = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));


        hub[0].localPosition = new Vector3(Random.Range(3, 7), 0, Random.Range(3, 7));
        hub[1].localPosition = new Vector3(Random.Range(-3, -7), 0, Random.Range(3, 7));
        hub[2].localPosition = new Vector3(Random.Range(-3, -7), 0, Random.Range(-3, -7));
        hub[3].localPosition = new Vector3(Random.Range(3, 7), 0, Random.Range(-3, -7));


        env.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        transform.rotation = Quaternion.identity;

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

        sensor.AddObservation((Vector3)hub[0].localPosition);
        sensor.AddObservation((Vector3)hub[1].localPosition);
        sensor.AddObservation((Vector3)hub[2].localPosition);
        sensor.AddObservation((Vector3)hub[3].localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float mouvementSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0, moveY) * Time.deltaTime * mouvementSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
        {
            if (other.name == target.name)
            {
                AddReward(200f);
                visualAgent.material.color = Color.green;
                count++;

                target.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                target = hub[count % hub.Count];
                target.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            }

            else
            {
                AddReward(-2f);
                visualAgent.material.color = Color.gray;
            }
            if (count >= hub.Count)
            {
                EndEpisode();
            }
        }
        else if (other.tag == "Wall")
        {
            AddReward(-100f);
            visualAgent.material.color = Color.red;
            EndEpisode();
        }
    }
}



