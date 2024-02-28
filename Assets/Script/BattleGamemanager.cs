using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ������ �������� ������ �׸��⸦ ����ϴ� Ŭ����. 

// ��Ʈ������ ���带 ���� �� �ʱ�ȭ�� ���ִ� ��.
public class BattleGameManager : MonoBehaviour
{
    public List<Material> Materials;    // ���͸����� ���� �迭
    public List<Sprite> SpriteList;     // ��Ʈ�ι̳� �ؽ�ó�� ���� �迭

    // ���忡 ���õ� ���� ������Ʈ
    public Block EmptyPrefab;
    Tetromino Tetromino;

    // �÷��̾��� �������� ���� ���� 
    GameObject Prefab_Player1;
    GameObject Prefab_Player2;
    
    // �÷��̾� ��ü 
    CPlayer Player1;
    CPlayer Player2;

    public int[] Attacks; // 0�� player1 ����, 1�� player2 ����.  

    // UI ���� ������ 
    GameObject UI_Result;
    GameObject UI_Pause;

    bool IsGameOver = false;

    private bool Button_Again = false; 
    private bool Button_Pause = false; 

    private void Awake()
    {
        Debug.Log("battle manager �� Awake");

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
        Debug.Log("battle manager �� start");

        Application.targetFrameRate = 60;   // 60 ������

        SoundManager.Instance.PlaySound("02. Game Theme");

        UI_Result.SetActive(false);
        UI_Pause.SetActive(false);

        Player1.Initialized(EPlayer.Player1, Prefab_Player1, new Vector3(-8.31f, 3.36f, 0f), new Vector3(-1.75f, 3.35f, 0f));
        Player2.Initialized(EPlayer.Player2, Prefab_Player2, new Vector3(1.37f, 3.36f, 0f), new Vector3(7.97f, 3.35f, 0f));

        SetTetrominoPack(); // ��Ʈ�ι̳븦 �÷��̾�鿡�� �־���. 

        Player1.SetTetromino(); 
        Player2.SetTetromino(); 
    }
   
    // Update is called once per frame
    private void Update()
    {
        if(KeyInput()) // ESC �� ���ȴ� 
        {
            // ������Ű�� UI ȭ�� ������. 
            ButtonContinue();
        }

        // �������¶��return; �ƹ��͵� �������� �ʵ��� �ϱ� ���� �ڵ�. 
        if (Button_Pause) return; 

        if (Button_Again) // �ٽ� ���� ��ư�� ���ȴٸ� data board �� ���� �ʱ�ȭ 
        {
            SoundManager.Instance.PlaySound("02. Game Theme");

            Button_Again = false;
            
            IsGameOver = false; 

            UI_Result.SetActive(false);

            Attacks[0] = 0; 
            Attacks[1] = 0;

            Player1.DataInitialize();
            Player2.DataInitialize();

            SetTetrominoPack(); // ��Ʈ�ι̳븦 �÷��̾�鿡�� �־���. 

            Player1.SetTetromino();
            Player2.SetTetromino();

            return;
        }

        // preView �� ��Ʈ�ι̳���� �����ؼ� 5�� ���Ϸ� ������ ��, ��Ʈ�ι̳븦 �־���. 
        if (Player1.TetrominoPacks.Count <= 5 || Player2.TetrominoPacks.Count <= 5)
        {
            SetTetrominoPack(); // ��� �÷��̾�鿡�� ���� �־���.
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

            IsGameOver = true; // ���� ���� ������ �� �ѹ��� ����. 

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

        // ���� �� ���� �ִٸ� ����.
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
        if (!Button_Pause) // ���ȴ�. 
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

    // Button OnClick���� �ҷ��ִ� �Լ���. 
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

