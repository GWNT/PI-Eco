using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] List<Transform> _itemMenu;

    public void MenuOFF()
    {
        for (int i = 0; i < _itemMenu.Count; i++)
        {
            _itemMenu[i].localScale = Vector3.zero;

        }
    }

    public void ChamarMenu()
    {
        _itemMenu[0].GetComponent<Button>().Select();
        StartCoroutine(TimeItens());
    }

    // Update is called once per frame
    IEnumerator TimeItens()
    {
        for (int i = 0; i < _itemMenu.Count; i++)
        {
            yield return new WaitForSeconds(.15f);
            _itemMenu[i].DOScale(1.5f, .15f);
            yield return new WaitForSeconds(.15f);
            _itemMenu[i].DOScale(1f, .15f);

        }
    }
}
