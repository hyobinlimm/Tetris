using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 게임의 전반적인 로직과 그리기를 담당하는 클래스. 

// 테트리스의 보드를 생성 및 초기화를 해주는 곳.
public class BattleGameManager : MonoBehaviour
{
    public List<Material> Materials;    // 머터리얼을 담은 배열
    public List<Sprite> SpriteList;     // 테트로미노 텍스처를 담은 배열

    // 보드에 관련된 게임 오프젝트
    public Block EmptyPrefab;
    Tetromino Tetromino;

    // 플레이어의 프리팹을 담을 변수 
    GameObject Prefab_Player1;
    GameObject Prefab_Player2;
    
    // 플레이어 객체 
    CPlayer Player1;
    CPlayer Player2;

    public int[] Attacks; // 0은 player1 공격, 1은 player2 공격.  

    // UI 관련 변수들 
    GameObject UI_Result;
    GameObject UI_Pause;

    bool IsGameOver = false;

    private bool Button_Again = false; 
    private bool Button_Pause = false; 

    private void Awake()
    {
        Debug.Log("battle manager 의 Awake");

        Attacks = new int[2] { 0, 0};

        Tetromino = GetComponent<Tetromino>();

        Prefab_Player1 = new GameObject("Prefab_Player1");
        Prefab_Player1.transform.position = new Vector3(-7.85f, -5.5f, 0.0f);

        Prefab_Player2 = new GameObject("Prefab_Player2");
        Prefab_Player2.transform.position = new Vector3(1.9f, -5.5f, 0.0f);

        Prefab_Player1.AddComponent<Board>();
        Prefab_Player2.AddComponent<Board>();

        Prefab_Player1.AddComponent<CRenderer>();
        Prefab_Player2.AddComponent<CRenderer>();

        Prefab_Player1.AddComponent<CPlayer>();
        Prefab_Player2.AddComponent<CPlayer>();

        Player1 = Prefab_Player1.GetComponent<CPlayer>();
        Player2 = Prefab_Player2.GetComponent<CPlayer>();

        UI_Result = GameObject.Find("UI_Result");
        UI_Pause = GameObject.Find("UI_pause");
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("battle manager 의 start");

        Application.targetFrameRate = 60;   // 60 프레임

        SoundManager.Instance.PlaySound("02. Game Theme");

        UI_Result.SetActive(false);
        UI_Pause.SetActive(false);

        Player1.Initialized(EPlayer.Player1, Prefab_Player1, new Vector3(-8.31f, 3.36f, 0f), new Vector3(-1.75f, 3.35f, 0f));
        Player2.Initialized(EPlayer.Player2, Prefab_Player2, new Vector3(1.37f, 3.36f, 0f), new Vector3(7.97f, 3.35f, 0f));

        SetTetrominoPack(); // 테트로미노를 플레이어들에게 넣어줌. 

        Player1.SetTetromino(); 
        Player2.SetTetromino(); 
    }
   
    // Update is called once per frame
    private void Update()
    {
        if(KeyInput()) // ESC 가 눌렸다 
        {
            // 정지시키고 UI 화면 보여줌. 
            ButtonContinue();
        }

        // 정지상태라면return; 아무것도 동작하지 않도록 하기 위한 코드. 
        if (Button_Pause) return; 

        if (Button_Again) // 다시 시작 버튼이 눌렸다면 data board 및 정보 초기화 
        {
            SoundManager.Instance.PlaySound("02. Game Theme");

            Button_Again = false;
            
            IsGameOver = false; 

            UI_Result.SetActive(false);

            Attacks[0] = 0; 
            Attacks[1] = 0;

            Player1.DataInitialize();
            Player2.DataInitialize();

            SetTetrominoPack(); // 테트로미노를 플레이어들에게 넣어줌. 

            Player1.SetTetromino();
            Player2.SetTetromino();

            return;
        }

        // preView 의 테트로미노까지 생각해서 5개 이하로 남았을 때, 테트로미노를 넣어줌. 
        if (Player1.TetrominoPacks.Count <= 5 || Player2.TetrominoPacks.Count <= 5)
        {
            SetTetrominoPack(); // 모든 플레이어들에게 팩을 넣어줌.
        }

        var _result_Player1 = Player1.CheckDeadLine();

        if (_result_Player1 == true && Player2.CheckDeadLine() == true)
        {
            Player1.PlayerUpdate(Attacks[1]);
            Player2.PlayerUpdate(Attacks[0]);
        }
        else 
        {
            if (IsGameOver == true) return;

            SoundManager.Instance.Stop("02. Game Theme");

            IsGameOver = true; // 게임 오버 상태일 때 한번만 동작. 

            UI_Result.SetActive(true);

            var button = UI_Result.transform.GetChild(2).GetComponent<Button>();
            button.Select();

            if (_result_Player1)
            {
                UI_Result.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                UI_Result.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                UI_Result.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                UI_Result.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
            }
            else 
            {
                UI_Result.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                UI_Result.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                UI_Result.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                UI_Result.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        // 제거 된 줄이 있다면 저장.
        StoreAttackLine(0, Player1.AttactLine());  
        StoreAttackLine(1, Player2.AttactLine()); 
    }

    private void SetTetrominoPack()
    {
        var temp = Tetromino.GetTetrominoPack(); 

        for(int i = 0; i < temp.Count; i++) 
        {
            Player1.TetrominoPacks.Add(temp[i]);
            Player2.TetrominoPacks.Add(temp[i]);
        }
    }

    void StoreAttackLine(int _storeIndex, int _attackLine)
    {
        if (0 < _attackLine)
        {
            Attacks[_storeIndex] = _attackLine;
        }
        else { Attacks[_storeIndex] = 0; }
    }

    public void ButtonContinue()
    {
        if (!Button_Pause) // 눌렸다. 
        {
            SoundManager.Instance.Pause("02. Game Theme");

            Button_Pause = true;
            UI_Pause.SetActive(true);

            var button = UI_Pause.transform.GetChild(0).GetComponent<Button>();
            button.Select();

            UI_Pause.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(PushContinue); 
            UI_Pause.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GameSceneManager.instance.MoveTitle);
        }
    }

    void PushContinue()
    {
        SoundManager.Instance.PlaySound("02. Game Theme");

        Button_Pause = false;
        UI_Pause.SetActive(false);
    }

    // Button OnClick으로 불러주는 함수들. 
    public void Again()
    {
        Button_Again = true;
    }

    public void ButtonTitle()
    {
        GameSceneManager.instance.MoveTitle(); 
    }

    bool KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          return true;
        }

        return false;
    }
}

