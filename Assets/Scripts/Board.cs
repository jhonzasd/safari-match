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
    // Start is called before the first frame update
    void Start()
    {
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
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                o.GetComponent<Piece>()?.Setup(x, y, this);
                
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                o.GetComponent<Tile>()?.Setup(x, y, this);
                
            }
        }
    }
}
