using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int x, y;
    public Board board;

    [Tooltip("Animales")] 
    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    };

    public void Setup(int x_, int y_, Board board_ )
    {
        x = x_;
        y = y_;
        board = board_;
    }

    public type pieceType;
}
