using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorController : MonoBehaviour
{
    public Image image;                    // O componente Image do elemento de UI
    public float transitionDuration = 3f;  // Duração da transição em segundos
    public Color lightColor = new Color(0f, 0f, 0f, 0f); // Cor clara (RGBA)
    public Color darkColor = new Color(0f, 0f, 0f, 1f);  // Cor escura (RGBA)

    private float transitionTimer;
    private bool isFading;                 // Controla se a transição está ativa
    private bool fadeToDark;               // Controla a direção da transição

    void Start()
    {
        if (image == null)
            image = GetComponent<Image>();

        image.color = lightColor; // Inicia com a cor clara
    }

    void Update()
    {
        if (isFading)
        {
            transitionTimer += Time.deltaTime / transitionDuration;

            // Determina a cor alvo com base na direção da transição
            if (fadeToDark)
                image.color = Color.Lerp(lightColor, darkColor, transitionTimer);
            else
                image.color = Color.Lerp(darkColor, lightColor, transitionTimer);

            // Finaliza a transição e redefine o timer
            if (transitionTimer >= 1f)
            {
                transitionTimer = 0f;
                isFading = false;
            }
        }
    }

    public void FadeToDark()
    {
        isFading = true;
        fadeToDark = true;
        transitionTimer = 0f; // Reinicia o timer ao iniciar a transição
    }

    public void FadeToLight()
    {
        isFading = true;
        fadeToDark = false;
        transitionTimer = 0f; // Reinicia o timer ao iniciar a transição
    }
}
