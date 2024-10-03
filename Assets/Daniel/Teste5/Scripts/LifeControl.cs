using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeControl : MonoBehaviour
{
    [SerializeField] int _quantLifes;
    [SerializeField] Transform[] Lifes;

    void Start()
    {
        _quantLifes = 4;
        Lifes[0].transform.localScale = Vector3.one;
        Lifes[1].transform.localScale = Vector3.one;
        Lifes[2].transform.localScale = Vector3.one;   
    }

    public void GanharVida()
    {
        // nada por enquanto :)
    }

    public void PerderVida()
    {
        if (_quantLifes >= 0)
        {
            Lifes[_quantLifes].transform.localScale = Vector3.zero;
            _quantLifes--;
        }
    }
}
