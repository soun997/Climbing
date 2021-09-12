using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;
    void Awake()
    {
        if (SoundManager.instance == null)
        {
            SoundManager.instance = this;
        }

    }
    #endregion

    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioClip btn_click;
    
    public AudioClip hold_grab;
    public AudioClip Screaming;
    public AudioClip clear;
    public AudioClip fail;

    public Slider BGM_slider;
    public Slider effect_slider;

    public Button BGM_release;
    public Button effect_release;
    public Button BGM_mute;
    public Button effect_mute;

    float BGM;
    float effect;

    public bool isBGMmute;
    public bool isEffectMute;
    

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        BGM_release.gameObject.SetActive(true);
        effect_release.gameObject.SetActive(true);
        BGM_mute.gameObject.SetActive(false);
        effect_mute.gameObject.SetActive(false);
        BGM_slider.value = 0.5f;
        effect_slider.value = 0.5f;

        isBGMmute = false;
        isEffectMute = false;

    }

    // Update is called once per frame
    void Update()
    {
        audioSource1.volume = BGM_slider.value;
        audioSource2.volume = effect_slider.value;

        if (BGM_slider.value != 0)
        {
            BGM_release.gameObject.SetActive(true);
            BGM_mute.gameObject.SetActive(false);
            isBGMmute = false;
        }

        if (effect_slider.value != 0)
        {
            effect_release.gameObject.SetActive(true);
            effect_mute.gameObject.SetActive(false);
            isEffectMute = false;
        }
    }

    public void ButtonClickSound()
    {
        audioSource2.PlayOneShot(btn_click);
    }


    public void GrabSound()
    {
        audioSource2.PlayOneShot(hold_grab);
    }

    public void FallingSound()
    {
        audioSource2.PlayOneShot(Screaming);
    }

    public void Clear()
    {
        audioSource2.PlayOneShot(clear);
    }

    public void Fail()
    {
        audioSource2.PlayOneShot(fail);
    }

    public void BGMMute()
    {
        isBGMmute = true;
        audioSource1.volume = 0f;
        BGM_mute.gameObject.SetActive(true);
        BGM_release.gameObject.SetActive(false);
        BGM = BGM_slider.value;
        BGM_slider.value = audioSource1.volume;
    }

    public void EffectMute()
    {
        isEffectMute = true;
        audioSource2.volume = 0f;
        effect_mute.gameObject.SetActive(true);
        effect_release.gameObject.SetActive(false);
        effect = effect_slider.value;
        effect_slider.value = audioSource2.volume;
    }

    public void ReleaseBGMMute()
    {
        isBGMmute = false;
        BGM_mute.gameObject.SetActive(false);
        BGM_release.gameObject.SetActive(true);
        BGM_slider.value = BGM;
        audioSource1.volume = BGM_slider.value;
    }

    public void ReleaseEffectMute()
    {
        isEffectMute = false;
        effect_mute.gameObject.SetActive(false);
        effect_release.gameObject.SetActive(true);
        effect_slider.value = effect;
        audioSource2.volume = effect_slider.value;
    }

}
