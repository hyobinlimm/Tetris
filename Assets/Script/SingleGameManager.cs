using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 게임의 전반적인 로직과 그리기를 담당하는 클래스. 

// 테트리스의 보드를 생성 및 초기화를 해주는 곳.
public class SingleGameManager : MonoBehaviour
{
    readonly int Board_Min = 2;
    readonly int Board_Max = 11;

    // 게임 보드 배열
    Block[,] ObjectBoard; // 실제 그려지는 오브젝트를 관리하는 배열
    Block[,] HoldBoard; // hold 된 테트로미노를 보여주는 배열
    List<Block[,]> PreViewBoard; // hold 된 테트로미노를 보여주는 배열

    // 현재 좌표 
    public int PosY;
    public int PosX;

    // 스프라이트의 사이즈를 저장할 변수
    float SpriteWidth;
    float SpriteHeight;

    public List<Material> Materials;    // 머터리얼을 담은 배열
    public List<Sprite> SpriteList;     // 테트로미노 텍스처를 담은 배열

    // 게임 오브젝트들 
    // 보드에 관련된 게임 오프젝트
    public Block EmptyPrefab;
    GameObject BoardPrefab; // 런타임 중 생성된 wall, empty 를 담은 곳. 
    GameObject HoldPrefab; // 런타임 중 생성된 테트리스 보드 담는 곳. 
    GameObject Obj_GameOver;
    GameObject Obj_Pause;

    List<GameObject> PreViewFolders;

    // 시간 관련된 변수들 
    float Timer = 0.0f;
    bool Term = false; // 한텀에 홀드 한번만

    private bool Button_Again = false; // again 이 눌렸다면 초기화 
    private bool Button_Pause = false; // esc 가 눌렸다면

    bool IsGameOver = false;
    bool PauseOut = false;

    // 테트리미노 객체
    public Tetromino Tetromino;
    public Board GameBoard; // 데이터를 담고 있는 보드를 관리하는 클래스의 인스턴스

    // 화면에 나올 테트로미노를 관리하는 배열
    List<int> TetrominoPack;
    int NowTetrominoID = 0; // 테트로미노의 ID.
    int HoldTetrominoID = -1; // 홀드된 테트로미노의 ID. 
    int[,] NowTetromino;

    int TetroDegree = 0; // 현재 테트로미노의 각도 90 : 0 > 1 / 180 : 1 > 2 / 270 : 2 > 3 / 360 : 3 > 0

    // key Input 관련
    KeyCode NowKeyInput;
    KeyCode OldKeyInput;
    float deltaTime = 0.0f;

    // 점수 
    int TetrisScore = 0;
    public TextMeshProUGUI ScoreText;

    float TetrisSpeed = Define.WaitTime;

    // 스크립트 인스턴스의 수명 동안 한번만 호출.
    // 호출 순서가 정해져있지 않다. 
    // 생성자와 비슷하다고 생각하면 될 듯.
    // 소멸자는 필요 없음.(가비지 컬렉터가 해줌.)
    private void Awake()
    {
        ObjectBoard = new Block[Define.Height, Define.Width];
        HoldBoard = new Block[Define.Max4x4, Define.Max4x4];

        PreViewBoard = new List<Block[,]>();

        // 스프라이트 사이즈 구하기 
        SpriteHeight = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        SpriteWidth = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        Tetromino = gameObject.GetComponent<Tetromino>(); 

        TetrominoPack = new List<int>(); // 랜덤 테트로미노를 관리할 리스트. 
        PreViewFolders = new List<GameObject>();

        Obj_GameOver = GameObject.Find("UI_again/title");
        Obj_Pause = GameObject.Find("UI_pause");

        ScoreText = GameObject.Find("UI_Score").GetComponent<TextMeshProUGUI>();
        ScoreText.text = "초기화한당";
    }

    // Start is called before the first frame update
    private void Start()
    {
        UnityEngine.Application.targetFrameRate = 60;   // 60 프레임

        Obj_GameOver.SetActive(false);
        Obj_Pause.SetActive(false);

        GameBoard.InitializeBoard();
        CreateRanderBoard();

        CreateHoldTetromino();

        CreatePreView();

        SetTetrominoPack(); // 테트로미노를 플레이어들에게 넣어줌. 

        SetTeroimino();
    }

    public void Initialized()
    {
        GameBoard.InitializeBoard(); // data Board 초기화 

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

        SetTetrominoPack(); // 테트로미노를 플레이어들에게 넣어줌. 

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

            Button_Again = false; // true 가 되서 들어왔으니 다시 false 바꿔줌. 

            IsGameOver = false;

            return; // 스페이스를 keyinput 에 안 먹히도록 하기 위해. 
        }

