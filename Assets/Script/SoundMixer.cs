using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class SoundMixer : MonoBehaviour
{
    public AudioMixer mixer;

    [Range(-80, 0)]
    public float master = 0;

    [Range(-80, 0)]
    public float bgm = 0;

    [Range(-80, 0)]
    public float drop = 0;

    [Range(-80, 0)]
    public float line = 0;

    public void MixerControl()
    {
        mixer.SetFloat(nameof(master), master);
        mixer.SetFloat(nameof(bgm), bgm);
        mixer.SetFloat(nameof(drop), drop);
        mixer.SetFloat(nameof(line), line);
    }

    // Update is called once per frame
    void Update()
    {
        MixerControl(); 
    }
}
