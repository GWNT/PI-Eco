using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameController : MonoBehaviour
{
    [Header("Áudio")]
    public AudioSource backgroundMusic;

    [Header("História")]
    public List<GameObject> historia;
    public int currentHistory;

    // Variáveis de controle
    public bool GameStarted = false;
    public bool canShowHistory = false;
    public bool showingHistory = false;
    [SerializeField] private Teste2Play playerScript;
    //[SerializeField] private MoveInimigo enemyScript;
    [SerializeField] private ImageColorController PanelController;

    public bool input = false;
    public int inimigosDerrotados = 0;
    public bool purificouInimigo = false;
    public bool purificouTodosInimigos = false;
    public bool bossDerrotado = false;

    public bool playerMorreu = false;

    void Start()
    {
        PanelController = Camera.main.GetComponent<ImageColorController>();

        for (int i = 0; i < historia.Count; i++)
        {
            historia[i].SetActive(false);
        }

        PanelController.FadeToLight();
    }

    
    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            backgroundMusic.Pause();
        } else if (!PauseMenu.GameIsPaused)
        {
            backgroundMusic.UnPause();
        }

        if (GameStarted && canShowHistory)
        {
            StartCoroutine(ShowHistory(0));
        }

        if (playerMorreu)
        {
            PanelController.FadeToDark();
            playerMorreu = !playerMorreu;
            return;
        }

        input = SpecificInputDetected();

        if (showingHistory && input)
        {
            HideHistory(); 
            if (currentHistory == 4)
            {
                PanelController.FadeToDark();
            }
        }

        if (inimigosDerrotados == 1 && !purificouInimigo)
        {
            StartCoroutine(ShowHistory(1));
            purificouInimigo = true;
        }

        if (inimigosDerrotados == 7 && !purificouTodosInimigos)
        {
            StartCoroutine(ShowHistory(2));
            purificouTodosInimigos = true;
        }

        if (inimigosDerrotados == 8 && bossDerrotado)
        {
            StartCoroutine(ShowHistory(3));
            bossDerrotado = false; // evitar repetição
        }

    }

    IEnumerator ShowHistory(int i)
    {
        yield return new WaitForSeconds(.1f);

        historia[i].SetActive(true);
        playerScript.enabled = false;
        //enemyScript.enabled = false;
        canShowHistory = false;

        showingHistory = true;
        currentHistory = i;
    }

    void HideHistory()
    {
        historia[currentHistory].SetActive(false);
        currentHistory++;
        playerScript.enabled = true;
        //enemyScript.enabled = true;

        showingHistory = false; 
    }

    public bool AnyInputDetected()
    {
        // Verifica se qualquer tecla foi pressionada no teclado
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }

        // Verifica se qualquer botão foi pressionado no gamepad
        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool SpecificInputDetected()
    {
        // Verifica se a tecla Enter foi pressionada no teclado
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            return true;
        }

        // Verifica se o botão sul (A no Xbox, X no PlayStation) foi pressionado no gamepad
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }
}
