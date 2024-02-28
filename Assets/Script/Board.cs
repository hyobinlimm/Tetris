using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// 데이터 보드의 생성 및 관리하는 클래스 

public class Board : MonoBehaviour
{
    readonly int Board_Min = 2;
    readonly int Board_Max = 11;

    // 게임 보드 배열
    int[,] DataBoard; // 데이터용
   
    List<int> DeleteY;  // 제거 된 Y 열을 담은 배열 

    public int DeleteLineCount; // 제거된 줄 수. 

    private void Awake()
    {
        Debug.Log("Board 의 Awake");

        DataBoard = new int[Define.Height, Define.Width];

        DeleteY = new List<int>();
    }
   
    public void InitializeBoard() // 데이터보드 초기화 
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

                // 폴더 하이어라키 구조 만드는 다른 방법.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    } 

    public int[,] GetBoard() 
    {
        return DataBoard; 
    }
   
    // x 줄이 다 찾으면 지움. 
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

        DeleteLineCount = DeleteY.Count; // 제거되는 y 줄을 저장해 놓자. 

        _score += (DeleteLineCount * 100);
    }

    // 제거 된 줄에 따라 배열의 값들을 아래로 내림. 
    public void DownLine()
    {
        int _deleteIndex = DeleteY.Count - 1; // list의 index를 접근  

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
        // 랜덤으로 빈공간 하나 만들기. 
        int _Index = UnityEngine.Random.Range(Board_Min, Board_Max + 1);
        
        // num 만큼 y 축으로 올리기.
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

    // 테트로미노 굳었는지 체크. 
    public bool Freeze(float _timer, int _posY, int _posX, int _tetroID, int[,] _tetromino)
    {
        bool isFrozen = false;

        if (_timer < 1.0f) return isFrozen;  // 테트로미노 비비기를 위한 타이머

        SoundManager.Instance.PlaySound("drop"); // 굳었을 때 소리

        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            if (isFrozen) break;

            for (int x = 0; x < _max; ++x)
            {
                if (_tetromino[y, x] == (int)ETetrominoColor.Empty) continue;

                if (_posY - (y + 1) < 0) continue;

                var boardData = DataBoard[_posY - (y + 1), _posX + x]; // 현재 테트로미노 위치의 바로 밑을 검사. 

                if ((0 < boardData && boardData <= 8) || boardData == (int)ETetrominoColor.Wall)
                {
                    isFrozen = true;
                    PushFreezeTetromino(_posY, _posX, _tetroID, _tetromino); // 테트로미노를 굳힌다.

                    return isFrozen;
                }
            }
        }

        return isFrozen;
    }

    // 굳힐 테트로미노 배열을 데이터 배열에 넣음. 
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
