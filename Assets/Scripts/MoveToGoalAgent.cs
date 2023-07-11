using System;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private TextMeshProUGUI rewardText;

    private string lastValue = "0";
    private float value = 0.0f;
    
    private Rigidbody rb;
    
    public override void Initialize()
    {
        // Cache the agent rigidbody
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        Vector3 agentPosition;
        Vector3 goalPosition;
        
        do {
            agentPosition = new Vector3(Random.Range(-3f, 11.5f), 0, Random.Range(-4.15f, 4.15f));
            goalPosition = new Vector3(Random.Range(-3f, 11.5f), 0, Random.Range(-4.15f, 4.15f));
        } while (Vector3.Distance(agentPosition, goalPosition) < 4.0f);

        transform.localPosition = agentPosition;
        targetTransform.localPosition = goalPosition;
        
        lastValue = value.ToString("F2");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        // Vector3 currentDir = (new Vector3(moveX, 0, moveZ)).normalized;
        // Vector3 targetDir = (targetTransform.localPosition - transform.localPosition).normalized;
        //
        // if (Mathf.Approximately(moveX + moveZ, 0))
        // {
        //     SetReward(-1);
        // }
        // else
        // {
        //     float angle = Vector3.Angle(currentDir, targetDir);
        //     // SetReward(1.0f - (angle/180.0f));
        // }
        
        AddReward(-1.0f / MaxStep);
        value = GetCumulativeReward();

        if (StepCount >= MaxStep)
        {
            floorMeshRenderer.material = loseMaterial;
        }
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
            AddReward(5.0f);
            floorMeshRenderer.material = winMaterial;
        }
        
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-5.0f);
            floorMeshRenderer.material = loseMaterial;
        }
        
        value = GetCumulativeReward();
        EndEpisode();
    }
    
    private void Update()
    {
        rewardText.SetText(lastValue + " / " + value.ToString("F2"));
    }
}
