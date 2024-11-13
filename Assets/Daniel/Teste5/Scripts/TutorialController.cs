using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TutorialController : MonoBehaviour
{
    // Tutoriais para Xbox, PlayStation e teclado
    public GameObject xboxTutorial;
    public GameObject playstationTutorial;
    public GameObject keyboardTutorial;

    public GameObject MenuControles;
    public GameObject MenuControlesBotaoVoltar;

    public GameController gameController;

    private bool tutorialVisible = true;


    void Start()
    {
        gameController = Camera.main.GetComponent<GameController>();

        ShowTutorial();
    }

    void Update()
    {
        // Verifica se o tutorial está visível e se qualquer botão/tecla foi pressionado
        if (tutorialVisible && gameController.AnyInputDetected())
        {
            HideTutorial();
            gameController.GameStarted = true;
            gameController.canShowHistory = true;
        }
    }

    void ShowTutorial()
    {
        MenuControlesBotaoVoltar.SetActive(false);
        MenuControles.SetActive(true);

        if (Gamepad.current != null)
        {
            string deviceName = Gamepad.current.name;

            if (deviceName.Contains("DualShock") || deviceName.Contains("DualSense"))
            {
                // Controle de PlayStation
                playstationTutorial.SetActive(true);
                Debug.Log("Controle Xbox detectado: " + deviceName);
            }
            else if (deviceName.Contains("XInput") || deviceName.Contains("Xbox"))
            {
                // Controle de Xbox
                xboxTutorial.SetActive(true); 
                Debug.Log("Controle Playstation detectado: " + deviceName);
            }
        }
        else
        {
            // Nenhum controle conectado, mostrar tutorial de teclado
            keyboardTutorial.SetActive(true); 
            Debug.Log("Nenhum controle detectado. Exibindo tutorial para teclado.");
        }
    }

    void HideTutorial()
    {
        MenuControles.SetActive(false);
        MenuControlesBotaoVoltar.SetActive(true);

        xboxTutorial.SetActive(false);
        playstationTutorial.SetActive(false);
        keyboardTutorial.SetActive(false);
        tutorialVisible = false;
    }

    bool AnyInputDetected()
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
}
