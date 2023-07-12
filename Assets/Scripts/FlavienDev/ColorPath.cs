using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ColorPath : MonoBehaviour
{
    public ColorState actualPathColor;
    [SerializeField] private List<Material> colorMaterials = new List<Material>();
    [SerializeField] private List<Material> visionMaterials = new List<Material>();
    [SerializeField] private MeshRenderer visionRoad;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetColor(actualPathColor);
    }

    public void SetColor(ColorState color)
    {
        actualPathColor = color;
        if (meshRenderer != null)
        {
            switch (actualPathColor)
            {
                case ColorState.BLUE:
                    meshRenderer.material = colorMaterials[0];
                    // visionRoad.material = visionMaterials[0];
                    break;
                case ColorState.YELLOW:
                    meshRenderer.material = colorMaterials[1];
                    // visionRoad.material = visionMaterials[1];
                    break;
                case ColorState.RED:
                    meshRenderer.material = colorMaterials[2];
                    // visionRoad.material = visionMaterials[2];
                    break;
                case ColorState.GREEN:
                    meshRenderer.material = colorMaterials[3];
                    // visionRoad.material = visionMaterials[3];
                    break;
                default:
                    break;
            }
        }
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     PostalAgent agent = other.GetComponent<PostalAgent>();
    //     if (agent != null)
    //     {
    //         if (agent.actualColor != actualPathColor)
    //         {
    //             agent.speedRate = agent.minSpeedRate;
    //         }
    //         else
    //         {
    //             agent.speedRate = agent.maxSpeedRate;
    //         }
    //     }
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     PostalAgent agent = other.GetComponent<PostalAgent>();
    //     if (agent != null)
    //     {
    //         agent.speedRate = agent.maxSpeedRate;
    //     }
    // }
}
