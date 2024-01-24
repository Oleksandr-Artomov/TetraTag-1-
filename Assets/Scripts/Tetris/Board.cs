using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


public class Board : Actor
{
    [Serializable]
    public class Keybind
    {
        public Key key;
        public GamepadButton button;

        public static implicit operator Key(Keybind myObj)
        {
            return myObj.key;
        }

        public static implicit operator GamepadButton(Keybind myObj)
        {
            return myObj.button;
        }
    }

    public Keybind LeftRotate;
    public Keybind RightRotate;
    public Keybind HardDrop;
    public Keybind SoftDrop;
    public Keybind MoveRight;
    public Keybind MoveLeft;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0); //SPAWN----------
    public Vector2Int boardSize = new Vector2Int(10, 20); // BOARD SIZE----------
    public float LastDroppedPieceStepDelay { get; private set; }

    private Tetromino nextTetromino;
    private Tetromino secondNextTetromino;
    private Tetromino thirdNextTetromino;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
        SetNextTetromino(); // Set the initial next tetromino
    }


    public void SpawnPiece()
    {
        TetrominoData data = tetrominoes[(int)nextTetromino]; // Use the next tetromino
        activePiece.Initialize(this, spawnPosition, data, SetNextTetromino());

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
            SetNextTetromino(); // Set the next next tetromino after spawning the current one
        }
        else
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        tilemap.ClearAllTiles();
        SystemManager.Get<PlayerManager>().OnAllPlayersWon.Invoke();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public Tetromino GetNextTetromino()
    {
        return nextTetromino;
    }

    public Tetromino[] SetNextTetromino()
    {
        // Get the next three tetrominos
        Tetromino[] nextTetrominos = new Tetromino[3];
        for (int i = 0; i < 3; i++)
        {
            nextTetrominos[i] = GetRandomTetromino();
        }

        // Save the nextTetrominos for reference
        this.nextTetromino = nextTetrominos[0];
        this.secondNextTetromino = nextTetrominos[1];
        this.thirdNextTetromino = nextTetrominos[2];

        // Set the UI text for the next three tetrominos
        UpdateNextTetrominoDisplay();

        return nextTetrominos;
    }


    // Your existing GetRandomTetromino method
    private Tetromino GetRandomTetromino()
    {
        Array values = Enum.GetValues(typeof(Tetromino));
        return (Tetromino)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }



    public List<string> GetNextTetrominoTexts()
    {
        List<string> nextTetrominos = new List<string>();

        // Get the next three tetrominos
        Tetromino[] nextTetrominosArray = new Tetromino[3];
        for (int i = 0; i < 3; i++)
        {
            nextTetrominosArray[i] = GetRandomTetromino();
        }

        // Convert the tetrominos to their string representations
        foreach (Tetromino tetromino in nextTetrominosArray)
        {
            nextTetrominos.Add(tetromino.ToString());
        }

        return nextTetrominos;
    }
    private void UpdateNextTetrominoDisplay()
    {
        if (activePiece != null && activePiece.nextTetrominoText != null)
        {
            List<string> nextTetrominos = new List<string>
            {
                nextTetromino.ToString(),
                secondNextTetromino.ToString(),
                thirdNextTetromino.ToString()
            };

            // Set the UI text for the next three tetrominos
            for (int i = 0; i < activePiece.nextTetrominoText.Length; i++)
            {
                if (i < nextTetrominos.Count)
                {
                    activePiece.nextTetrominoText[i].text = " " + nextTetrominos[i];
                }
                else
                {
                    // If there are fewer next tetrominos than available Text components, clear the text
                    activePiece.nextTetrominoText[i].text = "";
                }
            }
        }
    }
}