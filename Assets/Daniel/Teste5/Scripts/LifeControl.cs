using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeControl : MonoBehaviour
{
    [SerializeField] int _quantLifes;
    [SerializeField] Transform[] Lifes;
    [SerializeField] GameObject Player;

    void Start()
    {
        _quantLifes = 4; 
    }

    void Update()
    {
        
    }

    public void GanharVida()
    {
        if (_quantLifes < 4)
        {
            _quantLifes++;
            Lifes[_quantLifes].transform.localScale = Vector3.one;
        }
    }

    public void PerderVida()
    {
        if (_quantLifes >= 0)
        {
            Lifes[_quantLifes].transform.localScale = Vector3.zero;
            _quantLifes--;
        }
    }

    public void CheckMorte()
    {
        if (_quantLifes == -1)
        {
            Debug.Log("Player morreu!");
            Player.SetActive(false);
            _quantLifes--; // só pra não ficar spamando "Player morreu!" no console
        }
    }
}
