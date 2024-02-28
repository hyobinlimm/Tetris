using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

// ������Ʈ�� �׷��ִ� Ŭ����

public class CRenderer : MonoBehaviour
{
    Block[,] ObjectBoard; // ���� �׷����� ������Ʈ�� �����ϴ� �迭
    Block[,] HoldBoard; // hold �� ��Ʈ�ι̳븦 �����ִ� �迭

    List<Block[,]> PreViews;

    public List<Material> Materials;    // ���͸����� ���� �迭
    List<Sprite> SpriteList;     // ��Ʈ�ι̳� �ؽ�ó�� ���� �迭

    // ��������Ʈ�� ����� ������ ����
    float SpriteWidth;
    float SpriteHeight;

    // ���忡 ���õ� ���� ������Ʈ
    public Block EmptyPrefab;

    GameObject HoldViewPrefab;
    List<GameObject> PreViewFolders;

    Vector3 HoldDefaultPos;
    Vector3 PreViewDefaultPos;

    private void Awake()
    {
        Debug.Log("CRenderer�� Awake");

        ObjectBoard = new Block[Define.Height, Define.Width];
        HoldBoard = new Block[Define.Max, Define.Max];

        PreViewFolders = new List<GameObject>();
        PreViews = new List<Block[,]>();

        EmptyPrefab = Resources.Load<Block>("Block") as Block;

        SpriteList = GameObject.Find("BattleGameManager").GetComponent<BattleGameManager>().SpriteList;

        SpriteHeight = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        SpriteWidth = EmptyPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    // � ������ �׸��� ������ ��� ���� �ʱ�ȭ 
    public void CreateRenderBoard(GameObject _folder)
    {
        float _parent_PosX = _folder.transform.position.x;
        float _parent_PosY = _folder.transform.position.y;

        for (int y = 0; y < Define.Height; ++y)
        {
            for (int x = 0; x < Define.Width; ++x)
            {
                if (y <= 1 || (x <= 1 || x >= 12))
                {
                    ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, _folder.transform);
                    ObjectBoard[y, x].transform.position = new Vector3((SpriteWidth * x) + _parent_PosX, (SpriteHeight * y) + _parent_PosY, 0);
                    ObjectBoard[y, x].GetComponent<SpriteRenderer>().enabled = false;

                    continue;
                }
                ObjectBoard[y, x] = Instantiate<Block>(EmptyPrefab, _folder.transform);
                ObjectBoard[y, x].transform.position = new Vector3((SpriteWidth * x) + _parent_PosX, (SpriteHeight * y) + _parent_PosY, 0);

                // ���� ���̾��Ű ���� ����� �ٸ� ���.
                //var tempEmpty = Instantiate<GameObject>(EmptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                //tempEmpty.transform.parent = EmptyFolder.transform;
            }
        }
    }

