using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HudController : MonoBehaviour
{
    [SerializeField] List<MenuController> _menuControls;
    void Start()
    {
        for (int i = 0;  i < _menuControls.Count; i++)
        {
            _menuControls[i].transform.localScale = Vector3.zero;
            _menuControls[i].gameObject.SetActive(false);
        }
        _menuControls[0].gameObject.SetActive(true);
        _menuControls[0].MenuOFF();
        _menuControls[0].transform.DOScale(1, 0.15f);
        _menuControls[0].ChamarMenu();
    }

    public void ChamarMenuControl(int value)
    {
        for (int i = 0; i < _menuControls.Count; i++)
        {
            _menuControls[i].transform.localScale = Vector3.zero;
            _menuControls[i].MenuOFF();
            _menuControls[i].gameObject.SetActive(false);
        }
        _menuControls[value].gameObject.SetActive(true);
        _menuControls[value].transform.DOScale(1, .15f);
        _menuControls[value].ChamarMenu();
    }
}
