using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public bool isPawn;

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        // If it is vertical or horizontal
        int deltaMoveX = Mathf.Abs(x1 - x2);
        int deltaMoveY = Mathf.Abs(y1 - y2);
        if (deltaMoveX * deltaMoveY != 0)
            return false;

        if (isWhite)
        {
            if (board[x2, y2] != null)
            {
                // If it is your piece
                Piece p = board[x2, y2];
                if (p.isWhite == isWhite)
                    return true;
                else return false;           
            }
            return true;
        }
        if (!isWhite)
        {
            if (board[x2, y2] != null)
            {
                // If it is your piece
                Piece p = board[x2, y2];
                if (p.isWhite == isWhite)
                    return true;
                else return false;
            }
            return true;
        }
        return false;
    }
}
