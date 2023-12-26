using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public int width, height;
    public GameObject tileObject;

    public float cameraSizeOffset;
    public float cameraVerticalOffset;

    public GameObject[] availablePieces;

    private Tile[,] Tiles;

    private Piece[,] Pieces;

    private Tile startTile;
    private Tile endTile;
    
    // Start is called before the first frame update
    void Start()
    {
        Tiles = new Tile [width, height]; // Inicializamos el array de espacios con el ancho y alto del tablero
        Pieces = new Piece [width, height]; // Inicializamos el array de piezas con el ancho y el alto del tablero
        SetupBoard();
        PositionCamera();
        SetupPieces();

    }

    private void SetupPieces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
                var o = Instantiate(selectedPiece, new Vector3(x, y, 1), Quaternion.identity);
                o.transform.parent = transform;
                Pieces[x, y] = o.GetComponent<Piece>();
                Pieces[x,y].Setup(x, y, this);
                
                
            }
        }
    }

    private void PositionCamera()
    {
        float newPosX = (float)width / 2f;
        float newPosY = (float)height / 2f;

        Camera.main.transform.position = new Vector3(newPosX - 0.5f, newPosY - 0.5f + cameraVerticalOffset, -10f);
        float horizontal = width + 1;
        float vertical = (height / 2f) + 1;
        Camera.main.orthographicSize = horizontal > vertical ? horizontal + cameraSizeOffset: vertical + cameraSizeOffset;
        
    }
    

    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                Tiles[x, y] = o.GetComponent<Tile>(); // Se guarda una referencia a los espacios creados.
                Tiles[x, y]?.Setup(x, y, this);
                
            }
        }
    }

    public void TileDown(Tile tile_)
    {
        startTile = tile_;
    }
    public void TileOver(Tile tile_)
    {
        endTile = tile_;
    }
    public void TileUp(Tile tile_)
    {
        if (startTile!=null && endTile != null)
        {
            SwapTiles();
        }

        startTile = null;
        endTile = null;
    }

    private void SwapTiles()
    {
        var StartPiece = Pieces[startTile.x, startTile.y]; // Se obtiene la posición de la pieza que estaba en el espacio incial
        var EndPiece = Pieces[endTile.x, endTile.y]; // Se obtiene la posición de la pieza que estaba en el espacio final
        
        StartPiece.Move(endTile.x, endTile.y);
        EndPiece.Move(startTile.x, startTile.y);

        Pieces[startTile.x, startTile.y] = EndPiece;
        Pieces[endTile.x, endTile.y] = StartPiece;
    }
}
