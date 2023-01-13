using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAI : MonoBehaviour
{



    private enum Difficulty
    {
        Easy = 0,
        Hard = 1,
        Impossible = 2
    }

    private Difficulty _difficulty = Difficulty.Easy;

    [SerializeField]
    private Dropdown _dropdown;


    public void OnDropdownChange()
    {
        Debug.Log(_dropdown.value);
        _difficulty = (Difficulty)_dropdown.value;
    }


    void Start()
    {
        Configurations();
    }


    private void Update()
    {
        if (GameManager.Instance.IsAITurn)
        {

            Play();

        }

        

    }


    public void Play()
    {

        string id = Node.CreateID(GameManager.Instance.Board);
        Node currentNode = Node.IdNodePair[id];
        Debug.Log(currentNode.Id);
        foreach (var item in currentNode.Childs)
        {
            Debug.Log("  "+item.Id+"->"+item.Value);
        }


        if(_difficulty == Difficulty.Easy)
        {
            int r = UnityEngine.Random.Range(0, currentNode.Childs.Count);
            Vector2Int rowCol = currentNode.Childs[r].GetRowCol(currentNode.Id);

            Debug.Log("Random SELECTED : " + currentNode.Childs[r].Id + "->" + rowCol.x + " " + rowCol.y);
            GameManager.Instance.Play(rowCol.x, rowCol.y, XOX.O);
            return;
        }

        if(_difficulty == Difficulty.Hard)
        {
            int playRandom = UnityEngine.Random.Range(0, 100);
            if (playRandom < 20f)
            {
                int r = UnityEngine.Random.Range(0, currentNode.Childs.Count);
                Vector2Int rowCol = currentNode.Childs[r].GetRowCol(currentNode.Id);

                Debug.Log("Random SELECTED : " + currentNode.Childs[r].Id + "->" + rowCol.x + " " + rowCol.y);
                GameManager.Instance.Play(rowCol.x, rowCol.y, XOX.O);
                return;
            }

        }


        // Is there a child with value O win
        List<Node> childValueO = currentNode.ChildsOWin;
        if (childValueO.Count != 0)
        {
            int r = UnityEngine.Random.Range(0, childValueO.Count);
            Vector2Int rowCol =  childValueO[r].GetRowCol(currentNode.Id);

            Debug.Log("Win SELECTED : " + childValueO[r].Id + "->" + rowCol.x + " " + rowCol.y);
            GameManager.Instance.Play(rowCol.x, rowCol.y, XOX.O);
            
            return;
        }

        // Is there a child with value draw
        List<Node> childValueDraw = currentNode.ChildsDraw;
        if (childValueDraw.Count != 0)
        {
            List<Node> childs = new List<Node>();
            for (int i = 0; i < childValueDraw.Count; i++)
            {
                if (childValueDraw[i].HasOWinChild)
                {
                    childs.Add(childValueDraw[i]);
                }
            }

            if(childs.Count > 0)
            {
                Debug.Log("hi");
                int ri = UnityEngine.Random.Range(0, childs.Count);
                Vector2Int rowColi = childs[ri].GetRowCol(currentNode.Id);

                Debug.Log("     SELECTED : " + childs[ri].Id + "->" + rowColi.x + " " + rowColi.y);
                GameManager.Instance.Play(rowColi.x, rowColi.y, XOX.O);
                return;
            }

            int r = UnityEngine.Random.Range(0, childValueDraw.Count);
            Vector2Int rowCol = childValueDraw[r].GetRowCol(currentNode.Id);

            Debug.Log("2 SELECTED : " + childValueDraw[r].Id + "->" + rowCol.x + " " + rowCol.y);
            GameManager.Instance.Play(rowCol.x, rowCol.y, XOX.O);
            
            return;
        }

        // Is there a child with value X win;
        List<Node> childValueX = currentNode.ChildsXWin;
        if (childValueX.Count != 0)
        {
            int r = UnityEngine.Random.Range(0, childValueX.Count);
            Vector2Int rowCol = childValueX[r].GetRowCol(currentNode.Id);

            Debug.Log("SELECTED : " + childValueX[r].Id + "->" + rowCol.x + " " + rowCol.y);
            GameManager.Instance.Play(rowCol.x, rowCol.y, XOX.O);
            
            return;
        }

        Debug.Log("Error");


    }


    public void Configurations()
    {

        Queue<Node> queue = new Queue<Node>();


        Node root = new Node();
        queue.Enqueue(root);
        Node current;
        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            List<Vector2Int> emptySlots = current.EmptySlots;
            for (int i = 0;i < emptySlots.Count; i++)
            {
                string childID = current.CreateChildId(emptySlots[i].x, emptySlots[i].y, current.IsMax? XOX.X : XOX.O);
                if (Node.IdNodePair.ContainsKey(childID))
                {
                    current.Childs.Add(Node.IdNodePair[childID]);
                }
                else
                {
                    Node childNode = new Node(current.Board,emptySlots[i].x,emptySlots[i].y, current.IsMax ? XOX.X : XOX.O);
                    childNode.IsMax = !current.IsMax;
                    current.Childs.Add(childNode);
                    queue.Enqueue(childNode);
                }

            }
        }

        
        //Debug.Log(Node.idNodePair.Count);
        foreach (KeyValuePair<string, Node> entry in Node.IdNodePair)
        {
            //Debug.Log(entry.Key+"->"+entry.Value.CalcuteNodeValue());
            entry.Value.Value = entry.Value.CalcuteNodeValue();
        }




    }


    public class Node
    {
        private readonly static Dictionary<string, Node> _idNodePairs 
            = new Dictionary<string, Node>();

        private string id;

        private XOX[][] _board;

        private List<Node> _childs = new List<Node>();

        private List<Node> _childsXWin;
        private List<Node> _childsOWin;
        private List<Node> _childsDraw;

        private List<Vector2Int> _emptySlots;

        private bool _isMax;

        private int _value;

        

        public Node(XOX[][] board,int row, int column, XOX value)
        {       
            CopyBoard(board);
            this._board[row][column] = value;
            SetEmptySlots();
            id = CreateID(this._board);
            _idNodePairs.Add(id, this);
        }

        public Node()
        {
            _isMax = true;

            _emptySlots = new List<Vector2Int>();
            this._board = new XOX[3][];
            id = "";
            for (int i = 0; i < 3; i++)
            {
                _board[i] = new XOX[3];
                for (int j = 0; j < 3; j++)
                {
                    _board[i][j] = XOX.N;
                    _emptySlots.Add(new Vector2Int(i, j));
                    id += this._board[i][j];
                }
            }
            _idNodePairs.Add(id, this);

        }


        private void CopyBoard(XOX[][] board)
        {
            this._board = new XOX[3][];
            for (int i = 0; i < 3; i++)
            {
                this._board[i] = new XOX[3];
                for (int j = 0; j < 3; j++)
                {
                    this._board[i][j] = board[i][j];
                }
            }
            
        }

        private void SetEmptySlots()
        {
            _emptySlots = new List<Vector2Int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    id += this._board[i][j];
                    if (this._board[i][j] == XOX.N)
                    {
                        _emptySlots.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        public string CreateChildId(int row, int column, XOX value)
        {
            StringBuilder childID = new StringBuilder(id);
            childID[row * 3 + column] = (char)value;
            return childID.ToString();
        }

        public static string CreateID(XOX [][] board)
        {
            string id = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    id += board[i][j];
                }
            }
            return id;
        }


        private static readonly Dictionary<Node,int> nodeValuePairs = new Dictionary<Node,int>();

        public int CalcuteNodeValue()
        {

            if (nodeValuePairs.ContainsKey(this))
            {
                return nodeValuePairs[this];
            }

            GameStatus status = CheckGameStatus();
            if (status == GameStatus.Running && _childs.Count!=0)
            {
                if (_isMax)
                {
                    int max = -1;
                    for (int i = 0;i < _childs.Count; i++)
                    {
                        max = Math.Max(max,_childs[i].CalcuteNodeValue());
                    }
                    return max;
                }
                else
                {
                    int min = 1;
                    for (int i = 0; i < _childs.Count; i++)
                    {
                        min = Math.Min(min, _childs[i].CalcuteNodeValue());
                    }
                    return min;
                }
            }
            if(status == GameStatus.Running)
            {
                nodeValuePairs[this] = 0;
                return 0;
            }

            nodeValuePairs[this] = (int)status;
            return (int)status;
        }



        public GameStatus CheckGameStatus()
        {

            bool row0 = _board[0][0] != XOX.N && _board[0][0] == _board[0][1] && _board[0][0] == _board[0][2];
            if (row0)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
            }

            bool row1 = _board[1][0] != XOX.N && _board[1][0] == _board[1][1] && _board[1][0] == _board[1][2];
            if (row1)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[1][0]];
            }
            bool row2 = _board[2][0] != XOX.N && _board[2][0] == _board[2][1] && _board[2][0] == _board[2][2];
            if (row2)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[2][0]];
            }


            bool col0 = _board[0][0] != XOX.N && _board[0][0] == _board[1][0] && _board[0][0] == _board[2][0];
            if (col0)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
            }

            bool col1 = _board[0][1] != XOX.N && _board[0][1] == _board[1][1] && _board[0][1] == _board[2][1];
            if (col1)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][1]];
            }

            bool col2 = _board[0][2] != XOX.N && _board[0][2] == _board[1][2] && _board[0][2] == _board[2][2];
            if (col2)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][2]];
            }


            bool cross0 = _board[0][0] != XOX.N && _board[0][0] == _board[1][1] && _board[0][0] == _board[2][2];
            if (cross0)
            {
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][0]];
            }
            bool cross1 = _board[0][2] != XOX.N && _board[0][2] == _board[1][1] && _board[0][2] == _board[2][0];
            if (cross1)
            {
  
                return (GameStatus)XOXHelper.XoxValuePair[_board[0][2]];
            }

            return (GameStatus)(-5);
        }



        #region GetterSetter
        public int Value { get => _value; set => _value = value; }
        public string Id { get => id; }
        public XOX[][] Board { get => _board;}
        public List<Node> Childs { get => _childs; set => _childs = value; }
        public List<Vector2Int> EmptySlots { get => _emptySlots; set => _emptySlots = value; }


        public bool IsMax { get => _isMax; set => _isMax = value; }
        public Vector2Int GetRowCol(string parentID) {

            int r;
            int c;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    if(parentID[i*3+j] != Id[i*3+j])
                    {
                        return new Vector2Int(i, j);
                    }

                }

            }

            return Vector2Int.zero;
        }

        public static Dictionary<string, Node> IdNodePair { get => _idNodePairs;}
        public List<Node> ChildsXWin { get 
            {
                if (_childsXWin == null)
                {
                    _childsXWin = new List<Node>();
                    for (int i = 0; i < _childs.Count; i++)
                    {
                        if (_childs[i].Value == 1)
                        {
                            _childsXWin.Add(_childs[i]);
                        }
                    }
                }
                return _childsXWin;
            }  
        }
        public List<Node> ChildsOWin
        {
            get
            {
                if (_childsOWin == null)
                {
                    _childsOWin = new List<Node>();
                    for (int i = 0; i < _childs.Count; i++)
                    {
                        if (_childs[i].Value == -1)
                        {
                            _childsOWin.Add(_childs[i]);
                        }
                    }
                }
                return _childsOWin;
            }
        }
        public List<Node> ChildsDraw
        {
            get
            {
                if (_childsDraw == null)
                {
                    _childsDraw = new List<Node>();
                    for (int i = 0; i < _childs.Count; i++)
                    {
                        if (_childs[i].Value == 0)
                        {
                            _childsDraw.Add(_childs[i]);
                        }
                    }
                }
                return _childsDraw;
            }
        }

        public bool HasOWinChild
        {
            get
            {
                Queue<Node> queue = new Queue<Node>();
                for (int i = 0; i < Childs.Count; i++)
                {
                    queue.Enqueue(Childs[i]);
                }
                while (queue.Count > 0)
                {
                    Node node = queue.Dequeue();
                    if(node.Value == -1)
                    {
                        return true;
                    }else if(node.Value == 1)
                    {
                        continue;
                    }

                    for (int i = 0; i < node.Childs.Count; i++)
                    {
                        queue.Enqueue(node.Childs[i]);
                    }
                }

                return false;
            }
            
        }

        #endregion


    }





}