        if (CheckDeadLine())
        {
            SpeedUp();

            if (TetrominoPack.Count == 5)
            {
                SetTetrominoPack(); // 모든 플레이어들에게 팩을 넣어줌.
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

                Term = false; // 굳은 테트로미노가 있으면 한 턴이 끝난 것임. 
            }

            if (CheckDirection(PosY - 1, PosX)) // 밑으로 내리는 코드 
            {
                if (Timer > TetrisSpeed)
                {
                    --PosY;

                    Timer -= TetrisSpeed;
                }
            }

            ShowScore(); // 점수를 화면에 띄우는 함수. 
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

    // 어떤 색으로 그릴지 정보를 담는 보드 초기화 
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

                // 폴더 하이어라키 구조 만드는 다른 방법.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    }

    // 현재 테트로미노 설정.  
    private void SetTeroimino()
    {
        // 랜덤으로 테트리미노를 뽑는다.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        TetroDegree = 0; // 맨 처음 상태 

        NowTetrominoID = TetrominoPack[0];
        TetrominoPack.RemoveAt(0); // 사용된 테트로미노는 팩에서 제거. 

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);

        if (!CheckDirection(PosY - 1, PosX)) // 시작 위치에 굳은 테트로미노가 있으면 위로 한칸 올려주자. 
        {
            ++PosY;
        }
    }

