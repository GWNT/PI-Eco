using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject HelpMenu;
    [SerializeField] GameObject Controles;
    [SerializeField] GameObject ConfirmacaoSair;


    void Start()
    {
        MainMenu.SetActive(true);
        HelpMenu.SetActive(false);
        Controles.SetActive(false);
        ConfirmacaoSair.SetActive(false);
    }

    public void Jogar()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
            Debug.Log("Quitting game...");
        #endif
    }

}
