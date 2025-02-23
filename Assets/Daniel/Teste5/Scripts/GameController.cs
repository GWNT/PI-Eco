using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Áudios")]
    public AudioSource backgroundMusic;
    public AudioSource endingMusic;
    public AudioSource playerDeathMusic;
    public AudioSource disparoFlecha;
    public AudioSource inimigoPurificado;

    [Header("História")]
    public List<GameObject> historia;
    public int currentHistory;

    [Header("Botões para continuar história")]
    public Sprite xboxBT;
    public Sprite playstationBT;
    public Sprite tecladoBT;
    public Image tutorialBT;
    public GameObject continuar;

    [Header("Variáveis de controle")]
    public bool GameStarted = false;
    public bool canShowHistory = false;
    public bool showingHistory = false;
    public bool wasShowingHistory = false;
    public bool pacifico = false;
    [SerializeField] private Teste2Play playerScript;
    [SerializeField] private ImageColorController PanelController;

    public bool input = false;
    public int inimigosDerrotados = 0;
    public bool podeMostrarPrimeiraHistoria = true;
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
        input = SpecificInputDetected();

        if (PauseMenu.GameIsPaused)
        {
            backgroundMusic.Pause();

            if(showingHistory)
            {
                HideHistory();
                currentHistory--;
                wasShowingHistory = true;
                Debug.Log("História ocultada devido pausamento.");
            }

            InputSystem.Update();
            input = SpecificInputDetected();
        } 
        else if (!PauseMenu.GameIsPaused)
        {
            backgroundMusic.UnPause();

            if(wasShowingHistory)
            {
                StartCoroutine(ShowHistory(currentHistory));
                wasShowingHistory=false;
                Debug.Log("Re-exibindo história após pausamento.");
            }
        }

        if (GameStarted && canShowHistory && podeMostrarPrimeiraHistoria)
        {
            canShowHistory = false;
            podeMostrarPrimeiraHistoria = false;
            StartCoroutine(ShowHistory(0));
            Debug.Log("Exibindo primeira história.");
        }

        if (playerMorreu)
        {
            PanelController.FadeToDark();
            playerMorreu = !playerMorreu;
            playerDeathMusic.Play();
            Invoke("LoadGame", 3);
            return;
        }

        input = SpecificInputDetected();
        if (showingHistory && input)
        {
            HideHistory();
            Time.timeScale = 1;
            if (currentHistory == 8)
            {
                PanelController.FadeToDark();
                endingMusic.Play();
                Invoke("LoadMenu", 3);
            }
        }

        HistoryControl();
        CheckDevice();
    }

    void HistoryControl()
    {
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

        if (canShowHistory && currentHistory > 3 && currentHistory < 8)
        {
            StartCoroutine(ShowHistory(currentHistory));
        }
    }

    IEnumerator ShowHistory(int i)
    {
        canShowHistory = false;
        yield return new WaitForSeconds(.1f);
        Time.timeScale = 0;

        historia[i].SetActive(true);
        playerScript.enabled = false;
        //enemyScript.enabled = false;
        

        showingHistory = true;
        currentHistory = i;

        Debug.Log("Exibindo história.");
        continuar.SetActive(true);
    }

    void HideHistory()
    {

        historia[currentHistory].SetActive(false);
        currentHistory++;
        playerScript.enabled = true;
        //enemyScript.enabled = true;

        showingHistory = false;
        canShowHistory = true;

        continuar.SetActive(false);
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

    void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void CheckDevice()
    {
        if (Gamepad.current != null)
        {
            string deviceName = Gamepad.current.name;

            if (deviceName.Contains("DualShock") || deviceName.Contains("DualSense"))
            {
                tutorialBT.sprite = playstationBT;
            }
            else if (deviceName.Contains("XInput") || deviceName.Contains("Xbox"))
            {
                tutorialBT.sprite = xboxBT;
            }
        }
        else
        {
            tutorialBT.sprite = tecladoBT;
        }
    }
}
