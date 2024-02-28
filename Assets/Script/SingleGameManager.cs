using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ������ �������� ������ �׸��⸦ ����ϴ� Ŭ����. 

// ��Ʈ������ ���带 ���� �� �ʱ�ȭ�� ���ִ� ��.
public class SingleGameManager : MonoBehaviour
{
    readonly int Board_Min = 2;
    readonly int Board_Max = 11;

    // ���� ���� �迭
    Block[,] ObjectBoard; // ���� �׷����� ������Ʈ�� �����ϴ� �迭
    Block[,] HoldBoard; // hold �� ��Ʈ�ι̳븦 �����ִ� �迭
    List<Block[,]> PreViewBoard; // hold �� ��Ʈ�ι̳븦 �����ִ� �迭

    // ���� ��ǥ 
    public int PosY;
    public int PosX;

    // ��������Ʈ�� ����� ������ ����
    float SpriteWidth;
    float SpriteHeight;

    public List<Material> Materials;    // ���͸����� ���� �迭
    public List<Sprite> SpriteList;     // ��Ʈ�ι̳� �ؽ�ó�� ���� �迭

    // ���� ������Ʈ�� 
    // ���忡 ���õ� ���� ������Ʈ
    public Block EmptyPrefab;
    GameObject BoardPrefab; // ��Ÿ�� �� ������ wall, empty �� ���� ��. 
    GameObject HoldPrefab; // ��Ÿ�� �� ������ ��Ʈ���� ���� ��� ��. 
    GameObject Obj_GameOver;
    GameObject Obj_Pause;

    List<GameObject> PreViewFolders;

    // �ð� ���õ� ������ 
    float Timer = 0.0f;
    bool Term = false; // ���ҿ� Ȧ�� �ѹ���

    private bool Button_Again = false; // again �� ���ȴٸ� �ʱ�ȭ 
    private bool Button_Pause = false; // esc �� ���ȴٸ�

    bool IsGameOver = false;
    bool PauseOut = false;

    // ��Ʈ���̳� ��ü
    public Tetromino Tetromino;
    public Board GameBoard; // �����͸� ��� �ִ� ���带 �����ϴ� Ŭ������ �ν��Ͻ�

    // ȭ�鿡 ���� ��Ʈ�ι̳븦 �����ϴ� �迭
    List<int> TetrominoPack;
    int NowTetrominoID = 0; // ��Ʈ�ι̳��� ID.
    int HoldTetrominoID = -1; // Ȧ��� ��Ʈ�ι̳��� ID. 
    int[,] NowTetromino;

    int TetroDegree = 0; // ���� ��Ʈ�ι̳��� ���� 90 : 0 > 1 / 180 : 1 > 2 / 270 : 2 > 3 / 360 : 3 > 0

    // key Input ����
    KeyCode NowKeyInput;
    KeyCode OldKeyInput;
    float deltaTime = 0.0f;

    // ���� 
    int TetrisScore = 0;
    public TextMeshProUGUI ScoreText;

    float TetrisSpeed = Define.WaitTime;

    // ��ũ��Ʈ �ν��Ͻ��� ���� ���� �ѹ��� ȣ��.
    // ȣ�� ������ ���������� �ʴ�. 
    // �����ڿ� ����ϴٰ� �����ϸ� �� ��.
    // �Ҹ��ڴ� �ʿ� ����.(������ �÷��Ͱ� ����.)
    private void Awake()
    {
        ObjectBoard = new Block[Define.Height, Define.Width];
        HoldBoard = new Block[Define.Max4x4, Define.Max4x4];

        PreViewBoard = new List<Block[,]>();

        // ��������Ʈ ������ ���ϱ� 
        SpriteHeight = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        SpriteWidth = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        Tetromino = gameObject.GetComponent<Tetromino>(); 

        TetrominoPack = new List<int>(); // ���� ��Ʈ�ι̳븦 ������ ����Ʈ. 
        PreViewFolders = new List<GameObject>();

        Obj_GameOver = GameObject.Find("UI_again/title");
        Obj_Pause = GameObject.Find("UI_pause");

        ScoreText = GameObject.Find("UI_Score").GetComponent<TextMeshProUGUI>();
        ScoreText.text = "�ʱ�ȭ�Ѵ�";
    }

