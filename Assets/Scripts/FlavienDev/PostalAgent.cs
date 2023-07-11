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

public class PostalAgent : Agent
{
    
    [SerializeField] private float moveSpeed = 15.0f;
    
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private TextMeshProUGUI rewardText;

    private string lastValue = "0";
    private float value = 0.0f;
    
    [SerializeField] private List<ColorPath> ColorPaths = new List<ColorPath>();
    [SerializeField] private List<Material> colorMaterials = new List<Material>();
    [SerializeField] private SkinnedMeshRenderer characterMeshReference;
    private Rigidbody rb;

    public float speedRate = 1;
    public float maxSpeedRate;
    public float minSpeedRate;
    public ColorState actualColor;
    
    public ColorState currentFloorColor;

    // public List<PostalBox> deliveries = new List<PostalBox>();

    public override void Initialize()
    {
        minSpeedRate = speedRate * 0.5f;
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
    }
    
    public override void OnEpisodeBegin()
    {
        // Vector3 agentPosition;
        // Vector3 goalPosition;
        //
        // do {
        //     agentPosition = new Vector3(Random.Range(-3f, 11.5f), 0, Random.Range(-4.15f, 4.15f));
        //     goalPosition = new Vector3(Random.Range(-3f, 11.5f), 0, Random.Range(-4.15f, 4.15f));
        // } while (Vector3.Distance(agentPosition, goalPosition) < 4.0f);
        //
        // transform.localPosition = agentPosition;
        // targetTransform.localPosition = goalPosition;
        
        Vector3 agentPosition = Vector3.zero;
        transform.localPosition = agentPosition;

        foreach (var path in ColorPaths)
        {
            path.SetColor((ColorState) Random.Range(0, 4));
        }

        currentFloorColor = (ColorState) Random.Range(0, 4);
        SetColor(currentFloorColor);

        speedRate = maxSpeedRate;
        
        lastValue = value.ToString("F2");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(speedRate);
        sensor.AddObservation((int) currentFloorColor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed * speedRate;

        if (transform.localPosition.y <= -1)
        {
            CustomAddReward(-10);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
            return;
        }
        
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
            CustomAddReward(-0.01f);
        }

        CustomAddReward(-1.0f / MaxStep);

        if (StepCount >= MaxStep)
        {
            floorMeshRenderer.material = loseMaterial;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ColorPath path))
        {
            currentFloorColor = path.actualPathColor;
        }
        
        if (other.TryGetComponent(out PostalBox box))
        {
            CustomAddReward(10.0f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
    }

    void CustomAddReward(float reward)
    {
        AddReward(reward);
        value = GetCumulativeReward();
        
        rewardText?.SetText(lastValue + " / " + value.ToString("F2"));
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
