using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 테트로미노를 생성 및 관리해주는 클래스. 

public class Tetromino : MonoBehaviour
{
    // 테트로미노 배열
    int[,] L_arr;
    int[,] J_arr;
    int[,] S_arr;
    int[,] Z_arr;
    int[,] T_arr;
    int[,] I_arr;
    int[,] O_arr;

    public int[,] NowTetromino;    // 현재 테트로미노 
    public int[,] HoldTetromino;    // 홀드 된 테트로미노 
    public List<int[,]> TetrominoList;  // data 를 담은 배열(4x4 tetromino)
    //public List<int>TetrominoPacks;  // 테트로미노 팩을 담은 배열

    int[] TetrominoIndexes; // 테트로미노의 인덱스만 담은 배열
    int TetrominoCount; // 7개의 테트로미노를 모두 뽑았다면 초기화  

    private void Awake()
    {
        TetrominoCount = 7;
        TetrominoIndexes = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        TetrominoList = new List<int[,]>();

        L_arr = new int[,] { { 0, 0, 1 },
                             { 1, 1, 1 },
                             { 0, 0, 0 },};

        J_arr = new int[,] { { 2, 0, 0 },
                             { 2, 2, 2 },
                             { 0, 0, 0 } };

        S_arr = new int[,] { { 0, 3, 3 },
                             { 3, 3, 0 },
                             { 0, 0, 0 } };

        Z_arr = new int[,] { { 4, 4, 0 },
                             { 0, 4, 4 },
                             { 0, 0, 0 } };

        T_arr = new int[,] { { 0, 5, 0 },
                             { 5, 5, 5 },
                             { 0, 0, 0 } };

        I_arr = new int[,] { { 0, 0, 0, 0 },
                             { 6, 6, 6, 6 },
                             { 0, 0, 0, 0 },
                             { 0, 0, 0, 0 } };

        O_arr = new int[,] { { 0, 0, 0, 0 },
                             { 0, 7, 7, 0 },
                             { 0, 7, 7, 0 },
                             { 0, 0, 0, 0 } };

        TetrominoList.Add(L_arr);
        TetrominoList.Add(J_arr);
        TetrominoList.Add(S_arr);
        TetrominoList.Add(Z_arr);
        TetrominoList.Add(T_arr);
        TetrominoList.Add(I_arr);
        TetrominoList.Add(O_arr);
    }

    public void SetRotateTetromino(int[,] tetromino)
    {
        NowTetromino = tetromino;
    }

    public void SetTetromino()
    {
        int num = RandomTetrominoIndex();

        NowTetromino = TetrominoList[num];
    }

    public int[,] GetTetromino(int ID)
    {
        return TetrominoList[ID];
    }

    // 테트로미노 랜덤으로 생성하는 함수. 
    int RandomTetrominoIndex()
    {
        if (TetrominoCount == 0) TetrominoCount = 7; // 7개의 테트로미노를 다 돌았다면 초기화

        int _Index = UnityEngine.Random.Range(0, TetrominoCount);

        var _tetrominoNum = TetrominoIndexes[_Index]; // 현재 보여질 테트로미노 인덱스

        // 뽑은 인덱스를 배열의 맨 뒤로 보내고, 그 자리에 배열의 맨 뒤에 있던 값을 넣어줌. 
        TetrominoIndexes[_Index] = TetrominoIndexes[TetrominoCount - 1];
        TetrominoIndexes[TetrominoCount - 1] = _tetrominoNum;

        --TetrominoCount; // 이미 뽑은 것을 못 뽑게 카운드 다운, 다음번에는 현재 뽑은 것을 제외하고 랜덤이 나오게 됨. 

        return _tetrominoNum;
    }

    // 만들어진 팩을 플레이어에게 넘겨주는 함수. 
    public List<int> GetTetrominoPack()
    {
        List<int> _tetrominoPacks = new List<int>();  // 테트로미노 팩을 담은 배열

        // 테트로미노 7개를 담은 팩를 5개 만들어 놓는다. 
        for (int j = 0; j < 5; ++j)
        {
            for (int i = 0; i < 7; i++)
            {
                _tetrominoPacks.Add(RandomTetrominoIndex());
            }
        }

        return _tetrominoPacks;
    }
}

