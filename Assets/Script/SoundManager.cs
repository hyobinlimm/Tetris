using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    Dictionary<string, AudioClip> SoundClipDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Debug.Log("SoundManager Awake");
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        var _clipList = Resources.LoadAll<AudioClip>("Sound");

        foreach(var _clip in _clipList) 
        {
            string _name = _clip.name;

            SoundClipDictionary.Add(_name, _clip);

            Debug.Log("SoundDictionary 에 집어넣는 중" + _clip.name);
        }
    }

    public void PlaySound(string _name)
    {
        var audioSourceArr = Camera.main.GetComponents<AudioSource>();

        switch(_name)
        {
            case "02. Game Theme":
                {
                    var SFXsource = audioSourceArr[0];
                    SFXsource.Play();
                }
                break;

            case "drop":
                {
                    var SFXsource = audioSourceArr[1];
                    SFXsource.PlayOneShot(SoundClipDictionary[_name]);
                }
                break;

            case "line":
                {
                    var SFXsource = audioSourceArr[2];
                    SFXsource.PlayOneShot(SoundClipDictionary[_name]);
                }
                break;

            case "button":
                {
                    var SFXsource = audioSourceArr[3];
                    SFXsource.PlayOneShot(SoundClipDictionary[_name]);
                }
                break;

        }
    }

    public void Pause(string _name)
    {
        var audioSourceArr = Camera.main.GetComponents<AudioSource>();

        switch (_name)
        {
            case "02. Game Theme":
                {
                    var SFXsource = audioSourceArr[0];
                    SFXsource.Pause();
                }
                break;

            case "drop":
                {
                    var SFXsource = audioSourceArr[1];
                    SFXsource.Pause();
                }
                break;

            case "line":
                {
                    var SFXsource = audioSourceArr[2];
                    SFXsource.Stop();
                }
                break;

            case "button":
                {
                    var SFXsource = audioSourceArr[3];
                    SFXsource.Stop();
                }
                break;

        }
    }

    public void Stop(string _name)
    {
        var audioSourceArr = Camera.main.GetComponents<AudioSource>();

        switch (_name)
        {
            case "02. Game Theme":
                {
                    var SFXsource = audioSourceArr[0];
                    SFXsource.Stop();
                }
                break;
        }
    }
}
