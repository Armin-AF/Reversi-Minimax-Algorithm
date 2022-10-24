using UnityEngine;

public enum PlayerType { Human, Agent };
public class ReversiScript : MonoBehaviour
{
    public GameObject tilePrefab;
    public PlayerType playerOneType, playerTwoType;
    public int width;
    public int height;
    public int depth;
    public float aiDelay;
    
    [SerializeField] private float aiTimer;
    [SerializeField] private Vector3 origin;
    
    private TileState[,] _board;
    private GameObject[,] _tileGameObjects;
    private bool _reset, _win;
    private TileState _currentColor;
    

    private void Awake()
    {
        aiTimer = aiDelay;
        _reset = false;
        _win = false;
        origin = transform.position;
        _currentColor = TileState.Black;
        SpawnBoard(origin, width, height);
    }

    private void Start()
    {
        SetStartPieces(width, height);
    }

    private void Update()
    {
        if (DetermineWin() && !_win)
        {
            HandleWin();
        }
        if (_reset)
        {
            ResetBoard();
        }

        if (_win) return;
        if (!GameRules.IsBoardPlayable(_board, _currentColor))
        {
            SwitchColor(_currentColor);
        }
        var move = RequestMove(_currentColor);
        if (!MakeMove(_currentColor, move)) return;
        SwitchColor(_currentColor);
        PrintBoard();
    }

    private void PrintBoard()
    {
        Debug.Log("Printing board state...");
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Debug.Log("Tile [" + j + "," + i + "] = " + _board[j, i]);
            }
        }
    }

    private void HandleWin()
    {
        _win = true;
        int blackCount = 0;
        int whiteCount = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_board[j, i] == TileState.Black) blackCount++;
                if (_board[j, i] == TileState.White) whiteCount++;
            }
        }

        if (blackCount > whiteCount)
        {
            UIScript.Instance.DisplayWin(TileState.Black);
        }
        else if(whiteCount > blackCount)
        {
            UIScript.Instance.DisplayWin(TileState.White);
        }
        else
        {
            UIScript.Instance.DisplayWin(TileState.Empty);
        }
    }

    private bool DetermineWin()
    {
        if (!GameRules.IsBoardPlayable(_board, TileState.Black) && !GameRules.IsBoardPlayable(_board, TileState.White))
        {
            return true;
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_board[j, i] == TileState.Empty) return false;
            }
        }
        return true;
    }


    private bool MakeMove(TileState color, Index move)
    {
        if (!GameRules.IsPlayable(_board, move, color)) return false;
        _board[move.Z, move.X] = color;
        _tileGameObjects[move.Z, move.X].transform.GetComponent<TileScript>().PlaceTile(color, true);
        _board = GameRules.SimulateTurn(_board, move, color);
        RefreshTiles();
        aiTimer = aiDelay;
        return true;

    }

    private Index RequestMove(TileState currentColor)
    {
        switch (currentColor)
        {
            case TileState.Black:
                return playerOneType == PlayerType.Agent ? RequestAgentMove(TileState.Black) : RequestPlayerMove(TileState.Black);
            case TileState.White:
                return playerTwoType == PlayerType.Agent ? RequestAgentMove(TileState.White) : RequestPlayerMove(TileState.White);
            case TileState.Empty:
            default:
                Debug.LogError("Color is neither black or white.");
                return new Index(width, height);
        }
        
    }

    private Index RequestPlayerMove(TileState color)
    {
        if (!Input.GetMouseButtonDown(0)) return new Index(width, height);
        if (Camera.main == null) return new Index(width, height);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out var hit, 100) ? hit.transform.GetComponent<TileScript>().GetIndex() : new Index(width, height);
    }

    private Index RequestAgentMove(TileState color)
    {
        aiTimer -= Time.deltaTime;
        return aiTimer < 0 ? AI.CalculateNextMove(_board, color, depth) : new Index(width, height);
    }

    private void ChangeColor(TileState changeToColor)
    {
        UIScript.Instance.DisplayTurn(changeToColor);
        _currentColor = changeToColor;
    }

    private void SwitchColor(TileState switchFromColor)
    {
        switch (switchFromColor)
        {
            case TileState.Black:
                ChangeColor(TileState.White);
                break;
            case TileState.White:
                ChangeColor(TileState.Black);
                break;
            case TileState.Empty:
            default:
                break;
        }
    }
    public void PlayerVsPlayer()
    {
        playerOneType = PlayerType.Human;
        playerTwoType = PlayerType.Human;
    }

    public void PlayerVsAgent()
    {
        playerOneType = PlayerType.Agent;
        playerTwoType = PlayerType.Human;
    }

    public void AgentVsAgent()
    {
        playerOneType = PlayerType.Agent;
        playerTwoType = PlayerType.Agent;
    }

    private void SpawnBoard(Vector3 origin, int width, int height)
    {
        _board = new TileState[width, height];
        _tileGameObjects = new GameObject[width, height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                SpawnTile(i, j);
            }
        }
    }

    private void SpawnTile(int xOffset, int zOffset)
    {
        Vector3 offset = new Vector3(xOffset, 0, zOffset);
        _board[zOffset, xOffset] = TileState.Empty;
        GameObject tile = Instantiate(tilePrefab, gameObject.transform);
        tile.transform.position += offset;
        tile.GetComponent<TileScript>().SetIndex(zOffset, xOffset);
        _tileGameObjects[zOffset, xOffset] = tile;
    }

    private void SetStartPieces(int width, int height)
    {
        _board[(width / 2) - 1, (height / 2) - 1] = TileState.White;
        _board[width / 2, height / 2] = TileState.White;
        _board[width / 2, (height / 2) - 1] = TileState.Black;
        _board[(width / 2) - 1, height / 2] = TileState.Black;
        RefreshTiles();

    }

    private void RefreshTiles()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_tileGameObjects[j, i].transform.GetComponent<TileScript>().GetTileState() == TileState.Empty)
                {
                    _tileGameObjects[j, i].transform.GetComponent<TileScript>().PlaceTile(_board[j, i], false);
                }
                else
                {
                    if (_tileGameObjects[j, i].transform.GetComponent<TileScript>().GetTileState() != _board[j, i]) _tileGameObjects[j, i].transform.GetComponent<TileScript>().TurnTile(_board[j, i]);
                }
            }
        }
    }

    private void ResetBoard()
    {
        ChangeColor(TileState.Black);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                _board[j, i] = TileState.Empty;
            }
        }
        aiTimer = aiDelay;
        _reset = false;
        _win = false;
        SetStartPieces(width, height);
    }

    public void RequestReset()
    {
        _reset = true;
    }
}
