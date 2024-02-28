using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CPlayer : MonoBehaviour
{
    readonly int Board_Min = 2;
    readonly int Board_Max = 11;

    private EPlayer PlayerID;

    public Tetromino Tetromino;
    public Board GameBoard;
    public CRenderer ObjectRenderer;

    // 테트로미노에 관한 변수들
    public List<int> TetrominoPacks; // 테트로미노 팩(테트로미노 7개 묶음)
    private int NowTetrominoID = 0; // 테트로미노의 ID.
    private int HoldTetrominoID = -1; // 홀드된 테트로미노의 ID. 
    private int[,] NowTetromino; // 현재 자신의 테트로미노를 담은 배열. 

    int TetroDegree = 0; // 현재 테트로미노의 각도 90 : 0 > 1 / 180 : 1 > 2 / 270 : 2 > 3 / 360 : 3 > 0

    // 현재 좌표 
    private int PosY;
    private int PosX;

    // 시간 관련 변수 
    private float Timer = 0f;
    private bool Term = false;

    // key Input 관련
    KeyCode NowKeyInput;
    KeyCode OldKeyInput;
    float deltaTime = 0.0f;

    // 점수 
    int TetrisScore = 0;

    public int Pos_Y
    { get { return PosY; } set { PosY = value; } }

    public int Pos_X
    { get { return PosX; } set { PosX = value; } }

    public void Initialized(EPlayer ID, GameObject _prefabFolder, Vector3 _holdPos, Vector3 _preViewPos)
    {
        Debug.Log("Player 의 initialize");

        PlayerID = ID;

        GameBoard = GetComponent<Board>();
        ObjectRenderer = GetComponent<CRenderer>();
        Tetromino = GameObject.Find("BattleGameManager").GetComponent<Tetromino>();

        GameBoard.InitializeBoard();

        // 데이터 보드에 영향을 받을 오브젝트 보드 생성
        ObjectRenderer.CreateRenderBoard(_prefabFolder);
        ObjectRenderer.CreatHoldBaord(_holdPos, "hold"); // 홀드 보드 생성. 
        ObjectRenderer.CreatePreViewBoard(_preViewPos, "PreView");

        // 테트로미노 받아오기 
        TetrominoPacks = new List<int>();
    }

    public void DataInitialize()
    {
        // 저장된 데이터를 날리자. 
        GameBoard.InitializeBoard();

        Timer = 0.0f;
        Term = false;
        // 점수 
        TetrisScore = 0;

        NowTetrominoID = 0;
        HoldTetrominoID = -1;

        TetroDegree = 0; 

        while (TetrominoPacks.Count > 0)
        {
            TetrominoPacks.RemoveAt(0);
        }

        ObjectRenderer.HoldBoardInitialize();
        ObjectRenderer.PreViewInitialize();

    }

    public void PlayerUpdate(int _attackCount)
    {
        Timer += Time.deltaTime;

        if ( EPlayer.Player1 == PlayerID)
        {
            Player1_KeyInput();
        }
        else { Player2_KeyInput(); }

        GameBoard.PushAttackLine(_attackCount); // 키 잇풋을 받은 후에 동작하도록 한다. 

        // 데이터 보드의 라인을 지우고 내리는 함수
        GameBoard.LineDelete(ref TetrisScore);

        GameBoard.DownLine();

        // 그려주기 함수들
        ObjectRenderer.DrawBoard(GameBoard.GetBoard());

        ObjectRenderer.DrawHoldBoard(HoldTetrominoID, Tetromino); // draw hold

        for (int i = 0; i < Define.Max; ++i) // draw preView 4개 만들기 
        {
            ObjectRenderer.DrawPreView(i, TetrominoPacks[i], Tetromino);
        }

        DrawGhostTetromino();

        ObjectRenderer.DrawTetromino(NowTetrominoID, PosY, PosX, NowTetromino);

        // 테트로미노 굳히기 
        if (GameBoard.Freeze(Timer, PosY, PosX,NowTetrominoID, NowTetromino) == true)
        {
            Timer -= Timer;

            SetTetromino();

            Term = false; // 보드에 굳은게 있으면 한턴이 끝난 것. 
        }
    }

    // 현재 테트로미노 설정.  
    public void SetTetromino()
    {
        // 랜덤으로 테트리미노를 뽑는다.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        TetroDegree = 0; // 맨 처음 상태 

        NowTetrominoID = TetrominoPacks[0];
        TetrominoPacks.RemoveAt(0); // 사용된 테트로미노는 팩에서 제거. 

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);

        if (!CheckDirection(PosY - 1, PosX)) // 시작 위치에 굳은 테트로미노가 있으면 위로 한칸 올려주자.
        {
            ++PosY;
        }
    }

    // 홀드에서 꺼낸 테트로미노 설정
    private void PopHoldTetromino()
    {
        // 랜덤으로 테트리미노를 뽑는다.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);
    }

    private void HaveToHoldTateromino()
    {
        // 첫 홀드 시, 현재 테트로미노를 홀드해놓고 새 테트로미노를 꺼낸다. 
        if (HoldTetrominoID == -1)
        {
            HoldTetrominoID = NowTetrominoID;

            SetTetromino();
        }
        else
        {
            int _tempArr = NowTetrominoID;
            NowTetrominoID = HoldTetrominoID;
            HoldTetrominoID = _tempArr;
        }
    }

    private void Player1_KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.M))
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

        // 밑으로 내리는 코드 
        if (CheckDirection(PosY - 1, PosX))
        {
            if (Timer > Define.WaitTime)
            {
                --PosY;

                Timer -= Define.WaitTime;
            }
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            while (CheckDirection(PosY - 1, PosX))
            {
                --PosY;

                Timer += 1.0f;

                SoundManager.Instance.PlaySound("drop");
            }
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (Term == false)
            {
                Term = true; // 이번 턴에 홀드를 사용했다. 
                HaveToHoldTateromino();

                PopHoldTetromino();
            }
        }
    }

    private void Player2_KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.R)) // 시계방향 회전
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.W)) // 반시계방향 회전
        {
            LeftRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            NowKeyInput = KeyCode.D;

            if (CheckDirection(PosY - 1, PosX))
            {
                --PosY;
            }

            deltaTime = 0; // 첫 키에는 초기화 

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.D) && OldKeyInput == NowKeyInput)
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

        // 밑으로 내리는 코드 
        if (CheckDirection(PosY - 1, PosX))
        {
            if (Timer > Define.WaitTime)
            {
                --PosY;

                Timer -= Define.WaitTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            NowKeyInput = KeyCode.S;

            if (CheckDirection(PosY, PosX - 1))
            {
                --PosX;
            }

            deltaTime = 0;

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.S) && OldKeyInput == NowKeyInput)
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            NowKeyInput = KeyCode.F;

            deltaTime = 0;

            if (CheckDirection(PosY, PosX + 1))
            {
                ++PosX;
            }

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.F) && OldKeyInput == NowKeyInput)
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) // space
        {
            while (CheckDirection(PosY - 1, PosX))
            {
                --PosY;

                Timer += 1.0f;

                SoundManager.Instance.PlaySound("drop");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) // 홀드
        {
            if (Term == false)
            {
                Term = true; // 이번 턴에 홀드를 사용했다. 
                
                HaveToHoldTateromino();

                PopHoldTetromino();
            }
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
                var _tatromino = NowTetromino[y, x];

                if (_tatromino == 0) continue;

                if (_posY - y < 2) return false; // 바닥을 만났을때 실패반환

                if (_posX + x < 2 || _posX + x > 11) return false; // 좌 우 에 있는 벽을 만났을때 실패 반환

                var _dataBoard = GameBoard.GetBoard();

                if ((int)ETetrominoColor.Empty < _dataBoard[_posY - y, _posX + x] && _dataBoard[_posY - y, _posX + x] <= (int)ETetrominoColor.Gray) return false; // 밑에 굳은 테트로미노 검사
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

    private bool Spin_RightRotate(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O 모양은 검사 x; 

        for (int i = 0; i < 4; i++)
        {
            // 반시계 반향으로 회전하기 때문에 -1를 곱해줘야함. 
            var _offsetX = -1 * Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = -1 * Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

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

        if (TetroDegree <= 0) _degree = 3;
        else _degree = TetroDegree - 1;

        for (int i = 0; i < 4; i++)
        {
            // 반시계 반향으로 회전하기 때문에 -1를 곱해줘야함. 
            var _offsetX = Define.Spin_Offset[NowTetrominoID, _degree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, _degree, i, 1];

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

    // 회전하다 벽을 뚫으면 벽에 들어간 오프셋만큼 위치값 이동 
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

                // 벽을 뚫어서 옮겨진 좌표에 굳은 테트로미노가 있나? 
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

    private bool Spin(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O 모양은 검사 x; 

        for (int i = 0; i < 4; i++)
        {
            var _offsetX = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

            Debug.LogWarning(NowTetrominoID);

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
                if ( _dataBoard[_posY - y + _offsetY, _posX + x + _offsetX] != 0) return false; // 벽과 다른 오브젝트와 만나면 회전 금지.
            }
        }

        return true;
    }

    private void DrawGhostTetromino()
    {
        int _tempY = PosY;

        while (CheckDirection(_tempY, PosX))
        {
            --_tempY;
        } // 바닥에 닿을 때까지 

        ObjectRenderer.DrawGhostTetromino(NowTetrominoID, _tempY, PosX, NowTetromino);
    }

    public bool CheckDeadLine()
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

    public int AttactLine()
    {
        if ( GameBoard.DeleteLineCount - 1 <= 0) return 0; 

        return GameBoard.DeleteLineCount - 1; 
    }
}
