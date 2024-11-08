using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    [SerializeField] private Teste2Play playerScript;
    [SerializeField] List<Transform> _botaoSelecionado;
    [SerializeField] List<Transform> _menus;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    public void PauseOrResume(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if (GameIsPaused)
            {
                Resume();
                CloseAllMenus();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
        playerScript.enabled = true;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        _botaoSelecionado[0].GetComponent<Button>().Select();

        playerScript.enabled = false;
    }

    public void LoadMenu()

    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void SelectButton(int i)
    {
        _botaoSelecionado[i].GetComponent<Button>().Select();
    }

    private void CloseAllMenus()
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            _menus[i].gameObject.SetActive(false);
        }
    }
}