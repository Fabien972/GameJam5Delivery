using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private float lastDistance = Single.MaxValue;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
        targetTransform.localPosition = new Vector3(8.5f, 0, 0) + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        // float distance = Vector3.Distance(transform.position, targetTransform.position);
        //
        // if (distance < lastDistance)
        // {
        //     AddReward(0.01f);
        // }
        // else
        // {
        //     AddReward(-0.01f);
        // }
        //
        // lastDistance = distance;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(10.0f);
            floorMeshRenderer.material = winMaterial;
            
            EndEpisode();
        }
        
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-10.0f);
            floorMeshRenderer.material = loseMaterial;
            
            EndEpisode();
        }
    }
}
