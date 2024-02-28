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

    // ��Ʈ�ι̳뿡 ���� ������
    public List<int> TetrominoPacks; // ��Ʈ�ι̳� ��(��Ʈ�ι̳� 7�� ����)
    private int NowTetrominoID = 0; // ��Ʈ�ι̳��� ID.
    private int HoldTetrominoID = -1; // Ȧ��� ��Ʈ�ι̳��� ID. 
    private int[,] NowTetromino; // ���� �ڽ��� ��Ʈ�ι̳븦 ���� �迭. 

    int TetroDegree = 0; // ���� ��Ʈ�ι̳��� ���� 90 : 0 > 1 / 180 : 1 > 2 / 270 : 2 > 3 / 360 : 3 > 0

    // ���� ��ǥ 
    private int PosY;
    private int PosX;

    // �ð� ���� ���� 
    private float Timer = 0f;
    private bool Term = false;

    // key Input ����
    KeyCode NowKeyInput;
    KeyCode OldKeyInput;
    float deltaTime = 0.0f;

    // ���� 
    int TetrisScore = 0;

    public int Pos_Y
    { get { return PosY; } set { PosY = value; } }

    public int Pos_X
    { get { return PosX; } set { PosX = value; } }

    public void Initialized(EPlayer ID, GameObject _prefabFolder, Vector3 _holdPos, Vector3 _preViewPos)
    {
        Debug.Log("Player �� initialize");

        PlayerID = ID;

        GameBoard = GetComponent<Board>();
        ObjectRenderer = GetComponent<CRenderer>();
        Tetromino = GameObject.Find("BattleGameManager").GetComponent<Tetromino>();

        GameBoard.InitializeBoard();

        // ������ ���忡 ������ ���� ������Ʈ ���� ����
        ObjectRenderer.CreateRenderBoard(_prefabFolder);
        ObjectRenderer.CreatHoldBaord(_holdPos, "hold"); // Ȧ�� ���� ����. 
        ObjectRenderer.CreatePreViewBoard(_preViewPos, "PreView");

        // ��Ʈ�ι̳� �޾ƿ��� 
        TetrominoPacks = new List<int>();
    }

    public void DataInitialize()
    {
        // ����� �����͸� ������. 
        GameBoard.InitializeBoard();

        Timer = 0.0f;
        Term = false;
        // ���� 
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

        GameBoard.PushAttackLine(_attackCount); // Ű ��ǲ�� ���� �Ŀ� �����ϵ��� �Ѵ�. 

        // ������ ������ ������ ����� ������ �Լ�
        GameBoard.LineDelete(ref TetrisScore);

        GameBoard.DownLine();

        // �׷��ֱ� �Լ���
        ObjectRenderer.DrawBoard(GameBoard.GetBoard());

        ObjectRenderer.DrawHoldBoard(HoldTetrominoID, Tetromino); // draw hold

        for (int i = 0; i < Define.Max; ++i) // draw preView 4�� ����� 
        {
            ObjectRenderer.DrawPreView(i, TetrominoPacks[i], Tetromino);
        }

        DrawGhostTetromino();

        ObjectRenderer.DrawTetromino(NowTetrominoID, PosY, PosX, NowTetromino);

        // ��Ʈ�ι̳� ������ 
        if (GameBoard.Freeze(Timer, PosY, PosX,NowTetrominoID, NowTetromino) == true)
        {
            Timer -= Timer;

            SetTetromino();

            Term = false; // ���忡 ������ ������ ������ ���� ��. 
        }
    }

    // ���� ��Ʈ�ι̳� ����.  
    public void SetTetromino()
    {
        // �������� ��Ʈ���̳븦 �̴´�.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        TetroDegree = 0; // �� ó�� ���� 

        NowTetrominoID = TetrominoPacks[0];
        TetrominoPacks.RemoveAt(0); // ���� ��Ʈ�ι̳�� �ѿ��� ����. 

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);

        if (!CheckDirection(PosY - 1, PosX)) // ���� ��ġ�� ���� ��Ʈ�ι̳밡 ������ ���� ��ĭ �÷�����.
        {
            ++PosY;
        }
    }

    // Ȧ�忡�� ���� ��Ʈ�ι̳� ����
    private void PopHoldTetromino()
    {
        // �������� ��Ʈ���̳븦 �̴´�.
        PosY = Define.Start_Y - 1;
        PosX = Define.Start_X;

        NowTetromino = Tetromino.GetTetromino(NowTetrominoID);
    }

    private void HaveToHoldTateromino()
    {
        // ù Ȧ�� ��, ���� ��Ʈ�ι̳븦 Ȧ���س��� �� ��Ʈ�ι̳븦 ������. 
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

        // ������ ������ �ڵ� 
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
                Term = true; // �̹� �Ͽ� Ȧ�带 ����ߴ�. 
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

        if (Input.GetKeyDown(KeyCode.R)) // �ð���� ȸ��
        {
            RightRotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.W)) // �ݽð���� ȸ��
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

            deltaTime = 0; // ù Ű���� �ʱ�ȭ 

            OldKeyInput = NowKeyInput;
        }
        else if (Input.GetKey(KeyCode.D) && OldKeyInput == NowKeyInput)
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

        // ������ ������ �ڵ� 
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

        if (Input.GetKeyDown(KeyCode.LeftControl)) // Ȧ��
        {
            if (Term == false)
            {
                Term = true; // �̹� �Ͽ� Ȧ�带 ����ߴ�. 
                
                HaveToHoldTateromino();

                PopHoldTetromino();
            }
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
                var _tatromino = NowTetromino[y, x];

                if (_tatromino == 0) continue;

                if (_posY - y < 2) return false; // �ٴ��� �������� ���й�ȯ

                if (_posX + x < 2 || _posX + x > 11) return false; // �� �� �� �ִ� ���� �������� ���� ��ȯ

                var _dataBoard = GameBoard.GetBoard();

                if ((int)ETetrominoColor.Empty < _dataBoard[_posY - y, _posX + x] && _dataBoard[_posY - y, _posX + x] <= (int)ETetrominoColor.Gray) return false; // �ؿ� ���� ��Ʈ�ι̳� �˻�
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

    private bool Spin_RightRotate(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O ����� �˻� x; 

        for (int i = 0; i < 4; i++)
        {
            // �ݽð� �������� ȸ���ϱ� ������ -1�� ���������. 
            var _offsetX = -1 * Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = -1 * Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

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

        if (TetroDegree <= 0) _degree = 3;
        else _degree = TetroDegree - 1;

        for (int i = 0; i < 4; i++)
        {
            // �ݽð� �������� ȸ���ϱ� ������ -1�� ���������. 
            var _offsetX = Define.Spin_Offset[NowTetrominoID, _degree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, _degree, i, 1];

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

    // ȸ���ϴ� ���� ������ ���� �� �����¸�ŭ ��ġ�� �̵� 
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

                // ���� �վ �Ű��� ��ǥ�� ���� ��Ʈ�ι̳밡 �ֳ�? 
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

    private bool Spin(int _posY, int _posX, int[,] _rotateArr)
    {
        var _dataBoard = GameBoard.GetBoard();

        if ((int)ETetrominoType.I <= NowTetrominoID) return false; // I, O ����� �˻� x; 

        for (int i = 0; i < 4; i++)
        {
            var _offsetX = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 0];
            var _offsetY = Define.Spin_Offset[NowTetrominoID, TetroDegree, i, 1];

            Debug.LogWarning(NowTetrominoID);

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
                if ( _dataBoard[_posY - y + _offsetY, _posX + x + _offsetX] != 0) return false; // ���� �ٸ� ������Ʈ�� ������ ȸ�� ����.
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
        } // �ٴڿ� ���� ������ 

        ObjectRenderer.DrawGhostTetromino(NowTetrominoID, _tempY, PosX, NowTetromino);
    }

    public bool CheckDeadLine()
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

    public int AttactLine()
    {
        if ( GameBoard.DeleteLineCount - 1 <= 0) return 0; 

        return GameBoard.DeleteLineCount - 1; 
    }
}