    // Start is called before the first frame update
    private void Start()
    {
        UnityEngine.Application.targetFrameRate = 60;   // 60 ������

        Obj_GameOver.SetActive(false);
        Obj_Pause.SetActive(false);

        GameBoard.InitializeBoard();
        CreateRanderBoard();

        CreateHoldTetromino();

        CreatePreView();

        SetTetrominoPack(); // ��Ʈ�ι̳븦 �÷��̾�鿡�� �־���. 

        SetTeroimino();
    }

    public void Initialized()
    {
        GameBoard.InitializeBoard(); // data Board �ʱ�ȭ 

        Timer = 0.0f;
        Term = false;

        deltaTime = 0.0f;
        TetrisSpeed = Define.WaitTime; 

        TetrisScore = 0;

        while (TetrominoPack.Count > 0)
        {
            TetrominoPack.RemoveAt(0);
        }

        NowTetrominoID = 0;
        HoldTetrominoID = -1;

        Obj_GameOver.SetActive(false);

        SetTetrominoPack(); // ��Ʈ�ι̳븦 �÷��̾�鿡�� �־���. 

        SetTeroimino();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Button_Pause)
        {
            return;
        }

        if (PauseOut)
        {
            PauseOut = false;

            return;
        }

        if (Button_Again)
        {
            SoundManager.Instance.PlaySound("02. Game Theme");

            Initialized();

            Button_Again = false; // true �� �Ǽ� �������� �ٽ� false �ٲ���. 

            IsGameOver = false;

            return; // �����̽��� keyinput �� �� �������� �ϱ� ����. 
        }

