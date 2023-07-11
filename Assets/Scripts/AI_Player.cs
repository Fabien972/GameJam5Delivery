using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public enum ColorState { NEUTRAL, BLUE, YELLOW, RED, GREEN };

public class AI_Player : MonoBehaviour
{
    
    [SerializeField] private List<Material> colorMaterials = new List<Material>();
    [SerializeField] private SkinnedMeshRenderer characterMeshReference;

    [SerializeField] private float speed = 15.0f;
    public float speedRate = 1;

    public float maxSpeedRate;
    public float minSpeedRate;
    public ColorState actualColor;

    public List<MailBox> deliveries = new List<MailBox>();

    private void Start()
    {
        minSpeedRate = speedRate * 0.5f;
        maxSpeedRate = speedRate;

        GameManager gameManagerRef = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gameManagerRef != null )
        {
            foreach (MailBox item in gameManagerRef.GetLevelMailBoxes())
            {
                deliveries.Add(item);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Inputs de debug pour déplacer le personnage
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDown();
        }
        if (Input.GetKey(KeyCode.Z))
        {
            MoveUp();
        }


        //Inputs de debug pour changer la couleur du personnage
        if (Input.GetKey(KeyCode.Keypad0))
        {
            SetColor(ColorState.BLUE);
        }

        if (Input.GetKey(KeyCode.Keypad1))
        {
            SetColor(ColorState.YELLOW);
        }

        if (Input.GetKey(KeyCode.Keypad2))
        {
            SetColor(ColorState.RED);
        }

        if (Input.GetKey(KeyCode.Keypad3))
        {
            SetColor(ColorState.GREEN);
        }
    }

#region Movement functions
    public void MoveUp()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime * speedRate));
    }

    public void MoveDown()
    {
        transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime * speedRate));
    }

    public void MoveLeft()
    {
        transform.Translate(new Vector3(-speed * Time.deltaTime * speedRate, 0, 0));
    }

    public void MoveRight()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime * speedRate, 0, 0));
    }
    #endregion

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

    public void NextDelivery()
    {
    
        if(deliveries.Count <= 0) 
        {
            Debug.Log("Finish");
        }
    }

}