    // 홀드에서 꺼낸 테트로미노 설정
    private void PopHoldTetromino()
    {
        // 위치를 초기화해서 현재 테트로미노를 그리게 한다. 
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);
    }

    // 보드에 그려라. 
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
        // GetKey : 식별된 키가 눌렸을때
        // GetKeyDown : 식별된 키를 누르고 있을 때 true 반환
        // GetKeyUp : 식별된 키를 눌렀다 떼었을때 true 반환 

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

            deltaTime = 0; // 첫 키에는 초기화 

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && OldKeyInput == NowKeyInput)
        {
            deltaTime += Time.deltaTime; // 계속 눌려있다면 deltatime를 받아서 더해준다. 

            if (deltaTime > Define.conditionTime - 0.02f) // deltatime 값이 조건시간을 초과하면 아래 코드 진행. 
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
                Term = true; // 이번 턴에 홀드를 사용했다. 
                HaveToHoldTateromino();

                PopHoldTetromino();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 정지시키고 UI 화면 보여줌. 
            ButtonContinue();
        }

    }

    // 벽 뚫기 막기
    private bool CheckDirection(int _posY, int _posX)
    {
        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if (NowTetromino[y, x] == 0) continue;

                if (_posY - y < 2) return false; // 바닥을 만났을때 실패반환

                if (_posX + x < 2 || _posX + x > 11) return false; // 좌 우 에 있는 벽을 만났을때 실패 반환

                var _dataBoard = GameBoard.GetBoard();

                if (0 < _dataBoard[_posY - y, _posX + x] && _dataBoard[_posY - y, _posX + x] < 8) return false; // 밑에 굳은 테트로미노 검사
                // y + 1 를 해준 이유. 현재 좌표의 밑에 굳은 테트로미노가 있는지 검사 하기 위해. 
            }
        }
        return true;
    }

    // 2차원 배열 90도 회전 시키기
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

        if (PositionOffset(PosY, PosX, tempArr)) // 회전으로 벽을 뚫었다면 오프셋 만큼 올려주자. 
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

        if (PositionOffset(PosY, PosX, tempArr)) // 회전으로 벽을 뚫었다면 오프셋 만큼 올려주자. 
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

                if (_posX + x < Board_Min) // 왼쪽, 오른쪽 벽 검사. 
                {
                    if (_posX + x < _offset_X) continue;
                    _offset_X = Board_Min - (_posX + x);
                }
                else if (Board_Max < _posX + x)
                {
                    if (_posX + x < _offset_X) continue;
                    _offset_X = Board_Max - (_posX + x);
                }

                if (_posY - y < 2) // 바닥 뚫으면? 
                {
                    if (_offset_Y < (_posY - y)) _offset_Y = (_posY - y);
                }

                // 굳은 테트로미노를 만나면 회전 금지. 
                if ((int)ETetrominoColor.Empty < _dataBoard[_posY - y + _offset_Y, _posX + x + _offset_X] && _dataBoard[_posY - y + _offset_Y, _posX + x + _offset_X] <= (int)ETetrominoColor.Gray)
                {
                    return false; // 다른 오브젝트와 만나면 회전 금지.
                }
            }
        }

        // 구한 오프셋을 포지션 값에 더한다. 
        PosX += _offset_X;
        PosY += _offset_Y;

        return true;
    }

    private bool Spin_RightRotate(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O 모양은 검사 x; 

        for (int i = 0; i < 4; i++)
        {
            // 반시계 반향으로 회전하기 때문에 -1를 곱해줘야함. 
            var _offsetX = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

            Debug.LogWarning("라이트 스핀" + TetroDegree);
            Debug.LogWarning("right spin Offset " + _offsetX + " " + _offsetY);

            if (CheckSpin(_posY, _posX, _offsetY, _offsetX, _dataBoard, _rotateArr)) // true 가 있다면
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

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O 모양은 검사 x; 

        var _degree = 0;

        if (TetroDegree <= 0) { _degree = 3; }
        else { _degree = TetroDegree - 1; }

        for (int i = 0; i < 4; i++)
        {
            // 반시계 반향으로 회전하기 때문에 -1를 곱해줘야함. 
            var _offsetX = -1 * Define.Spin_Offset[NowTetrominoID, _degree, i, 0];
            var _offsetY = -1 * Define.Spin_Offset[NowTetrominoID, _degree, i, 1];

            Debug.LogError("래프트 스핀 " + TetroDegree);
            Debug.LogWarning("left spin Offset " + _offsetX + " " + _offsetY);

            if (CheckSpin(_posY, _posX, _offsetY, _offsetX, _dataBoard, _rotateArr)) // true 가 있다면
            {
                PosX += _offsetX;
                PosY += _offsetY;

                return true;
            }
        }

        return false;
    }

    private bool CheckSpin(int _posY, int _posX, int _offsetY, int _offsetX, int[,] _dataBoard, int[,] _rotateArr)
    { // spin 가능? 

        var _max = NowTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if (_rotateArr[y, x] == 0) continue; // 빈 곳은 검사 x 

                // 현재 테트로미노가 놓을 자리에 값이 있다면 false;
                if (_dataBoard[_posY - y + _offsetY, _posX + x + _offsetX] != 0) return false; // 다른 오브젝트와 만나면 회전 금지.
            }
        }

        return true;
    }

    // 오브젝트 배열에서 현재 테트로미노를 그린다. 
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

    // 현재 테트로미노가 바닥에 놓을 위치 보여주는 것. 
    private void DrawGhostTetromino()
    {
        int _tempY = PosY;

        while (CheckDirection(_tempY, PosX))
        {
            --_tempY;
        } // 바닥에 닿을 때까지 

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

    private void CreatePreView() // 나올 테트로미노를 그려줄 보드를 만드는 함수. 
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
            PreViewBoard.Add(_board); // 만든 보드를 추가해서 관리. 
        }
    }

    private void Board4x4Initialize(Block[,] _board) // 4x4 배열을 초기화 해주는 함수. 
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
        else // o 모양일 때
        {
            HoldPrefab.transform.position = new Vector3(-0.7f, 8.81f, 0.0f);
        }
    }

    private void DrawHoldTetromino()
    {
        Board4x4Initialize(HoldBoard); // 혹시 그려져 있을 보드 초기화. 

        if (HoldTetrominoID == -1) return; // 현재 홀드에 아무것도 안들어왔있다는 의미. 그려줄 필요 X

        // 3x3배열과 4x4 배열일 때 위치값을 바꿔준다. 
        HoldPositioning();

        var _holdTetromino = Tetromino.GetTetromino(HoldTetrominoID);

        if (_holdTetromino == null) return; // 아직 홀드에 아무것도 없다면 나가자. 

        var _max = HoldTetrominoID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int posY = (_max - 1) - y; // 보드를 좌상단에서부터 그리게 해도록 하는 좌표 

                if ((ETetrominoColor)_holdTetromino[y, x] == 0) continue;

                var _spriteEnabled = HoldBoard[posY, x].GetComponent<SpriteRenderer>(); // 스프라이트를 켜줄 변수. 

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
        else // o 모양일 때
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

            PreViewPositioning(i, _tetroID); // 테트리미노 모양에 다른 위치 변경.

            Board4x4Initialize(PreViewBoard[i]);

            var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

            for (int y = 0; y < _max; ++y)
            {
                for (int x = 0; x < _max; ++x)
                {
                    if ((ETetrominoColor)_preVeiwTetro[y, x] == 0) continue;

                    int _posY = (_max - 1) - y; // 좌상단에서부터 그려주기 위한 포지션. 

                    var _spriteEnabled = PreViewBoard[i][_posY, x].GetComponent<SpriteRenderer>(); // 스프라이트를 켜줄 변수. 

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
        // 첫 홀드 시, 현재 테트로미노를 홀드해놓고 새 테트로미노를 꺼낸다. 
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
            // dataBoard의 dead line 줄만 검사함. 그 부분에 값이 들어있다면 죽음. 
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
        if (!Button_Pause) // 눌렸다. 
        {
            SoundManager.Instance.Pause("02. Game Theme");

            Button_Pause = true;
            Obj_Pause.SetActive(true);

            var button = Obj_Pause.transform.GetChild(0).GetComponent<Button>();
            button.Select();

            // 0 : continue 버튼이 눌렸을 때 함수 호출 / 1: Title 버튼이 눌렸을 때 함수 호출
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

