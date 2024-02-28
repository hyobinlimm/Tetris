using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ��Ʈ�ι̳븦 ���� �� �������ִ� Ŭ����. 

public class Tetromino : MonoBehaviour
{
    // ��Ʈ�ι̳� �迭
    int[,] L_arr;
    int[,] J_arr;
    int[,] S_arr;
    int[,] Z_arr;
    int[,] T_arr;
    int[,] I_arr;
    int[,] O_arr;

    public int[,] NowTetromino;    // ���� ��Ʈ�ι̳� 
    public int[,] HoldTetromino;    // Ȧ�� �� ��Ʈ�ι̳� 
    public List<int[,]> TetrominoList;  // data �� ���� �迭(4x4 tetromino)
    //public List<int>TetrominoPacks;  // ��Ʈ�ι̳� ���� ���� �迭

    int[] TetrominoIndexes; // ��Ʈ�ι̳��� �ε����� ���� �迭
    int TetrominoCount; // 7���� ��Ʈ�ι̳븦 ��� �̾Ҵٸ� �ʱ�ȭ  

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

    // ��Ʈ�ι̳� �������� �����ϴ� �Լ�. 
    int RandomTetrominoIndex()
    {
        if (TetrominoCount == 0) TetrominoCount = 7; // 7���� ��Ʈ�ι̳븦 �� ���Ҵٸ� �ʱ�ȭ

        int _Index = UnityEngine.Random.Range(0, TetrominoCount);

        var _tetrominoNum = TetrominoIndexes[_Index]; // ���� ������ ��Ʈ�ι̳� �ε���

        // ���� �ε����� �迭�� �� �ڷ� ������, �� �ڸ��� �迭�� �� �ڿ� �ִ� ���� �־���. 
        TetrominoIndexes[_Index] = TetrominoIndexes[TetrominoCount - 1];
        TetrominoIndexes[TetrominoCount - 1] = _tetrominoNum;

        --TetrominoCount; // �̹� ���� ���� �� �̰� ī��� �ٿ�, ���������� ���� ���� ���� �����ϰ� ������ ������ ��. 

        return _tetrominoNum;
    }

    // ������� ���� �÷��̾�� �Ѱ��ִ� �Լ�. 
    public List<int> GetTetrominoPack()
    {
        List<int> _tetrominoPacks = new List<int>();  // ��Ʈ�ι̳� ���� ���� �迭

        // ��Ʈ�ι̳� 7���� ���� �Ѹ� 5�� ����� ���´�. 
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

