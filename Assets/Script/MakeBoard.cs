using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// ��Ʈ������ ���带 ���� �� �ʱ�ȭ�� ���ִ� ��.
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

    // ���� ������ ����, ����
    readonly int Width = 14;
    readonly int Height = 22;
    readonly int Max = 4; // ��Ʈ�ι̳� max

    readonly int Board_Min = 2; 
    readonly int Board_Max = 11; 

    // ���� ���� �迭
    int[,] DataBoard; // �����Ϳ�
    Block[,] ObjectBoard; // ���� �׷����� ������Ʈ�� �����ϴ� �迭

    // ��Ʈ�ι̳� �迭
    int[,] L_arr;
    int[,] J_arr;
    int[,] S_arr;
    int[,] Z_arr;
    int[,] T_arr;
    int[,] I_arr;
    int[,] O_arr;
    int[,] NowTetromino;    // ���� ��Ʈ�ι̳� 

    // ���� ��ǥ 
    public int PosY;
    public int PosX;

    // ��������Ʈ�� ����� ������ ����
    float SpriteWidth;
    float SpriteHeight;

    public List<Material> Materials;    // ���͸����� ���� �迭
    public List<Sprite> SpriteList;     // ��Ʈ�ι̳� �ؽ�ó�� ���� �迭
    List<int[,]> TetrominoList;         // ��Ʈ�ι̳븦 ���� �迭
    List<int> DeleteY;                  // ���� �� Y ���� ���� �迭 

    // ���� ������Ʈ�� 
    // ���忡 ���õ� ���� ������Ʈ
    public Block EmptyPrefab;
    GameObject BoardPrefab; // ��Ÿ�� �� ������ wall, empty �� ���� ��. 

    // �ð� ���õ� ������ 
    float Timer = 0.0f;
    public float WaitTime = 1.0f;
    public float LimitTime = 0.0f;
    bool Check = true; // �Ʒ� ������ ���Ȱ�, �ڷ�ƾ �Լ����� WaitForSecondes() �ϸ� false�� �ٲ��ش�. 

    // ��ũ��Ʈ �ν��Ͻ��� ���� ���� �ѹ��� ȣ��.
    // ȣ�� ������ ���������� �ʴ�. 
    // �����ڿ� ����ϴٰ� �����ϸ� �� ��.
    // �Ҹ��ڴ� �ʿ� ����.(������ �÷��Ͱ� ����.)
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

        // ��������Ʈ ������ ���ϱ� 
        SpriteHeight = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        SpriteWidth = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;   // 60 ������

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

                // ���� ���̾��Ű ���� ����� �ٸ� ���.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    }

    // ���� ��Ʈ�ι̳� ����.  
    void SetTerimino()
    {
        // �������� ��Ʈ���̳븦 �̴´�.
        PosY = 20;
        PosX = 2;

        int num = Random.Range(0, 7);
        NowTetromino = TetrominoList[num];
    }

    // ���忡 �׷���. 
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
        // GetKey : �ĺ��� Ű�� ��������
        // GetKeyDown : �ĺ��� Ű�� ������ ���� �� true ��ȯ
        // GetKeyUp : �ĺ��� Ű�� ������ �������� true ��ȯ 

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
        yield return new WaitForSeconds(limitTime); // 0.1�� ��ٸ���. 
        Check = true;
    }

    // �� �ձ� ����
    bool CheckDirection(int _posY, int _posX, int[,] _tetrominoArr)
    {
        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                if (_tetrominoArr[y, x] == 0) continue;

                if (_posY - y < 2) return false; // �ٴ��� �������� ���й�ȯ

                if (_posX + x < 2 || _posX + x > 11) return false; // �� �� �� �ִ� ���� �������� ���� ��ȯ

                if (0 < DataBoard[_posY - y, _posX + x] && DataBoard[_posY - y, _posX + x] < 8) return false; // �ؿ� ���� ��Ʈ�ι̳� �˻�
                // y + 1 �� ���� ����. ���� ��ǥ�� �ؿ� ���� ��Ʈ�ι̳밡 �ִ��� �˻� �ϱ� ����. 
            }
        }
        return true;
    }

    // 2���� �迭 90�� ȸ�� ��Ű��
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

    // ȸ���ϴ� ���� ������ ���� �� �����¸�ŭ ��ġ�� �̵� 
    void PositionOffset(int _posY, int _posX, int[,] _rotateArr)
    {
        int offset_Y = 0; 
        int offset_X = 0;

        for (int y = 0; y < Max; ++y)
        {
            for (int x = 0; x < Max; ++x)
            {
                if (_rotateArr[y, x] == 0) continue;

                if (_posX + x < Board_Min) // ����, ������ �� �˻�. 
                {
                    if (_posX + x < offset_X ) continue;
                    offset_X = Board_Min - (_posX + x);
                }
                else if(Board_Max < _posX + x)
                {
                    if (_posX + x < offset_X) continue;
                    offset_X = Board_Max - (_posX + x);
                }
                
                if (_posY - y < 2) // �ٴ� ������? 
                {
                    if(offset_Y < (_posY - y)) offset_Y = (_posY - y); 
                }

                //if (!CheckDirection(_posY, _posX, _rotateArr)) return; // �ٸ� ������Ʈ�� ������ ȸ�� ����.
            }
        }

        

        // ���� �������� ������ ���� ���Ѵ�. 
        PosX += offset_X;
        PosY += offset_Y;
    }

    // ������Ʈ �迭���� ���� ��Ʈ�ι̳븦 �׸���. 
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

        if (Timer < 1.0f) return;  // ��Ʈ�ι̳� ���⸦ ���� Ÿ�̸�

        for (int y = 0; y < Max; ++y)
        {
            if (isFrozen) break;

            for (int x = 0; x < Max; ++x)
            {
                if (NowTetromino[y, x] == (int)ETetrominoColor.Empty) continue;

                if (PosY - (y + 1) < 0) continue;

                //var boardData = DataBoard[PosY - (y + 1), PosX + x];
                var boardData = DataBoard[PosY - (y + 1), PosX + x]; // ���� ��Ʈ�ι̳� ��ġ�� �ٷ� ���� �˻�. 
                //if (boardData == (int)ETetrominoColor.Gray || boardData == (int)ETetrominoColor.Wall)
                if ((0 < boardData && boardData < 8) || boardData == (int)ETetrominoColor.Wall)
                {
                    isFrozen = true;
                    PushFreezeTetromino(); // ��Ʈ�ι̳븦 ������.
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
                    DataBoard[y, x] = (int)ETetrominoColor.Empty; // 0���� �ʱ�ȭ  
                }
            }
        }
    }

    int CheckLineOffset(ref List<int> line) // Ŭ������ ��� ���۷����� �޴´ٰ� ��������. 
    {
        // 1. �� ���̻� ������µ� �˻�. 
        // 2. ���� ����� ��ŭ ��ϵ��� �Ʒ��� ������. 
        int offset = 0;

        for (int i = 0; i < line.Count; ++i)
        {
            ++offset;

            if (i + 1 == line.Count) break;

            var column = line[i]; // x ���� ���ŵ� y ��

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
        int downConut = 0; // �ѹ��̶� ���� ������ ���������� ++;  

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

