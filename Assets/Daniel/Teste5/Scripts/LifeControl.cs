using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeControl : MonoBehaviour
{
    [SerializeField] int _quantLifes;
    [SerializeField] Transform[] Lifes;
    [SerializeField] GameObject Player;
    [SerializeField] Transform lifeTemp;
    [SerializeField] GameController gameController;
    [SerializeField] GameObject LifeHUD;

    void Start()
    {
        gameController = Camera.main.GetComponent<GameController>();

        _quantLifes = 4;
        lifeTemp.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameController.pacifico)
        {
            LifeHUD.SetActive(false);
        }
        else
        {
            LifeHUD.SetActive(true);
        }
    }

    public void GanharVida(Transform value)
    {
        if (_quantLifes < 4)
        {
           
            _quantLifes++;
            lifeTemp.position = value.position;
            lifeTemp.transform.parent = Lifes[_quantLifes].parent;
            lifeTemp.gameObject.SetActive(true);
            // lifeTemp.localPosition = value.localPosition;


          
            lifeTemp.DOLocalMove(Vector3.zero, 1);
            Invoke("TimeOff", 1);
            //value.DOLocalMove(Lifes[_quantLifes].transform.position, 1);
        }
    }

    void TimeOff()
    {
        lifeTemp.gameObject.SetActive(false);
        Lifes[_quantLifes].transform.localScale = Vector3.one;
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
            if (gameController.pacifico) return;
            
            Debug.Log("Player morreu!");
            gameController.playerMorreu = true;
            Player.SetActive(false);
            _quantLifes--; // só pra não ficar spamando "Player morreu!" no console
        }
    }
}
