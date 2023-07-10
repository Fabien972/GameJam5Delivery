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
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AI_Player ai = other.GetComponent<AI_Player>();
        if(ai != null)
        {
            if(ai.actualColor != actualPathColor)
            {
                Debug.Log("reduce speed");
                ai.speedRate *= 0.5f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AI_Player ai = other.GetComponent<AI_Player>();
        if (ai != null)
        {
            if (ai.actualColor != actualPathColor)
            {
                ai.speedRate *= 2;


            }
        }
    }
}