        if (CheckDeadLine())
        {
            SpeedUp();

            if (TetrominoPack.Count == 5)
            {
                SetTetrominoPack(); // ��� �÷��̾�鿡�� ���� �־���.
            }

            Timer += Time.deltaTime;

            KeyInput();

            GameBoard.LineDelete(ref TetrisScore);
           // Debug.Log("_score : " + TetrisScore);

            GameBoard.DownLine();

            DrawBoard();

            DrawHoldTetromino();

            DrawPreView();

            DrawGhostTetromino();

            DrawTetromino();

            if (GameBoard.Freeze(Timer, PosY, PosX, NowTetrominoID, NowTetromino) == true)
            {
                Timer -= Timer;

                SetTeroimino();

                Term = false; // ���� ��Ʈ�ι̳밡 ������ �� ���� ���� ����. 
            }

            if (CheckDirection(PosY - 1, PosX)) // ������ ������ �ڵ� 
            {
                if (Timer > TetrisSpeed)
                {
                    --PosY;

                    Timer -= TetrisSpeed;
                }
            }

            ShowScore(); // ������ ȭ�鿡 ���� �Լ�. 
        }
        else
        {
            if (IsGameOver == true) return;

            IsGameOver = true;

            SoundManager.Instance.Stop("02. Game Theme");

            Obj_GameOver.SetActive(true);

            var button = Obj_GameOver.transform.GetChild(0).GetComponent<Button>();
            button.Select();

            Obj_GameOver.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GameSceneManager.instance.MoveTitle);
        }


    }

    // � ������ �׸��� ������ ��� ���� �ʱ�ȭ 
    private void CreateRanderBoard()
    {
        BoardPrefab = new GameObject("Board_Folder");
        BoardPrefab.transform.position = new Vector3(-0.03f, 0.0f, 0.0f);

        float _parent_PosX = BoardPrefab.transform.position.x;

        for (int y = 0; y < Define.Height; ++y)
        {
            for (int x = 0; x < Define.Width; ++x)
            {
                if (y <= 1 || (x <= 1 || x >= 12))
                {
                    ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((SpriteWidth * x) + _parent_PosX, SpriteHeight * y, 0), Quaternion.identity, BoardPrefab.transform);
                    ObjectBoard[y, x].GetComponent<SpriteRenderer>().enabled = false;

                    continue;
                }

                ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((SpriteWidth * x) + _parent_PosX, SpriteHeight * y, 0), Quaternion.identity, BoardPrefab.transform);

                // ���� ���̾��Ű ���� ����� �ٸ� ���.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    }

    // ���� ��Ʈ�ι̳� ����.  
    private void SetTeroimino()
    {
        // �������� ��Ʈ���̳븦 �̴´�.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        TetroDegree = 0; // �� ó�� ���� 

        NowTetrominoID = TetrominoPack[0];
        TetrominoPack.RemoveAt(0); // ���� ��Ʈ�ι̳�� �ѿ��� ����. 

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);

        if (!CheckDirection(PosY - 1, PosX)) // ���� ��ġ�� ���� ��Ʈ�ι̳밡 ������ ���� ��ĭ �÷�����. 
        {
            ++PosY;
        }
    }

    // Ȧ�忡�� ���� ��Ʈ�ι̳� ����
    private void PopHoldTetromino()
    {
        // ��ġ�� �ʱ�ȭ�ؼ� ���� ��Ʈ�ι̳븦 �׸��� �Ѵ�. 
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);
    }

    // ���忡 �׷���. 
    private void DrawBoard()
    {
        for (int y = 0; y < Define.Height; ++y)
        {
            for (int x = 0; x < Define.Width; ++x)
            {
                switch ((ETetrominoColor)GameBoard.GetBoard()[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Orange]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Blue]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Green]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Red]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Purple]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Cyan]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Yellow]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Gray]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Empty:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Empty]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void KeyInput()
    {
        // GetKey : �ĺ��� Ű�� ��������
        // GetKeyDown : �ĺ��� Ű�� ������ ���� �� true ��ȯ
        // GetKeyUp : �ĺ��� Ű�� ������ �������� true ��ȯ 

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetTeroimino();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            LeftRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            NowKeyInput = KeyCode.DownArrow;

            if (CheckDirection(PosY - 1, PosX))
            {
                --PosY;
            }

            deltaTime = 0; // ù Ű���� �ʱ�ȭ 

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && OldKeyInput == NowKeyInput)
        {
            deltaTime += Time.deltaTime; // ��� �����ִٸ� deltatime�� �޾Ƽ� �����ش�. 

            if (deltaTime > Define.conditionTime - 0.02f) // deltatime ���� ���ǽð��� �ʰ��ϸ� �Ʒ� �ڵ� ����. 
            {
                if (CheckDirection(PosY - 1, PosX))
                {
                    --PosY;

                    deltaTime = 0;
                }
            }

            OldKeyInput = NowKeyInput;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NowKeyInput = KeyCode.LeftArrow;

            if (CheckDirection(PosY, PosX - 1))
            {
                --PosX;
            }

            deltaTime = 0;

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && OldKeyInput == NowKeyInput)
        {
            deltaTime += Time.deltaTime;

            if (deltaTime > Define.conditionTime)
            {
                if (CheckDirection(PosY, PosX - 1))
                {
                    --PosX;

                    deltaTime = 0;
                }
            }

            OldKeyInput = NowKeyInput;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NowKeyInput = KeyCode.RightArrow;

            deltaTime = 0;

            if (CheckDirection(PosY, PosX + 1))
            {
                ++PosX;
            }

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && OldKeyInput == NowKeyInput)
        {
            deltaTime += Time.deltaTime;

            if (deltaTime > Define.conditionTime)
            {
                if (CheckDirection(PosY, PosX + 1))
                {
                    ++PosX;

                    deltaTime = 0;
                }
            }

            OldKeyInput = NowKeyInput;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (CheckDirection(PosY - 1, PosX))
            {
                --PosY;

                Timer += 1.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Term == false)
            {
                Term = true; // �̹� �Ͽ� Ȧ�带 ����ߴ�. 
                HaveToHoldTateromino();

                PopHoldTetromino();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ������Ű�� UI ȭ�� ������. 
            ButtonContinue();
        }

    }

    // �� �ձ� ����
    private bool CheckDirection(int _posY, int _posX)
    {
        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if (NowTetromino[y, x] == 0) continue;

                if (_posY - y < 2) return false; // �ٴ��� �������� ���й�ȯ

                if (_posX + x < 2 || _posX + x > 11) return false; // �� �� �� �ִ� ���� �������� ���� ��ȯ

                var _dataBoard = GameBoard.GetBoard();

                if (0 < _dataBoard[_posY - y, _posX + x] && _dataBoard[_posY - y, _posX + x] < 8) return false; // �ؿ� ���� ��Ʈ�ι̳� �˻�
                // y + 1 �� ���� ����. ���� ��ǥ�� �ؿ� ���� ��Ʈ�ι̳밡 �ִ��� �˻� �ϱ� ����. 
            }
        }
        return true;
    }

    // 2���� �迭 90�� ȸ�� ��Ű��
    private void RightRotateTetromino()
    {
        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        int[,] tempArr = new int[_max, _max];

        for (int y = 0; y < _max; y++)
        {
            for (int x = 0; x < _max; x++)
            {
                tempArr[y, x] = NowTetromino[_max - x - 1, y];
            }
        }

        if (PositionOffset(PosY, PosX, tempArr)) // ȸ������ ���� �վ��ٸ� ������ ��ŭ �÷�����. 
        {
            NowTetromino = tempArr;

            if (3 <= TetroDegree)
            {
                TetroDegree = 0;
            }
            else { ++TetroDegree; }
        }
        else if (Spin_RightRotate(PosY, PosX, tempArr))
        {
            if (3 <= TetroDegree)
            {
                TetroDegree = 0;
            }
            else { ++TetroDegree; }

            NowTetromino = tempArr;
        }
    }

    private void LeftRotateTetromino()
    {
        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        int[,] tempArr = new int[_max, _max];

        for (int y = 0; y < _max; y++)
        {
            for (int x = 0; x < _max; x++)
            {
                tempArr[y, x] = NowTetromino[x, _max - y - 1];
            }
        }

        var _tempDegree = TetroDegree - 1;

        if (_tempDegree < 0) _tempDegree = 3;

        if (PositionOffset(PosY, PosX, tempArr)) // ȸ������ ���� �վ��ٸ� ������ ��ŭ �÷�����. 
        {
            NowTetromino = tempArr;

            --TetroDegree;

            if (TetroDegree < 0)
            {
                TetroDegree = 3;
            }
        }
        else if (Spin_LeftRotate(PosY, PosX, tempArr))
        {
            --TetroDegree;

            if (TetroDegree < 0)
            {
                TetroDegree = 3;
            }

            NowTetromino = tempArr;
        }
    }


    private bool PositionOffset(int _posY, int _posX, int[,] _rotateArr)
    {
        int _offset_Y = 0;
        int _offset_X = 0;

        var _dataBoard = GameBoard.GetBoard();

        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if (_rotateArr[y, x] == 0) continue;

                if (_posX + x < Board_Min) // ����, ������ �� �˻�. 
                {
                    if (_posX + x < _offset_X) continue;
                    _offset_X = Board_Min - (_posX + x);
                }
                else if (Board_Max < _posX + x)
                {
                    if (_posX + x < _offset_X) continue;
                    _offset_X = Board_Max - (_posX + x);
                }

                if (_posY - y < 2) // �ٴ� ������? 
                {
                    if (_offset_Y < (_posY - y)) _offset_Y = (_posY - y);
                }

                // ���� ��Ʈ�ι̳븦 ������ ȸ�� ����. 
                if ((int)ETetrominoColor.Empty < _dataBoard[_posY - y + _offset_Y, _posX + x + _offset_X] && _dataBoard[_posY - y + _offset_Y, _posX + x + _offset_X] <= (int)ETetrominoColor.Gray)
                {
                    return false; // �ٸ� ������Ʈ�� ������ ȸ�� ����.
                }
            }
        }

        // ���� �������� ������ ���� ���Ѵ�. 
        PosX += _offset_X;
        PosY += _offset_Y;

        return true;
    }

    private bool Spin_RightRotate(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O ����� �˻� x; 

        for (int i = 0; i < 4; i++)
        {
            // �ݽð� �������� ȸ���ϱ� ������ -1�� ���������. 
            var _offsetX = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

            Debug.LogWarning("����Ʈ ����" + TetroDegree);
            Debug.LogWarning("right spin Offset " + _offsetX + " " + _offsetY);

            if (CheckSpin(_posY, _posX, _offsetY, _offsetX, _dataBoard, _rotateArr)) // true �� �ִٸ�
            {
                PosX += _offsetX;
                PosY += _offsetY;

                return true;
            }
        }

        return false;
    }

    private bool Spin_LeftRotate(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O ����� �˻� x; 

        var _degree = 0;

        if (TetroDegree <= 0) { _degree = 3; }
        else { _degree = TetroDegree - 1; }

        for (int i = 0; i < 4; i++)
        {
            // �ݽð� �������� ȸ���ϱ� ������ -1�� ���������. 
            var _offsetX = -1 * Define.Spin_Offset[NowTetrominoID, _degree, i, 0];
            var _offsetY = -1 * Define.Spin_Offset[NowTetrominoID, _degree, i, 1];

            Debug.LogError("����Ʈ ���� " + TetroDegree);
            Debug.LogWarning("left spin Offset " + _offsetX + " " + _offsetY);

            if (CheckSpin(_posY, _posX, _offsetY, _offsetX, _dataBoard, _rotateArr)) // true �� �ִٸ�
            {
                PosX += _offsetX;
                PosY += _offsetY;

                return true;
            }
        }

        return false;
    }

    private bool CheckSpin(int _posY, int _posX, int _offsetY, int _offsetX, int[,] _dataBoard, int[,] _rotateArr)
    { // spin ����? 

        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if (_rotateArr[y, x] == 0) continue; // �� ���� �˻� x 

                // ���� ��Ʈ�ι̳밡 ���� �ڸ��� ���� �ִٸ� false;
                if (_dataBoard[_posY - y + _offsetY, _posX + x + _offsetX] != 0) return false; // �ٸ� ������Ʈ�� ������ ȸ�� ����.
            }
        }

        return true;
    }

    // ������Ʈ �迭���� ���� ��Ʈ�ι̳븦 �׸���. 
    private void DrawTetromino()
    {
        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int posY = PosY - y;
                int posX = PosX + x;

                if ((ETetrominoColor)NowTetromino[y, x] == 0) continue;

                switch ((ETetrominoColor)NowTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Orange));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Blue));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Green));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Red));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Purple));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Gray));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    Sprite GetSprite(ETetrominoColor color)
    {
        int _spriteIndex = (int)color;

        if (_spriteIndex < 0 || SpriteList.Count <= _spriteIndex)
        {
            return SpriteList[0];
        }

        return SpriteList[_spriteIndex];
    }

    // ���� ��Ʈ�ι̳밡 �ٴڿ� ���� ��ġ �����ִ� ��. 
    private void DrawGhostTetromino()
    {
        int _tempY = PosY;

        while (CheckDirection(_tempY, PosX))
        {
            --_tempY;
        } // �ٴڿ� ���� ������ 

        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int _posY = (_tempY + 1) - y;
                int _posX = PosX + x;

                var _nowTetromino = NowTetromino;

                if ((ETetrominoColor)_nowTetromino[y, x] == 0) continue;

                switch ((ETetrominoColor)_nowTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Orange));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Blue));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Green));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Red));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Purple));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[_posY, _posX].SetSprite(GetSprite(ETetrominoColor.Gray));
                        ObjectBoard[_posY, _posX].SetGhostColor();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CreateHoldTetromino()
    {
        HoldPrefab = new GameObject("Hold_Folder");
        HoldPrefab.transform.position = new Vector3(-0.51f, 8.81f, 0.0f);
        HoldPrefab.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        var _height = SpriteHeight * 0.7f;
        var _width = SpriteWidth * 0.7f;

        for (int y = 0; y < Define.Max4x4; ++y)
        {
            for (int x = 0; x < Define.Max4x4; ++x)
            {
                HoldBoard[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((_width * x) + HoldPrefab.transform.position.x, (_height * y) + HoldPrefab.transform.position.y, 0), Quaternion.identity, HoldPrefab.transform);
                HoldBoard[y, x].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void CreatePreView() // ���� ��Ʈ�ι̳븦 �׷��� ���带 ����� �Լ�. 
    {
        for (int i = 0; i < Define.Max; i++)
        {
            Block[,] _board = new Block[Define.Max, Define.Max];

            GameObject PreViewPrefab = new GameObject("PreView_Folder");
            PreViewPrefab.transform.position = new Vector3(6.05f, 8.71f - (i * 1.2f), 0.0f);
            PreViewPrefab.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

            var _height = SpriteHeight * 0.7f;
            var _width = SpriteWidth * 0.7f;

            for (int y = 0; y < Define.Max; ++y)
            {
                for (int x = 0; x < Define.Max; ++x)
                {
                    _board[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((_width * x) + PreViewPrefab.transform.position.x, (_height * y) + PreViewPrefab.transform.position.y, 0), Quaternion.identity, PreViewPrefab.transform);
                    _board[y, x].GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            PreViewFolders.Add(PreViewPrefab);
            PreViewBoard.Add(_board); // ���� ���带 �߰��ؼ� ����. 
        }
    }

    private void Board4x4Initialize(Block[,] _board) // 4x4 �迭�� �ʱ�ȭ ���ִ� �Լ�. 
    {
        for (int y = 0; y < Define.Max; ++y)
        {
            for (int x = 0; x < Define.Max; ++x)
            {
                _board[y, x].SetSprite(GetSprite(ETetrominoColor.Empty));
                _board[y, x].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void HoldPositioning()
    {
        if (HoldTetrominoID < (int)ETetrominoType.I)
        {
            HoldPrefab.transform.position = new Vector3(-0.51f, 8.81f, 0.0f);
        }
        else if (HoldTetrominoID == (int)ETetrominoType.I)
        {
            HoldPrefab.transform.position = new Vector3(-0.7f, 8.66f, 0.0f);
        }
        else // o ����� ��
        {
            HoldPrefab.transform.position = new Vector3(-0.7f, 8.81f, 0.0f);
        }
    }

    private void DrawHoldTetromino()
    {
        Board4x4Initialize(HoldBoard); // Ȥ�� �׷��� ���� ���� �ʱ�ȭ. 

        if (HoldTetrominoID == -1) return; // ���� Ȧ�忡 �ƹ��͵� �ȵ����ִٴ� �ǹ�. �׷��� �ʿ� X

        // 3x3�迭�� 4x4 �迭�� �� ��ġ���� �ٲ��ش�. 
        HoldPositioning();

        var _holdTetromino = Tetromino.GetTetromino(HoldTetrominoID);

        if (_holdTetromino == null) return; // ���� Ȧ�忡 �ƹ��͵� ���ٸ� ������. 

        var _max = HoldTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int posY = (_max - 1) - y; // ���带 �»�ܿ������� �׸��� �ص��� �ϴ� ��ǥ 

                if ((ETetrominoColor)_holdTetromino[y, x] == 0) continue;

                var _spriteEnabled = HoldBoard[posY, x].GetComponent<SpriteRenderer>(); // ��������Ʈ�� ���� ����. 

                switch ((ETetrominoColor)_holdTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Orange));
                        break;
                    case ETetrominoColor.Blue:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Blue));
                        break;
                    case ETetrominoColor.Green:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Green));
                        break;
                    case ETetrominoColor.Red:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Red));
                        break;
                    case ETetrominoColor.Purple:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Purple));
                        break;
                    case ETetrominoColor.Cyan:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        break;
                    case ETetrominoColor.Yellow:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        break;
                    case ETetrominoColor.Gray:
                        _spriteEnabled.enabled = true;
                        HoldBoard[posY, x].SetSprite(GetSprite(ETetrominoColor.Gray));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PreViewPositioning(int _index, int _tetroID)
    {
        var _nowPos = PreViewFolders[_index].transform.position;

        if (_tetroID < (int)ETetrominoType.I)
        {
            PreViewFolders[_index].transform.position = new Vector3(6.05f, _nowPos.y, _nowPos.z);
        }
        else if (_tetroID == (int)ETetrominoType.I)
        {
            PreViewFolders[_index].transform.position = new Vector3(5.86f, _nowPos.y, _nowPos.z);
        }
        else // o ����� ��
        {
            PreViewFolders[_index].transform.position = new Vector3(5.88f, _nowPos.y, _nowPos.z);
        }
    }

    private void DrawPreView()
    {
        for (int i = 0; i < Define.Max; i++)
        {
            var _tetroID = TetrominoPack[i];

            var _preVeiwTetro = Tetromino.GetTetromino(_tetroID);

            PreViewPositioning(i, _tetroID); // ��Ʈ���̳� ��翡 �ٸ� ��ġ ����.

            Board4x4Initialize(PreViewBoard[i]);

            var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

            for (int y = 0; y < _max; ++y)
            {
                for (int x = 0; x < _max; ++x)
                {
                    if ((ETetrominoColor)_preVeiwTetro[y, x] == 0) continue;

                    int _posY = (_max - 1) - y; // �»�ܿ������� �׷��ֱ� ���� ������. 

                    var _spriteEnabled = PreViewBoard[i][_posY, x].GetComponent<SpriteRenderer>(); // ��������Ʈ�� ���� ����. 

                    switch ((ETetrominoColor)_preVeiwTetro[y, x])
                    {
                        case ETetrominoColor.Orange:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Orange));
                            break;
                        case ETetrominoColor.Blue:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Blue));
                            break;
                        case ETetrominoColor.Green:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Green));
                            break;
                        case ETetrominoColor.Red:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Red));
                            break;
                        case ETetrominoColor.Purple:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Purple));
                            break;
                        case ETetrominoColor.Cyan:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Cyan));
                            break;
                        case ETetrominoColor.Yellow:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Yellow));
                            break;
                        case ETetrominoColor.Gray:
                            _spriteEnabled.enabled = true;
                            PreViewBoard[i][_posY, x].SetSprite(GetSprite(ETetrominoColor.Gray));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void HaveToHoldTateromino()
    {
        // ù Ȧ�� ��, ���� ��Ʈ�ι̳븦 Ȧ���س��� �� ��Ʈ�ι̳븦 ������. 
        if (HoldTetrominoID == -1)
        {
            HoldTetrominoID = NowTetrominoID;

            SetTeroimino();
        }
        else
        {
            int _tempArr = NowTetrominoID;
            NowTetrominoID = HoldTetrominoID;
            HoldTetrominoID = _tempArr;
        }
    }

    private bool CheckDeadLine()
    {
        for (int i = Define.Start_X; i < Define.Start_X + 4; ++i)
        {
            // dataBoard�� dead line �ٸ� �˻���. �� �κп� ���� ����ִٸ� ����. 
            if ((int)ETetrominoColor.Empty != GameBoard.GetBoard()[Define.DeadLine, i])
            {
                return false;
            }
        }
        return true;
    }
    private void SetTetrominoPack()
    {
        var temp = Tetromino.GetTetrominoPack();

        for (int i = 0; i < temp.Count; i++)
        {
            TetrominoPack.Add(temp[i]);
        }
    }

    public void Again()
    {
        Button_Again = true;
    }

    public void ButtonContinue()
    {
        if (!Button_Pause) // ���ȴ�. 
        {
            SoundManager.Instance.Pause("02. Game Theme");

            Button_Pause = true;
            Obj_Pause.SetActive(true);

            var button = Obj_Pause.transform.GetChild(0).GetComponent<Button>();
            button.Select();

            // 0 : continue ��ư�� ������ �� �Լ� ȣ�� / 1: Title ��ư�� ������ �� �Լ� ȣ��
            Obj_Pause.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(PushContinue);
            Obj_Pause.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GameSceneManager.instance.MoveTitle);
        }
    }

    void PushContinue()
    {
        SoundManager.Instance.PlaySound("02. Game Theme");

        Button_Pause = false;
        Obj_Pause.SetActive(false);

        PauseOut = true;
    }

    void ShowScore()
    {
        ScoreText.text = TetrisScore.ToString();
    }

    void SpeedUp()
    {
        if(TetrisScore == 12000) { TetrisSpeed = Define.WaitTime - 0.55f; }
        else if(TetrisScore == 8000) { TetrisSpeed = Define.WaitTime - 0.5f; }
        else if(TetrisScore == 4000) { TetrisSpeed = Define.WaitTime - 0.4f; }
        else if(TetrisScore == 2000) { TetrisSpeed = Define.WaitTime - 0.3f; }
        else if(TetrisScore == 500) { TetrisSpeed = Define.WaitTime - 0.2f; }

        Debug.Log(TetrisSpeed);
    }
}

