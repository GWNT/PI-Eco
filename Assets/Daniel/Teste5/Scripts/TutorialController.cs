using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TutorialController : MonoBehaviour
{
    // Sprites de tutoriais para Xbox, PlayStation e teclado
    public Sprite xboxTutorial;
    public Sprite playstationTutorial;
    public Sprite keyboardTutorial;

    // Referência ao componente de imagem do tutorial
    public UnityEngine.UI.Image tutorialImage;

    private bool tutorialVisible = true;

    // Testes> 
    bool firstRun = true;  //verificar se é a primeira run
    public static bool GameStarted = false;

    void Start()
    {
        ShowTutorial();
        /*
        if (firstRun)
        {
            ShowTutorial();
            firstRun = false;
        } */
    }

    void Update()
    {
        // Verifica se o tutorial está visível e se qualquer botão/tecla foi pressionado
        if (tutorialVisible && AnyInputDetected())
        {
            HideTutorial();
            GameStarted = true;
        }
    }

    void ShowTutorial()
    {
        tutorialImage.gameObject.SetActive(true);

        if (Gamepad.current != null)
        {
            string deviceName = Gamepad.current.name;

            if (deviceName.Contains("DualShock") || deviceName.Contains("DualSense"))
            {
                // Controle de PlayStation
                tutorialImage.sprite = playstationTutorial;
                Debug.Log("Controle Xbox detectado: " + deviceName);
            }
            else if (deviceName.Contains("XInput") || deviceName.Contains("Xbox"))
            {
                // Controle de Xbox
                tutorialImage.sprite = xboxTutorial;
                Debug.Log("Controle Playstation detectado: " + deviceName);
            }
        }
        else
        {
            // Nenhum controle conectado, mostrar tutorial de teclado
            tutorialImage.sprite = keyboardTutorial;
            Debug.Log("Nenhum controle detectado. Exibindo tutorial para teclado.");
        }
    }

    void HideTutorial()
    {
        tutorialImage.gameObject.SetActive(false);
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
