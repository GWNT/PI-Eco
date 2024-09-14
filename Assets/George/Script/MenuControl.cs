using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MenuControl : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> menu1 = new List<Transform>();
    public List<Transform> menu2 = new List<Transform>();


    void Start()
    {
        for (int i = 0; i < menu1.Count; i++ )
        {
            menu1[i].transform.localScale = new Vector2(0,0);

        }

        for (int i = 0; i < menu2.Count; i++ )
        {
            menu2[i].transform.localScale = new Vector2(0,0);


        }

        StartCoroutine(TimeONMenu());
    }

    // Update is called once per frame
   IEnumerator TimeONMenu()
    {
        for (int i = 0; i < menu1.Count; i++)
        {
            menu1[i].DOScale(1f, 1.5f);
            yield return new WaitForSeconds(.25f);
        }
         
        for (int i = 0; i < menu2.Count; i++)
        {
            menu2[i].DOScale(0.5195122f, 1.5f);
            yield return new WaitForSeconds(.25f);
        }
           
    }
}
