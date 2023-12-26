using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public void Move(int desX, int desY)
    {
        transform.DOMove(new Vector3(desX, desY, -5f), 0.25f).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            x = desX;
            y = desY;
        };
    }

    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0,0);
    }
}
