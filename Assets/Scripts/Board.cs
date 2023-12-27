using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private bool swappingPieces = false;
    
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
        int maxIterations = 50;
        int currentIteration = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                currentIteration = 0;

                var newPiece = CreatePieceAt(x, y);
                while (HasPreviousMatches(x, y))
                {
                    ClearPieceAt(x, y);
                    newPiece = CreatePieceAt(x, y);
                    currentIteration++;
                    if (currentIteration > maxIterations)
                    {
                        break;
                    }
                    
                }


            }
        }

        
    }

    private void ClearPieceAt(int x, int y)
    {
        var pieceToClear = Pieces[x, y];
        Destroy(pieceToClear.gameObject);
        Pieces[x, y] = null;
        
    }

    private Piece CreatePieceAt(int x, int y)
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o = Instantiate(selectedPiece, new Vector3(x, y, 1), Quaternion.identity);
        o.transform.parent = transform;
        Pieces[x, y] = o.GetComponent<Piece>();
        Pieces[x,y].Setup(x, y, this);
        
        return Pieces[x,y];
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
                Tiles[x, y] = o.GetComponent<Tile>(); // Se guarda una referencia a los espacios creados
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
        if (startTile!=null && endTile != null && IsCloseTo(startTile, endTile))
        {
            StartCoroutine(SwapTiles());
        }

        
    }

    private IEnumerator SwapTiles()
    {
        var StartPiece = Pieces[startTile.x, startTile.y]; // Se obtiene la posición de la pieza que estaba en el espacio incial
        var EndPiece = Pieces[endTile.x, endTile.y]; // Se obtiene la posición de la pieza que estaba en el espacio final
        
        StartPiece.Move(endTile.x, endTile.y);
        EndPiece.Move(startTile.x, startTile.y);

        Pieces[startTile.x, startTile.y] = EndPiece;
        Pieces[endTile.x, endTile.y] = StartPiece;

        yield return new WaitForSeconds(0.6f);
        
        var startMatches = GetMatchByPiece(startTile.x, startTile.y, 3); // Busca los matches de la pieza inicial
        var endMatches = GetMatchByPiece(endTile.x, endTile.y, 3); // Busca los matches de la pieza final


        var allMatches = startMatches.Union(endMatches).ToList();
        

        if (allMatches.Count == 0)
        {
            StartPiece.Move(startTile.x, startTile.y);
            EndPiece.Move(endTile.x, endTile.y);
            Pieces[startTile.x, startTile.y] = StartPiece;
            Pieces[endTile.x, endTile.y] = EndPiece;
        }
        else
        {
            ClearPieces(allMatches);
        }
        
        startTile = null;
        endTile = null;
        swappingPieces = false;

        yield return null;

    }

    private void ClearPieces(List<Piece> piecesToClear)
    {
        piecesToClear.ForEach(piece =>
        {
            ClearPieceAt(piece.x, piece.y);
        });
        List<int>columns = GetColumms(piecesToClear);
        List<Piece> collapsedPieces = collapseColumns(columns, 0.3f);
    }
    
    private List<Piece> collapseColumns(List<int> columns, float timeToCollapse)
    {
        List<Piece> movingPieces = new List<Piece>();

        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            for (int y = 0; y < height; y++)
            {
                if (Pieces[column, y] == null)
                {
                    for (int yplus = y + 1; yplus < height; y++)
                    {
                        if (Pieces[column, yplus] != null)
                        {
                            Pieces[column, yplus].Move(column, y);
                            Pieces[column, y] = Pieces[column, yplus];
                            if (!movingPieces.Contains(Pieces[column, y]))
                            {
                                movingPieces.Add(Pieces[column, y]);
                            }

                            Pieces[column, yplus] = null;
                            break;

                        }
                    }
                }
            }
        }

        return movingPieces;
    }


    private List<int> GetColumms(List<Piece> piecesToClear)
    {
        var result = new List<int>();
        piecesToClear.ForEach(piece =>
        {
            if (!result.Contains(piece.x))
            {
                result.Add(piece.x);
            }
        });
        return result;
    }

    public bool IsCloseTo(Tile start, Tile end)
    {
        if(Math.Abs((start.x-end.x))==1 && start.y == end.y)
        {
            return true;
        }

        if (Math.Abs(start.y - end.y) == 1 && start.x == end.x)
        {
            return true;
        }

        return false;
    }

    bool HasPreviousMatches(int posx, int posy)
    {
        var downMatches = GetMatchByDirection(posx, posy, new Vector2(0, -1), 2);
        var leftMatches = GetMatchByDirection(posx, posy, new Vector2(-1, 0), 2);

        if (downMatches == null) downMatches = new List<Piece>();
        if (leftMatches == null) leftMatches = new List<Piece>();
        return (downMatches.Count > 0 || leftMatches.Count > 0);
    }   
    

    public List<Piece> GetMatchByDirection(int xpos, int ypos, Vector2 direction, int minPieces = 3)
    {
        List<Piece> matches = new List<Piece>();
        Piece startPiece = Pieces[xpos, ypos];
        matches.Add(startPiece);

        int nextX;
        int nextY;
        int maxVal = width > height ? width : height;

        for (int i = 1; i < maxVal; i++)
        {
            nextX = xpos + ((int)direction.x * i);
            nextY = ypos + ((int)direction.y * i);

            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {
                var nextPiece = Pieces[nextX, nextY];
                if (nextPiece != null && nextPiece.pieceType == startPiece.pieceType)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }

            }
        }

        if (matches.Count >= minPieces)
        {
            return matches;
        }

        return null;
    }

    public List<Piece> GetMatchByPiece(int xpos, int ypos, int minPieces = 3)
    {
        var upMatchs = GetMatchByDirection(xpos, ypos, 
            new Vector2(0, 1), 2);
        var downMatchs = GetMatchByDirection(xpos, ypos, 
            new Vector2(0, -1), 2);
        var rightMatchs = GetMatchByDirection(xpos, ypos, 
            new Vector2(1, 0), 2);
        var leftMatchs = GetMatchByDirection(xpos, ypos, 
            new Vector2(-1, 0), 2);

        if (upMatchs == null) upMatchs = new List<Piece>();
        if (downMatchs == null) downMatchs = new List<Piece>();
        if (rightMatchs == null) rightMatchs = new List<Piece>();
        if (leftMatchs == null) leftMatchs = new List<Piece>();

        var verticalMatches = upMatchs.Union(downMatchs).ToList();
        var horizontalMatches = leftMatchs.Union(rightMatchs).ToList();

        var foundMatches = new List<Piece>();

        if (verticalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(verticalMatches).ToList();
        }

        if (horizontalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(horizontalMatches).ToList();
        }

        return foundMatches;


    }
    
    
        
    
}
