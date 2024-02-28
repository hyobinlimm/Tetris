using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 게임의 씬 전환 및 게임 루프가 시작하는 곳. 
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
        Debug.Log("호출되었다.");
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
        Application.Quit(); // 빌드 했을 때만 실행 가능. 
    }
}
