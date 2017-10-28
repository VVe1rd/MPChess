using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour
{
    public Piece[,] pieces = new Piece[32, 32];
    public GameObject whiterookPrefab;
    public GameObject blackrookPrefab;
    public GameObject whitepawnPrefab;
    public GameObject blackpawnPrefab;

    public Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    public Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private bool isWhite;
    private bool isWhiteTurn;

    private Piece selectedPiece = null;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private void Start()
    {
        isWhiteTurn = true;
        selectedPiece = null;
        startDrag.x = 31;
        startDrag.y = 31;
        GenerateBoard();
    }

    private void Update()
    {
        UpdateMouseOver();

        // If it is my turn
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            if (Input.GetMouseButtonDown(0))
                SelectPiece(x, y);

            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log((int)startDrag.y);
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }
    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
    }
    private void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void SelectPiece(int x, int y)
    {
        // Out of bounce
        Piece p;
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            if ((x >=- 5 && x <= 12) && (y >= 2 && y <= 5))
            {
                Debug.Log("lol");
                p = pieces[x + 16, y + 16];
                if (p != null)
                {
                    selectedPiece = p;
                    startDrag = mouseOver;
                    Debug.Log(selectedPiece.name);
                }
            }
            return;
        }

        p = pieces[x, y];
        if (p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        //Multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //Out of bounds
        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);

            //startDrag = Vector2.zero;
            startDrag.x = 31;
            startDrag.y = 31;
            selectedPiece = null;
            return;
        }
        
        if (selectedPiece != null)
        {
            //If it has not moved
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                startDrag.x = 31;
                startDrag.y = 31;
                selectedPiece = null;
                return;
            }

            //Check if its a valid move
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                // Did we kill anything

                // If we did not kill anything
                if (pieces[x2, y2] != null)
                {
                    Piece p = pieces[x2, y2];
                    pieces[x2, y2] = null;
                    Destroy(p.gameObject);
                                  
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {            
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                startDrag.x = 31;
                startDrag.y = 31;
                selectedPiece = null;
                return;
            }
        }
    }
    private void EndTurn()
    {
        selectedPiece = null;
        startDrag = Vector2.zero;
        startDrag.x = 31;
        startDrag.y = 31;

        isWhiteTurn = !isWhiteTurn;
        CheckVictory();
    }
    private void CheckVictory()
    {

    }

    private void GenerateBoard()
    {
        // Generate White rook
        GenerateWhiteRook(0, 0);
        // Generate Black rook
        GenerateBlackRook(7, 7);

        // Generate White pawns
        for (int x=-5; x<=-3; x+=2)
        {
            for (int y=2; y<=5; y++)
            {
                GenerateWhitePawn(x, y);
            }
        }
        // Generate Black pawns
        for (int x = 10; x <= 12; x += 2)
        {
            for (int y = 2; y <= 5; y++)
            {
                GenerateBlackPawn(x, y);
            }
        }

    }
    private void GenerateWhiteRook(int x, int y)
    {
        GameObject go = Instantiate(whiterookPrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }
    private void GenerateBlackRook(int x, int y)
    {
        GameObject go = Instantiate(blackrookPrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }
    private void GenerateWhitePawn(int x, int y)
    {
        GameObject go = Instantiate(whitepawnPrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x+16, y+16] = p;
        MovePiece(p, x, y);
    }
    private void GenerateBlackPawn(int x, int y)
    {
        {
            GameObject go = Instantiate(blackpawnPrefab) as GameObject;
            go.transform.SetParent(transform);
            Piece p = go.GetComponent<Piece>();
            pieces[x + 16, y + 16] = p;
            MovePiece(p, x, y);
        }
    }
    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}