    // ���忡 �׷���. 
    public void DrawBoard(int[,] _dataBoard)
    {
        for (int y = 0; y < Define.Height; ++y)
        {
            for (int x = 0; x < Define.Width; ++x)
            {
                var _gameBoard = _dataBoard[y, x];

                switch ((ETetrominoColor)_gameBoard)
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Orange]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Blue]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Green]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Red]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Purple]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Cyan]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Yellow]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Gray]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    case ETetrominoColor.Empty:
                        ObjectBoard[y, x].SetSprite(SpriteList[(int)ETetrominoColor.Empty]);
                        ObjectBoard[y, x].ReturnOriColor();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // ������Ʈ �迭���� ���� ��Ʈ�ι̳븦 �׸���. 
    public void DrawTetromino(int _tetroID, int _posY, int _posX, int[,] _tetromino)
    {
        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int posY = _posY - y;
                int posX = _posX + x;

                var _nowTetromino = _tetromino;

                if ((ETetrominoColor)_nowTetromino[y, x] == 0) continue;

                switch ((ETetrominoColor)_nowTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Orange));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Blue));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Green));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Red));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Purple));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[posY, posX].SetSprite(GetSprite(ETetrominoColor.Gray));
                        ObjectBoard[posY, posX].ReturnOriColor();
                        break;
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

    public void DrawGhostTetromino(int _tetroID, int _posY, int _posX, int[,] _tetromino)
    {
        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                int _tempY = (_posY + 1) - y;
                int _tempX = _posX + x;

                var _nowTetromino = _tetromino;

                if ((ETetrominoColor)_nowTetromino[y, x] == 0) continue;

                switch ((ETetrominoColor)_nowTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Orange));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Blue:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Blue));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Green:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Green));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Red:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Red));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Purple:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Purple));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Cyan:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Yellow:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    case ETetrominoColor.Gray:
                        ObjectBoard[_tempY, _tempX].SetSprite(GetSprite(ETetrominoColor.Gray));
                        ObjectBoard[_tempY, _tempX].SetGhostColor();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CreateBoard4x4(Vector3 _pos, string _folderName, Block[,] _board)
    {
        GameObject _preViewPrefab = new GameObject(_folderName);
        _preViewPrefab.transform.position = _pos;
        _preViewPrefab.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        PreViewDefaultPos = _pos;

        var _height = SpriteHeight * 0.7f;
        var _width = SpriteWidth * 0.7f;

        for (int y = 0; y < Define.Max; ++y)
        {
            for (int x = 0; x < Define.Max; ++x)
            {
                _board[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((_width * x) + _preViewPrefab.transform.position.x, (_height * y) + _preViewPrefab.transform.position.y, 0), Quaternion.identity, _preViewPrefab.transform);
                _board[y, x].GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        PreViewFolders.Add(_preViewPrefab);
    }

    private void CreateHoldBoard4x4(Vector3 _pos, string _folderName, Block[,] _board)
    {
        HoldViewPrefab = new GameObject(_folderName);
        HoldViewPrefab.transform.position = _pos;
        HoldViewPrefab.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        HoldDefaultPos = _pos;

        var _height = SpriteHeight * 0.7f;
        var _width = SpriteWidth * 0.7f;

        for (int y = 0; y < Define.Max; ++y)
        {
            for (int x = 0; x < Define.Max; ++x)
            {
                _board[y, x] = Instantiate<Block>(EmptyPrefab, new Vector3((_width * x) + HoldViewPrefab.transform.position.x, (_height * y) + HoldViewPrefab.transform.position.y, 0), Quaternion.identity, HoldViewPrefab.transform);
                _board[y, x].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void CreatHoldBaord(Vector3 _pos, string _folderName)
    {
        CreateHoldBoard4x4(_pos, _folderName, HoldBoard);
    }

    public void CreatePreViewBoard(Vector3 _pos, string _folderName)
    {
        for (int i = 0; i < Define.Max; i++)
        {
            var _board = new Block[Define.Max, Define.Max];
            PreViews.Add(_board);

            CreateBoard4x4(new Vector3(_pos.x, _pos.y - (i * 1.2f)), _folderName, PreViews[i]);
        }
    }

    private void InitializeBoard4X4(Block[,] _board) // Ȧ�� �� ��� �ʱ�ȭ. 
    {
        for (int y = 0; y < Define.Max; ++y)
        {
            for (int x = 0; x < Define.Max; ++x)
            {
                _board[y, x].SetSprite(GetSprite(ETetrominoColor.Empty));
                _board[y, x].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void HoldBoardInitialize()
    {
        InitializeBoard4X4(HoldBoard);
    }

    public void PreViewInitialize()
    {
        for (int i = 0; i < Define.Max; ++i)
        {
            InitializeBoard4X4(PreViews[i]);
        }
    }

    public void DrawHoldBoard(int _holdTetroID, Tetromino _tetromino)
    {
        DrawHoldBoard4x4(_holdTetroID, _tetromino, HoldBoard);
    }

    public void DrawPreView(int index, int _tetroID, Tetromino _tetromino)
    {
        PreViewPositioning(index, _tetroID);
        DrawPreViewBoard4x4(_tetroID, _tetromino, PreViews[index]);
    }

    private void HoldPositioning(int _tetroID)
    {
        if (_tetroID < (int)ETetrominoType.I)
        {
            HoldViewPrefab.transform.position = HoldDefaultPos;
        }
        else if (_tetroID == (int)ETetrominoType.I)
        {
            HoldViewPrefab.transform.position = new Vector3(HoldDefaultPos.x - 0.17f, HoldDefaultPos.y - 0.2f, 0.0f);
        }
        else // o ����� ��
        {
            HoldViewPrefab.transform.position = new Vector3(HoldDefaultPos.x - 0.16f, HoldDefaultPos.y - 0.04f, 0.0f);
        }
    }

    private void PreViewPositioning(int _index, int _tetroID)
    {
        var _nowPos = PreViewFolders[_index].transform.position;

        if (_tetroID < (int)ETetrominoType.I)
        {
            PreViewFolders[_index].transform.position = new Vector3(PreViewDefaultPos.x, _nowPos.y, _nowPos.z);
        }
        else
        {
            PreViewFolders[_index].transform.position = new Vector3(PreViewDefaultPos.x - 0.19f, _nowPos.y, _nowPos.z);
        }
    }

    private void DrawHoldBoard4x4(int _tetroID, Tetromino _tetromino, Block[,] _board)
    {
        if (_tetroID == -1) return; // ���� Ȧ�忡 �ƹ��͵� �ȵ����ִٴ� �ǹ�. �׷��� �ʿ� X

        InitializeBoard4X4(_board); // Ȥ�� �׷��� ���� ���� �ʱ�ȭ. 

        HoldPositioning(_tetroID);

        var _holdTetromino = _tetromino.GetTetromino(_tetroID);

        if (_holdTetromino == null) return; // ���� Ȧ�忡 �ƹ��͵� ���ٸ� ������. 

        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if ((ETetrominoColor)_holdTetromino[y, x] == 0) continue;

                int _posY = (_max - 1) - y;// �»������ �׸��� ���� ��ǥ 

                var _spriteEnabled = _board[_posY, x].GetComponent<SpriteRenderer>(); // ��������Ʈ�� ���� ����. 

                switch ((ETetrominoColor)_holdTetromino[y, x])
                {
                    case ETetrominoColor.Orange:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Orange));
                        break;
                    case ETetrominoColor.Blue:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Blue));
                        break;
                    case ETetrominoColor.Green:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Green));
                        break;
                    case ETetrominoColor.Red:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Red));
                        break;
                    case ETetrominoColor.Purple:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Purple));
                        break;
                    case ETetrominoColor.Cyan:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        break;
                    case ETetrominoColor.Yellow:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        break;
                    case ETetrominoColor.Gray:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Gray));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void DrawPreViewBoard4x4(int _tetroID, Tetromino _tetromino, Block[,] _board)
    {
        InitializeBoard4X4(_board); // Ȥ�� �׷��� ���� ���� �ʱ�ȭ. 

        var _tetro = _tetromino.GetTetromino(_tetroID);

        if (_tetro == null) return; // ���� Ȧ�忡 �ƹ��͵� ���ٸ� ������. 

        var _max = _tetroID < (int)ETetrominoType.I ? Define.Max3x3 : Define.Max4x4;

        for (int y = 0; y < _max; ++y)
        {
            for (int x = 0; x < _max; ++x)
            {
                if ((ETetrominoColor)_tetro[y, x] == 0) continue;

                int _posY = (_max - 1) - y;// �»������ �׸��� ���� ��ǥ 

                var _spriteEnabled = _board[_posY, x].GetComponent<SpriteRenderer>(); // ��������Ʈ�� ���� ����. 

                switch ((ETetrominoColor)_tetro[y, x])
                {
                    case ETetrominoColor.Orange:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Orange));
                        break;
                    case ETetrominoColor.Blue:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Blue));
                        break;
                    case ETetrominoColor.Green:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Green));
                        break;
                    case ETetrominoColor.Red:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Red));
                        break;
                    case ETetrominoColor.Purple:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Purple));
                        break;
                    case ETetrominoColor.Cyan:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Cyan));
                        break;
                    case ETetrominoColor.Yellow:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Yellow));
                        break;
                    case ETetrominoColor.Gray:
                        _spriteEnabled.enabled = true;
                        _board[_posY, x].SetSprite(GetSprite(ETetrominoColor.Gray));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
