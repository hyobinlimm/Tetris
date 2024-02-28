using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESound
{
    Bgm,
    Effect,
    End
}

public enum EPlayer
{
    Player1,
    Player2,
    Player3,
    Player4,
}

public enum EUIbutton
{
    Single,
    Battle,
    Quit,
}

public enum ETetrominoType
{
    L,
    J,
    S,
    Z,
    T,
    I,
    O,
}


enum ETetrominoColor
{
    Wall = -1,
    Empty,
    Orange,
    Blue,
    Green,
    Red,
    Purple,
    Cyan,
    Yellow,
    Gray,
}

public static class Define
{
    // ���� ������ ����, ����
    // board �� 20��(21��°����) // Srart_Y(23��) // dead(24��)
    public static int Width = 14;
    public static int Height = 29;

    public static int Max = 4; // ��Ʈ�ι̳� max
    public static int Max4x4 = 4; // ��Ʈ�ι̳� max
    public static int Max3x3 = 3; // ��Ʈ�ι̳� max

    public static int Start_Y = 24; 
    public static int Start_X = 5;

    public static int DeadLine = 23;

    // �ð� ���� ���� 
    public static float WaitTime = 0.6f; // �ڵ����� �����ִ� �ð�. 
    public static float LimitTime = 0.1f;
    public static float conditionTime = 0.12f;

    // T-spin �� ���� ������
    public static int[,,] LSpin_Offset = { 
                                           { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
                                           { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
                                           { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
                                           { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }};

    public static int[,,] JSpin_Offset = {
                                           { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
                                           { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
                                           { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
                                           { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }};

    public static int[,,] TSpin_Offset = {
                                           { { -1, 0 }, { -1, 1 }, { 0, 0 }, { -1, -2 } },
                                           { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
                                           { { 1, 0 }, { 0, 0 }, { 0, -2 }, { 1, -2 } },
                                           { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }};

    public static int[,,] SSpin_Offset = {
                                           { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
                                           { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
                                           { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
                                           { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }};

    public static int[,,] ZSpin_Offset = {
                                           { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
                                           { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
                                           { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
                                           { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }};

    public static int[,,,] Spin_Offset = {
        { // L
          { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
          { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
          { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
          { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } } 
        },
        
        { // J
          { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
          { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
          { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
          { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }
        },

        { // S
          { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
          { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
          { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
          { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }
        },
        
        { // Z
          { { -1, 0 }, { -1, 1 }, { 0, -2 }, { -1, -2 } },
          { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
          { { 1, 0 }, { 1, 1 }, { 0, -2 }, { 1, -2 } },
          { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }
        },

        { // T
          { { -1, 0 }, { -1, 1 }, { 0, 0 }, { -1, -2 } },
          { { 1, 0 }, { 1, -1 }, { 0, 2 }, { 1, 2 } },
          { { 1, 0 }, { 0, 0 }, { 0, -2 }, { 1, -2 } },
          { { -1, 0 }, { -1, -1 }, { 0, 2 }, { -1, 2 } }
        }
    };
}