using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeComtroller : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
    }
    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGMVol",Mathf.Log10(value) * 20);
    }
    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
    }
}
