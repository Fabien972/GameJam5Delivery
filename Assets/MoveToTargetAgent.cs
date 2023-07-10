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
    [SerializeField] private Transform env;
    [SerializeField] private MeshRenderer visualAgent;

    public override void OnEpisodeBegin()
    {
        target.localPosition = new Vector3(Random.Range(-8,-2), 0, Random.Range(-3, 3));
        transform.localPosition = new Vector3(Random.Range(2,8), 0, Random.Range(-3, 3));

        env.rotation = Quaternion.Euler(0,  Random.Range(0, 360f),0);
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
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)target.localPosition);

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
        Debug.Log("ici");
        if(other.tag == "Target")
        {
            AddReward(10f);
            visualAgent.material.color = Color.green;
            EndEpisode();
        }
        else if(other.tag == "Wall")
        {
            AddReward(-2f);
            visualAgent.material.color = Color.red;
            EndEpisode();
        }
    }
}
