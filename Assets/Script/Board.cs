using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// ������ ������ ���� �� �����ϴ� Ŭ���� 

public class Board : MonoBehaviour
{
    readonly int Board_Min = 2;
    readonly int Board_Max = 11;

    // ���� ���� �迭
    int[,] DataBoard; // �����Ϳ�
   
    List<int> DeleteY;  // ���� �� Y ���� ���� �迭 

    public int DeleteLineCount; // ���ŵ� �� ��. 

    private void Awake()
    {
        Debug.Log("Board �� Awake");

        DataBoard = new int[Define.Height, Define.Width];

        DeleteY = new List<int>();
    }
   
    public void InitializeBoard() // �����ͺ��� �ʱ�ȭ 
    {
        for (int y = 0; y < Define.Height; ++y)
        {
            for (int x = 0; x < Define.Width; ++x)
            {
                if (y <= 1 || (x <= 1 || x >= 12))
                {
                    DataBoard[y, x] = (int)ETetrominoColor.Wall;

                    continue;
                }

                DataBoard[y, x] = (int)ETetrominoColor.Empty;

                // ���� ���̾��Ű ���� ����� �ٸ� ���.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    } 

    public int[,] GetBoard() 
    {
        return DataBoard; 
    }
   
    // x ���� �� ã���� ����. 
    public void LineDelete(ref int _score)
    {
        for (int y = 0; y < Define.Height; ++y)
        {
            int _count = 0;

            for (int x = 0; x < Define.Width; ++x)
            {
                if (0 < DataBoard[y, x] && DataBoard[y, x] <= 8) ++_count;
            }

            if (_count == 10)
            {
                DeleteY.Add(y);
            }
        }

        DeleteLineCount = DeleteY.Count; // ���ŵǴ� y ���� ������ ����. 

        _score += (DeleteLineCount * 100);
    }

    // ���� �� �ٿ� ���� �迭�� ������ �Ʒ��� ����. 
    public void DownLine()
    {
        int _deleteIndex = DeleteY.Count - 1; // list�� index�� ����  

        while (_deleteIndex >= 0)
        {
            for (int y = DeleteY[_deleteIndex]; y < Define.Height; ++y)
            {
                for (int x = 2; x < Define.Width; x++)
                {
                    if (y + 1 >= Define.Height) break; 

                    int changeRow = DataBoard[y + 1, x];

                    DataBoard[y, x] = changeRow;
                }

                SoundManager.Instance.PlaySound("line");
            }

            DeleteY.RemoveAt(_deleteIndex);
            --_deleteIndex;
        }
    }

    public void PushAttackLine(int num)
    {
        // �������� ����� �ϳ� �����. 
        int _Index = UnityEngine.Random.Range(Board_Min, Board_Max + 1);
        
        // num ��ŭ y ������ �ø���.
        for (int y = Define.Height - 1; Board_Min <= y  ; y--)
        {
            for (int x = Board_Min; x <= Board_Max; x++)
            {
                if (y - num < Board_Min) break;

                int changeRow = DataBoard[y - num , x];

                DataBoard[y , x] = changeRow;
            }
        }
        
        for(int y = Board_Min; y < Board_Min + num; y++)
        {
            for (int x = Board_Min; x <= Board_Max; x++)
            {
                if(x == _Index)
                { DataBoard[y, x] = (int)ETetrominoColor.Empty; }
                else
                { DataBoard[y, x] = (int)ETetrominoColor.Gray; }
            }
        }
    }

    // ��Ʈ�ι̳� �������� üũ. 
    public bool Freeze(float _timer, int _posY, int _posX, int _tetroID, int[,] _tetromino)
    {
        bool isFrozen = false;

        if (_timer < 1.0f) return isFrozen;  // ��Ʈ�ι̳� ���⸦ ���� Ÿ�̸�

        SoundManager.Instance.PlaySound("drop"); // ������ �� �Ҹ�

        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            if (isFrozen) break;

            for (int x = 0; x < _max; ++x)
            {
                if (_tetromino[y, x] == (int)ETetrominoColor.Empty) continue;

                if (_posY - (y + 1) < 0) continue;

                var boardData = DataBoard[_posY - (y + 1), _posX + x]; // ���� ��Ʈ�ι̳� ��ġ�� �ٷ� ���� �˻�. 

                if ((0 < boardData && boardData <= 8) || boardData == (int)ETetrominoColor.Wall)
                {
                    isFrozen = true;
                    PushFreezeTetromino(_posY, _posX, _tetroID, _tetromino); // ��Ʈ�ι̳븦 ������.

                    return isFrozen;
                }
            }
        }

        return isFrozen;
    }

    // ���� ��Ʈ�ι̳� �迭�� ������ �迭�� ����. 
    void PushFreezeTetromino(int _posY, int _posX, int _tetroID, int[,] _tetromino)
    {
        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                var _nowTetromino = _tetromino;

                if (_nowTetromino[y, x] == 0) continue;

                DataBoard[_posY - y, _posX + x] = _nowTetromino[y, x];
            }
        }
    }
}
