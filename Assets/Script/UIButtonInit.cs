using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonInit : MonoBehaviour, ISelectHandler
{
    [SerializeField]EUIbutton buttonID;

    Button button; 
    public AudioSource audioSource;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        switch (buttonID) 
        {
            case EUIbutton.Single:
                button.onClick.AddListener(GameSceneManager.instance.MoveSinglePlay);
                break;

            case EUIbutton.Battle:
                button.onClick.AddListener(GameSceneManager.instance.MoveBattlePlay);
                break;

            case EUIbutton.Quit:
                button.onClick.AddListener(GameSceneManager.instance.MoveQuit);
                break;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        audioSource.Play();
    }

}
