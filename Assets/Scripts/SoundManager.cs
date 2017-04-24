using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    
    public Sprite spriteOn;
    public Sprite spriteOff;
    public Image soundButtonImage;
    private AudioSource[] sourcesAudio;
    private AudioSource musicAudio;
    private AudioSource fxAudio;
    private AudioSource chibiSound;

    void Start () {
        sourcesAudio = FindObjectsOfType<AudioSource>();
        musicAudio = transform.FindChild("MusicPlayer").GetComponent<AudioSource>();
        fxAudio = transform.FindChild("FXPlayer").GetComponent<AudioSource>();
        chibiSound = transform.FindChild("ChibiSound").GetComponent<AudioSource>();
        InitVolumes();
    }

    public void InitVolumes()
    {
        musicAudio.volume = OptionManager.optionsInstance.volMusicsound;
        fxAudio.volume = OptionManager.optionsInstance.volFXsound;
        chibiSound.volume = fxAudio.volume;
    }

    public void ToggleOnOff()
    {
        if (soundButtonImage.sprite == spriteOff)
        {
            soundButtonImage.sprite = spriteOn;
            foreach (AudioSource sourceAudio in sourcesAudio)
            {
                sourceAudio.mute = false;
            }

        }
        else
        {
            soundButtonImage.sprite = spriteOff;
            foreach (AudioSource sourceAudio in sourcesAudio)
            {
                sourceAudio.mute = true;
            }
        }
    }


    public void SetMusicVolume(Slider m_slider) {
        OptionManager.optionsInstance.volMusicsound = m_slider.value;
        musicAudio.volume = m_slider.value;
    }

    public void SetFXVolume(Slider m_slider) {
        OptionManager.optionsInstance.volFXsound = m_slider.value;
        fxAudio.volume = m_slider.value;
        if (m_slider.value != PlayerPrefs.GetFloat("FXVol", 0.8f)) { //Pour ne jouer le son que si la valeur du slider change. Permet de ne pas jouer le son au lancmeent de la scène.
            PlayRandomFX();
        }
    }

    void PlayRandomFX()
    {
        fxAudio.Stop();
        fxAudio.Play();
    }

    public void PlayFXSound(AudioClip audioToPlay) {
        fxAudio.PlayOneShot(audioToPlay);
    }

}
