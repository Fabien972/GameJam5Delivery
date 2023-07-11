using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ColoredPath : MonoBehaviour
{
    public ColorState actualPathColor;
    [SerializeField] private List<Material> colorMaterials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        //Colored path or neutral
        if (Random.value > 0.5f)
        {
            actualPathColor = ColorState.NEUTRAL;
        }
        else
        {
            actualPathColor = (ColorState)Random.Range(1, 5);
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            //Randomcolor
            if (meshRenderer != null)
            {
                switch (actualPathColor)
                {
                    case ColorState.BLUE:
                        meshRenderer.material = colorMaterials[0];
                        break;
                    case ColorState.YELLOW:
                        meshRenderer.material = colorMaterials[1];
                        break;
                    case ColorState.RED:
                        meshRenderer.material = colorMaterials[2];
                        break;
                    case ColorState.GREEN:
                        meshRenderer.material = colorMaterials[3];
                        break;
                    default:
                        break;
                }
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(actualPathColor != ColorState.NEUTRAL)
        {
            AI_Player ai = other.GetComponent<AI_Player>();
            if (ai != null)
            {
                if (ai.actualColor != actualPathColor)
                {
                    ai.speedRate = ai.minSpeedRate;
                }
                else
                {
                    ai.speedRate = ai.maxSpeedRate;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (actualPathColor != ColorState.NEUTRAL)
        {
            AI_Player ai = other.GetComponent<AI_Player>();
            if (ai != null)
            {
                ai.speedRate = ai.maxSpeedRate;
            }
        }
    }
}
