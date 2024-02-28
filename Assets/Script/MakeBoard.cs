using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// 테트리스의 보드를 생성 및 초기화를 해주는 곳.
public class MakeBoard : MonoBehaviour
{
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

    // 게임 보드의 가로, 세로
    readonly int Width = 14;
    readonly int Height = 22;
    readonly int Max = 4; // 테트로미노 max

    readonly int Board_Min = 2; 
    readonly int Board_Max = 11; 

    // 게임 보드 배열
    int[,] DataBoard; // 데이터용
    Block[,] ObjectBoard; // 실제 그려지는 오브젝트를 관리하는 배열

    // 테트로미노 배열
    int[,] L_arr;
    int[,] J_arr;
    int[,] S_arr;
    int[,] Z_arr;
    int[,] T_arr;
    int[,] I_arr;
    int[,] O_arr;
    int[,] NowTetromino;    // 현재 테트로미노 

    // 현재 좌표 
    public int PosY;
    public int PosX;

    // 스프라이트의 사이즈를 저장할 변수
    float SpriteWidth;
    float SpriteHeight;

    public List<Material> Materials;    // 머터리얼을 담은 배열
    public List<Sprite> SpriteList;     // 테트로미노 텍스처를 담은 배열
    List<int[,]> TetrominoList;         // 테트로미노를 담은 배열
    List<int> DeleteY;                  // 제거 된 Y 열을 담은 배열 

    // 게임 오브젝트들 
    // 보드에 관련된 게임 오프젝트
    public Block EmptyPrefab;
    GameObject BoardPrefab; // 런타임 중 생성된 wall, empty 를 담은 곳. 

    // 시간 관련된 변수들 
    float Timer = 0.0f;
    public float WaitTime = 1.0f;
    public float LimitTime = 0.0f;
    bool Check = true; // 아래 방향을 눌렸고, 코루틴 함수에서 WaitForSecondes() 하면 false로 바꿔준다. 

    // 스크립트 인스턴스의 수명 동안 한번만 호출.
    // 호출 순서가 정해져있지 않다. 
    // 생성자와 비슷하다고 생각하면 될 듯.
    // 소멸자는 필요 없음.(가비지 컬렉터가 해줌.)
    private void Awake()
    {
        DataBoard = new int[Height, Width];
        ObjectBoard = new Block[Height, Width];

        DeleteY = new List<int>();
        TetrominoList = new List<int[,]>();

        L_arr = new int[,] { { 0, 0, 1, 0 },
                             { 0, 0, 1, 0 },
                             { 0, 0, 1, 1 },
                             { 0, 0, 0, 0 } };

        J_arr = new int[,] { { 0, 0, 2, 0 },
                             { 0, 0, 2, 0 },
                             { 0, 2, 2, 0 },
                             { 0, 0, 0, 0 } };

        S_arr = new int[,] { { 0, 0, 0, 0 },
                             { 0, 0, 3, 3 },
                             { 0, 3, 3, 0 },
                             { 0, 0, 0, 0 } };

        Z_arr = new int[,] { { 0, 0, 0, 0 },
                             { 0, 4, 4, 0 },
                             { 0, 0, 4, 4 },
                             { 0, 0, 0, 0 } };

        T_arr = new int[,] { { 0, 0, 0, 0 },
                             { 0, 0, 5, 0 },
                             { 0, 5, 5, 5 },
                             { 0, 0, 0, 0 } };

        I_arr = new int[,] { { 0, 0, 6, 0 },
                             { 0, 0, 6, 0 },
                             { 0, 0, 6, 0 },
                             { 0, 0, 6, 0 } };

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

        // 스프라이트 사이즈 구하기 
        SpriteHeight = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        SpriteWidth = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;   // 60 프레임

        CreateBoard();

        int num = Random.Range(0, 7);
        NowTetromino = TetrominoList[num];
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        KeyInput();

        LineDelete();

        DownLine();

        DrawBoard();

        DrawTetromino();

        Freeze();
    }

    void CreateBoard()
    {
        BoardPrefab = new GameObject("Board_Folder");

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                if (y <= 1 || (x <= 1 || x >= 12))
                {
                    DataBoard[y, x] = (int)ETetrominoColor.Wall;

                    ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3(SpriteWidth * x, SpriteHeight * y, 0), Quaternion.identity, BoardPrefab.transform);
                    ObjectBoard[y, x].GetComponent<SpriteRenderer>().enabled = false;

                    continue;
                }

