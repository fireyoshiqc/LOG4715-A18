using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsController : MonoBehaviour {

    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

	// Use this for initialization
	void Start () {
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        volumeSlider.value = PlayerPrefs.GetFloat("volume");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ChangeVolume()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
        volumeText.SetText(((int)(volumeSlider.value * 100)).ToString());
    }
}
