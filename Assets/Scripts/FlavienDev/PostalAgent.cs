using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Random = UnityEngine.Random;

// mlagents-learn config\Postal.yaml --env=Build --no-graphics --run-id=PostalMassive105
// mlagents-learn config\Postal.yaml --run-id=PostalMassive110

public class PostalAgent : Agent
{
    
    [SerializeField] private float moveSpeed = 15.0f;
    
    [SerializeField] private List<Material> colorMaterials = new List<Material>();
    [SerializeField] private SkinnedMeshRenderer characterMeshReference;
    private Rigidbody rb;

    public float speedRate = 1;
    public float maxSpeedRate;
    public float minSpeedRate;
    public ColorState actualColor;
    
    public ColorState currentFloorColor;

    private List<Transform> deliveries = new List<Transform>();

    private List<GameObject> checkpoints = new List<GameObject>();
    
    public override void Initialize()
    {
        minSpeedRate = speedRate * 0.1f;
        maxSpeedRate = speedRate;

        // GameManager gameManagerRef = GameObject.Find("GameManager").GetComponent<GameManager>();
        // if(gameManagerRef != null )
        // {
        //     foreach (MailBox item in gameManagerRef.GetLevelMailBoxes())
        //     {
        //         deliveries.Add(item as PostalBox);
        //     }
        // }
        
        // Cache the agent rigidbody
        rb = GetComponent<Rigidbody>();

        deliveries = PostalGameManager.Instance.postalBoxes;
    }
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = PostalGameManager.Instance.GetRandomSpawnPos();

        currentFloorColor = (ColorState) Random.Range(0, 4);
        SetColor(currentFloorColor);

        speedRate = maxSpeedRate;
        
        checkpoints.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(deliveries[0].localPosition);
        sensor.AddObservation((int) currentFloorColor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        var lastDist = Vector3.Distance(transform.localPosition, deliveries[0].localPosition);

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed * speedRate;

        // if (Vector3.Distance(transform.localPosition, deliveries[0].localPosition) >= lastDist)
        // {
        //     AddReward(-0.3f);
        // }

        int newColor = actions.DiscreteActions[0];
        
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
        
        if (currentFloorColor != actualColor)
        {
            speedRate = minSpeedRate;
            AddReward(-0.04f);
        }
        else
        {
            speedRate = maxSpeedRate;
        }
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out hit, 100, 1 << 8 /* Road */))
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

        AddReward(-0.5f / MaxStep);

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
            AddReward(100.0f);
            PostalGameManager.Instance.AddSuccess();
            EndEpisode();
        }
        
        if (other.CompareTag("Checkpoint") && !checkpoints.Contains(other.gameObject))
        {
            checkpoints.Add(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") && !checkpoints.Contains(other.gameObject))
        {
            checkpoints.Add(other.gameObject);
            AddReward(20.0f);
        }
    }

    public void SetColor(ColorState color)
    {
        actualColor = color;

        if(characterMeshReference != null)
        {
            switch (color)
            {
                case ColorState.BLUE:
                    characterMeshReference.material = colorMaterials[0];
                    break;
                case ColorState.YELLOW:
                    characterMeshReference.material = colorMaterials[1];
                    break;
                case ColorState.RED:
                    characterMeshReference.material = colorMaterials[2];
                    break;
                case ColorState.GREEN:
                    characterMeshReference.material = colorMaterials[3];
                    break;
                default:
                    break;
            }
        }
       
    }
    
}