                DataBoard[y, x] = (int)ETetrominoColor.Empty;

                ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3(SpriteWidth * x, SpriteHeight * y, 0), Quaternion.identity, BoardPrefab.transform);

                // 폴더 하이어라키 구조 만드는 다른 방법.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    }

    // 현재 테트로미노 설정.  
    void SetTerimino()
    {
        // 랜덤으로 테트리미노를 뽑는다.
        PosY = 20;
        PosX = 2;

        int num = Random.Range(0, 7);
        NowTetromino = TetrominoList[num];
    }

    // 보드에 그려라. 
    void DrawBoard()
    {
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                switch ((ETetrominoColor)DataBoard[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Orange]);
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Blue]);
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Green]);
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Red]);
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Purple]);
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Cyan]);
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Yellow]);
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Gray]);
                        break;
                    case ETetrominoColor.Empty:
                        ObjectBoard[y, x].spriteSet(SpriteList[(int)ETetrominoColor.Empty]);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void KeyInput()
    {
        // GetKey : 식별된 키가 눌렸을때
        // GetKeyDown : 식별된 키를 누르고 있을 때 true 반환
        // GetKeyUp : 식별된 키를 눌렀다 떼었을때 true 반환 

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetTerimino();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateTetromino();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (CheckDirection(PosY, PosX, NowTetromino))
            {
                RotateTetromino();
            }
        }

        if (Input.GetKey(KeyCode.DownArrow) && Check)
        {
            Check = false;
            StartCoroutine(WaitForlimit(LimitTime));

            if (CheckDirection(PosY - 1, PosX, NowTetromino))
            {
                --PosY;
                Debug.Log("UpArrow " + PosY + " " + PosX);
            }
        }
        else
        {
            if (CheckDirection(PosY - 1, PosX, NowTetromino))
            {
                if (Timer > WaitTime)
                {
                    --PosY;
                    Debug.Log("UpArrow " + PosY + " " + PosX);

                    Timer -= WaitTime;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (CheckDirection(PosY, PosX - 1, NowTetromino))
            {
                --PosX;
                Debug.Log("UpArrow " + PosY + " " + PosX);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CheckDirection(PosY, PosX + 1, NowTetromino))
            {
                ++PosX;
                Debug.Log("UpArrow " + PosY + " " + PosX);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (CheckDirection(PosY - 1, PosX, NowTetromino))
            {
                --PosY;
            }
            Debug.Log("UpArrow " + PosY + " " + PosX);
        }
    }

    IEnumerator WaitForlimit(float limitTime)
    {
        yield return new WaitForSeconds(limitTime); // 0.1초 기다린다. 
        Check = true;
    }

    // 벽 뚫기 막기
    bool CheckDirection(int _posY, int _posX, int[,] _tetrominoArr)
    {
        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                if (_tetrominoArr[y, x] == 0) continue;

                if (_posY - y < 2) return false; // 바닥을 만났을때 실패반환

                if (_posX + x < 2 || _posX + x > 11) return false; // 좌 우 에 있는 벽을 만났을때 실패 반환

                if (0 < DataBoard[_posY - y, _posX + x] && DataBoard[_posY - y, _posX + x] < 8) return false; // 밑에 굳은 테트로미노 검사
                // y + 1 를 해준 이유. 현재 좌표의 밑에 굳은 테트로미노가 있는지 검사 하기 위해. 
            }
        }
        return true;
    }

    // 2차원 배열 90도 회전 시키기
    void RotateTetromino()
    {
        int[,] tempArr = new int[Max, Max];

        for (int y = 0; y < Max; y++)
        {
            for (int x = 0; x < Max; x++)
            {
                tempArr[y, x] = NowTetromino[Max - x - 1, y];
            }
        }

        PositionOffset(PosY, PosX, tempArr);
        NowTetromino = tempArr;
    }

    // 회전하다 벽을 뚫으면 벽에 들어간 오프셋만큼 위치값 이동 
    void PositionOffset(int _posY, int _posX, int[,] _rotateArr)
    {
        int offset_Y = 0; 
        int offset_X = 0;

        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                if (_rotateArr[y, x] == 0) continue;

                if (_posX + x < Board_Min) // 왼쪽, 오른쪽 벽 검사. 
                {
                    if (_posX + x < offset_X ) continue;
                    offset_X = Board_Min - (_posX + x);
                }
                else if(Board_Max < _posX + x)
                {
                    if (_posX + x < offset_X) continue;
                    offset_X = Board_Max - (_posX + x);
                }
                
                if (_posY - y < 2) // 바닥 뚫으면? 
                {
                    if(offset_Y < (_posY - y)) offset_Y = (_posY - y); 
                }

                //if (!CheckDirection(_posY, _posX, _rotateArr)) return; // 다른 오브젝트와 만나면 회전 금지.
            }
        }

        

        // 구한 오프셋을 포지션 값에 더한다. 
        PosX += offset_X;
        PosY += offset_Y;
    }

    // 오브젝트 배열에서 현재 테트로미노를 그린다. 
    void DrawTetromino()
    {
        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                int posY = PosY - y;
                int posX = PosX + x;

                if ((ETetrominoColor)NowTetromino[y, x] == 0) continue;

                switch ((ETetrominoColor)NowTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Orange));
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Blue));
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Green));
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Red));
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Purple));
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Cyan));
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Yellow));
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[posY, posX].spriteSet(GetSprite(ETetrominoColor.Gray));
                        break;
                    //case ETetrominoColor.Empty:
                    //    ObjectBoard[_posY, _posX].spriteSet(SpriteList[(int)ETetrominoColor.Empty]);
                    //    break;
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

    void PushFreezeTetromino()
    {
        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                if (NowTetromino[y, x] == 0) continue;

                DataBoard[PosY - y, PosX + x] = NowTetromino[y, x];
            }
        }
    }

    void Freeze()
    {
        bool isFrozen = false;

        if (Timer < 1.0f) return;  // 테트로미노 비비기를 위한 타이머

        for (int y = 0; y < Max; ++y)
        {
            if (isFrozen) break;

            for (int x = 0; x < Max; ++x)
            {
                if (NowTetromino[y, x] == (int)ETetrominoColor.Empty) continue;

                if (PosY - (y + 1) < 0) continue;

                //var boardData = DataBoard[PosY - (y + 1), PosX + x];
                var boardData = DataBoard[PosY - (y + 1), PosX + x]; // 현재 테트로미노 위치의 바로 밑을 검사. 
                //if (boardData == (int)ETetrominoColor.Gray || boardData == (int)ETetrominoColor.Wall)
                if ((0 < boardData && boardData < 8) || boardData == (int)ETetrominoColor.Wall)
                {
                    isFrozen = true;
                    PushFreezeTetromino(); // 테트로미노를 굳힌다.
                    SetTerimino();

                    Timer -= 1.0f;

                    break;
                }
            }
        }
    }

    void LineDelete()
    {
        for (int y = 0; y < Height; ++y)
        {
            int count = 0;

            for (int x = 0; x < Width; ++x)
            {
                if (0 < DataBoard[y, x] && DataBoard[y, x] < 8) ++count;
            }

            if (count == 10)
            {
                DeleteY.Add(y);

                for (int x = 0; x < Width; ++x)
                {
                    DataBoard[y, x] = (int)ETetrominoColor.Empty; // 0으로 초기화  
                }
            }
        }
    }

    int CheckLineOffset(ref List<int> line) // 클래스는 모두 레퍼런스로 받는다고 생각하자. 
    {
        // 1. 몇 줄이상 사라졌는데 검사. 
        // 2. 줄이 사라진 만큼 블록들을 아래로 내린다. 
        int offset = 0;

        for (int i = 0; i < line.Count; ++i)
        {
            ++offset;

            if (i + 1 == line.Count) break;

            var column = line[i]; // x 행이 제거된 y 열

            if (column + 1 == line[i + 1])
            {
                ++offset;

                line.RemoveAt(i + 1);
            }
            else
            {
                return offset;
            }
        }

        return offset;
    }

    void DownLine()
    {
        int downConut = 0; // 한번이라도 블럭이 밑으로 내려갔으면 ++;  

        while (DeleteY.Count != 0)
        {
            int offset = CheckLineOffset(ref DeleteY);

            for (int y = DeleteY[0] - downConut; y < Height; ++y)
            {
                for (int x = 2; x < Width; x++)
                {
                    if (y + offset >= Height) break;

                    int changeRow = DataBoard[y + offset, x];

                    DataBoard[y, x] = changeRow;
                }
            }

            DeleteY.RemoveAt(0);
            ++downConut;
        }
    }

}

