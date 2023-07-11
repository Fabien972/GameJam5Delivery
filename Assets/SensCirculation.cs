using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sens
{
    Horizontal, Vertical , CoinHD, CoinHG , CoinBD , CoinBG ,haut , bas ,gauche ,droite
}

public class SensCirculation : MonoBehaviour
{
    public Sens sens;
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject left;
    // Start is called before the first frame update
    void Start()
    {
        switch (sens)
        {
            case Sens.Horizontal:
                right.SetActive(false);
                left.SetActive(false);
                break;
            case Sens.Vertical:
                up.SetActive(false);
                down.SetActive(false);
                break;
            case Sens.CoinHD:
                down.SetActive(false);
                left.SetActive(false);
                break;
            case Sens.CoinHG:
                down.SetActive(false);
                left.SetActive(false);
                break;
            case Sens.CoinBD:
                up.SetActive(false);
                left.SetActive(false);
                break;
            case Sens.CoinBG:
                up.SetActive(false);
                right.SetActive(false);
                break;
            case Sens.haut:
                up.SetActive(false);
                break;
            case Sens.bas:
                down.SetActive(false);
                break;
            case Sens.gauche:
                left.SetActive(false);
                break;
            case Sens.droite:
                right.SetActive(false);
                break;
            default:
                break;
        }
    }
}

   
