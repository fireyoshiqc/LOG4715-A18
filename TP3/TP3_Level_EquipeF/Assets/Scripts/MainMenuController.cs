using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float elapsedTime;
    public float fadeTime = 0.0f;
    private bool fading = true;

    // Use this for initialization
    void Start()
    {
        AudioListener.volume = PlayerPrefs.HasKey("volume") ? PlayerPrefs.GetFloat("volume") : 1.0f;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        elapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeTime);
        }
        else if (fading)
        {
            fading = false;
            canvasGroup.alpha = 1.0f;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game from the menu.");
        Application.Quit();
    }

}
