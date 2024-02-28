using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ������ �� ��ȯ �� ���� ������ �����ϴ� ��. 
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void MoveTitle()
    {
        Debug.Log("ȣ��Ǿ���.");
        SceneManager.LoadScene("TitleScene");
    }

    public void MoveSinglePlay()
    {
        SceneManager.LoadScene("MainScene"); 
    }

    public void MoveBattlePlay()
    {
        SceneManager.LoadScene("BattleScene2");
    }

    public void MoveQuit()
    {
        Application.Quit(); // ���� ���� ���� ���� ����. 
    }
}
