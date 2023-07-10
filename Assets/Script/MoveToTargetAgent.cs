using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform env;
    [SerializeField] private MeshRenderer plane;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f, -1.5f), 1 , Random.Range(-3.5f, 3.5f));
        target.localPosition = new Vector3(Random.Range(1.5f, 3.5f), 0.5f, Random.Range(-3.5f, 3.5f));

        env.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        transform.rotation = Quaternion.identity;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float movementSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * movementSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Target target))
        {
            AddReward(10f);
            plane.material.color = Color.green;
            EndEpisode();
        }
        else if (other.TryGetComponent(out Wall wall))
        {
            AddReward(-2f);
            plane.material.color = Color.red;
            EndEpisode();
        }
    }
}
