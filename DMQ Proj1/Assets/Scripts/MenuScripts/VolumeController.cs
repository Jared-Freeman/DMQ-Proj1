using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mixerMaster;
    public AudioMixer mixerMusic;
    public AudioMixer mixerSFX;
    public void SetMasterVolumeLevel(float masterSliderValue)
    {
        mixerMaster.SetFloat("MasterVolume", Mathf.Log10(masterSliderValue) * 25);
    }
    public void setMusicVolumeLevel(float musicSliderValue)
    {
        mixerMusic.SetFloat("MusicVolume", Mathf.Log10(musicSliderValue) * 25);
    }
    public void setSFXVolumeLevel(float SFXSliderValue)
    {
        mixerSFX.SetFloat("SFXVolume", Mathf.Log10(SFXSliderValue) * 25);
    }
}
