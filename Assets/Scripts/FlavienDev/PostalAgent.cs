using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

// mlagents-learn config\Postal.yaml --env=Build --no-graphics --run-id=PostalMassive105
// mlagents-learn config\Postal.yaml --run-id=PostalMassive110

public class PostalAgent : Agent
{
    [SerializeField] private float moveSpeed = 15.0f;
    [SerializeField] private float slowSpeedMultiplier = 0.1f;
    
    [SerializeField] private List<Material> colorMaterials = new List<Material>();
    [SerializeField] private SkinnedMeshRenderer characterMeshReference;

    private float speedRate = 1.0f;
    private ColorState actualColor;
    private ColorState currentFloorColor;

    private List<Transform> deliveries = new List<Transform>();
    private Transform nextDelivery;

    private static PostalAgent one;

    public override void Initialize()
    {
        deliveries = PostalGameManager.Instance.postalBoxes;
    }
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = PostalGameManager.Instance.GetRandomSpawnPos();

        currentFloorColor = (ColorState) Random.Range(1, 5);
        SetColor(currentFloorColor);

        speedRate = 1.0f;

        nextDelivery = deliveries[0];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(nextDelivery.localPosition);
        sensor.AddObservation((int) currentFloorColor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed * speedRate;

        int newColor = actions.DiscreteActions[0];

        if (newColor == 0)
        {
            // do nothing
        }

        if (newColor == 1)
        {
            SetColor(ColorState.BLUE);
        }

        if (newColor == 2)
        {
            SetColor(ColorState.YELLOW);
        }

        if (newColor == 3)
        {
            SetColor(ColorState.RED);
        }

        if (newColor == 4)
        {
            SetColor(ColorState.GREEN);
        }
        
        if (currentFloorColor != ColorState.NEUTRAL && currentFloorColor != actualColor)
        {
            speedRate = slowSpeedMultiplier;
            AddReward(-0.05f);
        }
        else
        {
            speedRate = 1.0f;
        }

        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out var hit, 20, 1 << 8 /* Road */))
        {
            if (hit.transform.TryGetComponent(out ColorPath path))
            {
                currentFloorColor = path.actualPathColor;
            }
        }
        else
        {
            AddReward(-50);
            PostalGameManager.Instance.AddFailure();
            EndEpisode();
        }

        AddReward(-1f / MaxStep); // always add a negative reward so the agent learns to always go faster

        if (StepCount >= MaxStep)
        {
            PostalGameManager.Instance.AddFailure();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        discreteActions[0] = (Input.GetKey(KeyCode.Keypad0) ? 1
                            : Input.GetKey(KeyCode.Keypad1) ? 2
                            : Input.GetKey(KeyCode.Keypad2) ? 3
                            : Input.GetKey(KeyCode.Keypad3) ? 4 : 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PostalBox box))
        {
            AddReward(50.0f);
            PostalGameManager.Instance.AddSuccess();
            EndEpisode();
        }
    }

    public void SetColor(ColorState color)
    {
        actualColor = color;

        if (characterMeshReference)
        {
            characterMeshReference.material = colorMaterials[((int) color)-1];
        }
    }
    
}
