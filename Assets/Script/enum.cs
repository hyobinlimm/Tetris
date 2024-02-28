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
    // 게임 보드의 가로, 세로
    // board 총 20줄(21번째까지) // Srart_Y(23번) // dead(24번)
    public static int Width = 14;
    public static int Height = 29;

    public static int Max = 4; // 테트로미노 max
    public static int Max4x4 = 4; // 테트로미노 max
    public static int Max3x3 = 3; // 테트로미노 max

    public static int Start_Y = 24; 
    public static int Start_X = 5;

    public static int DeadLine = 23;

    // 시간 관련 변수 
    public static float WaitTime = 0.6f; // 자동으로 내려주는 시간. 
    public static float LimitTime = 0.1f;
    public static float conditionTime = 0.12f;

    // T-spin 를 위한 오프셋
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