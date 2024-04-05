using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;


    private XOX [][] _board;

    [SerializeField]
    private Sprite _xSprite;
    [SerializeField]
    private Sprite _oSprite;

    [SerializeField]
    private Image[] _slots;

    [SerializeField]
    private Text _gameStatusText;

    private void Awake()
    {
        _board = new XOX[3][];
        for (int i = 0; i < 3; i++)
        {
            _board[i] = new XOX[3];
            for (int j = 0; j < 3; j++)
            {
                _board[i][j] = XOX.N;
            }
        }

        MakeSingleton();
    }


    private bool _wait;

    private void GameStep()
    {
        if (_wait)
        {
            return;
        }
        GameStatus status = CheckGameStatus();
        if (status == GameStatus.Draw)
        {
            Debug.Log("Game Over Draw");
            _gameStatusText.text = "Draw";
            
            ResetGame();
            return;
        }
        else if (status == GameStatus.XWin)
        {
            Debug.Log("Game Over X Win");
            _gameStatusText.text = "X Win";
            ResetGame();
            return;
        }
        else if (status == GameStatus.OWin)
        {
            Debug.Log("Game Over O Win");
            _gameStatusText.text = "O Win";
            ResetGame();
            return;
        }
    }



    public void ResetGame()
    {
        _sequence = 0;
        StartCoroutine(ResetGameRoutine());

    }

    private IEnumerator ResetGameRoutine()
    {
        _gameStatusText.gameObject.SetActive(true);
        _wait = true;

        Color c = _gameStatusText.color;
        Vector3 s = Vector3.one/2;
        c.a = 0;
        _gameStatusText.color = c;
        _gameStatusText.transform.localScale = s;
        float time = 1f;
        float elapsedTime = 0;
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (elapsedTime<time)
        {
            elapsedTime+=Time.deltaTime;
            c.a = (1 / time) * elapsedTime;
            _gameStatusText.color = c;
            s = (Vector3.one / 2 ) + ((Vector3.one/2) / time) * elapsedTime;
            _gameStatusText.transform.localScale = s;
            yield return waitForEndOfFrame;
        }

        yield return new WaitForSeconds(0.5f);

        _board = new XOX[3][];
        for (int i = 0; i < 3; i++)
        {
            _board[i] = new XOX[3];
            for (int j = 0; j < 3; j++)
            {
                _board[i][j] = XOX.N;
            }
        }

        _sequence = 0;
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].sprite = null;
        }

        _wait = false;
        _gameStatusText.gameObject.SetActive(false);
    }



    private int _sequence = 0;

    public void Play(int r, int c, XOX value)
    {

        if (_wait)
        {
            return;
        }

        Debug.Log("Played " + (IsAITurn ? "ai" : "human")+" r: "+r+" c:"+c);
        if (_board[r][c] != XOX.N)
        {
            Debug.LogError("Error "+" r: "+r+" c: "+c);
            return;
        }

        
        

        _board[r][c] = value;

        string gameBoard = "";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                gameBoard += " " + _board[i][j];
            }
            gameBoard += "\n"; 
        }
        Debug.Log(gameBoard);
        _sequence++;

        _slots[r * 3 + c].sprite = value == XOX.X ? _xSprite : _oSprite;

        GameStep();

    }


    public void Play(int rc)
    {

        if (Board[rc / 3][rc % 3] != XOX.N)
        {
            return;
        }

        Play(rc/3, rc%3, XOX.X);

    }


    public bool IsAITurn
    {
        get { return _sequence %2==1; }
    }


    public GameStatus CheckGameStatus()
    {


        bool row0 = _board[0][0] != XOX.N && _board[0][0] == _board[0][1] && _board[0][0] == _board[0][2];
        if (row0)
        {
            Debug.Log("row 0");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
        }
            
        bool row1 = _board[1][0] != XOX.N && _board[1][0] == _board[1][1] && _board[1][0] == _board[1][2];
        if (row1)
        {
            Debug.Log("row 1");
            return (GameStatus)XOXHelper.XoxValuePair[_board[1][0]];
        }
        bool row2 = _board[2][0] != XOX.N && _board[2][0] == _board[2][1] && _board[2][0] == _board[2][2];
        if (row2)
        {
            Debug.Log("row 2");
            return (GameStatus)XOXHelper.XoxValuePair[_board[2][0]];
        }
            

        bool col0 = _board[0][0] != XOX.N && _board[0][0] == _board[1][0] && _board[0][0] == _board[2][0];
        if (col0)
        {
            Debug.Log("col 0");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
        }
            
        bool col1 = _board[0][1] != XOX.N && _board[0][1] == _board[1][1] && _board[0][1] == _board[2][1];
        if (col1)
        {
            Debug.Log("col 1");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][1]];
        }
            
        bool col2 = _board[0][2] != XOX.N && _board[0][2] == _board[1][2] && _board[0][2] == _board[2][2];
        if (col2)
        {
            Debug.Log("col 2");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][2]];
        }
            

        bool cross0 = _board[0][0] != XOX.N && _board[0][0] == _board[1][1] && _board[0][0] == _board[2][2];
        if (cross0)
        {
            Debug.Log("cross 0");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
        }
        bool cross1 = _board[0][2] != XOX.N && _board[0][2] == _board[1][1] && _board[0][2] == _board[2][0];
        if (cross1)
        {
            Debug.Log("cross 1");
            return (GameStatus)XOXHelper.XoxValuePair[_board[0][2]];
        }

        if (_sequence == 9)
        {
            return GameStatus.Draw;
        }

        return (GameStatus)(-5);
    }



    public static GameManager Instance { get => _instance; set => _instance = value; }
    public XOX[][] Board { get => _board; set => _board = value; }

    private void MakeSingleton()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}

public enum GameStatus
{
    Running = -5,
    Draw = 0,
    XWin = 1,
    OWin = -1
}


public enum XOX
{
    N = 'N',
    X = 'X',
    O = 'O',

}

public class XOXHelper
{
    private static Dictionary<XOX, int> _xoxValuePair=new Dictionary<XOX, int>() {
        {XOX.N, 0},
        {XOX.X, 1},
        {XOX.O, -1}
    };

    public static Dictionary<XOX, int> XoxValuePair { get => _xoxValuePair; }
}
