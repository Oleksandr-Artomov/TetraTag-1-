using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpriteManager : MonoBehaviour
{
    public static TetrominoSpriteManager Instance { get; private set; }

    public Dictionary<Tetromino, Sprite> tetrominoSprites;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeSprites();
    }

    private void InitializeSprites()
    {
        tetrominoSprites = new Dictionary<Tetromino, Sprite>();

        tetrominoSprites[Tetromino.I] = Resources.Load<Sprite>("Blocks/IBlock");
        tetrominoSprites[Tetromino.J] = Resources.Load<Sprite>("Blocks/JBlock");
        tetrominoSprites[Tetromino.L] = Resources.Load<Sprite>("Blocks/LBlock");
        tetrominoSprites[Tetromino.O] = Resources.Load<Sprite>("Blocks/OBlock");
        tetrominoSprites[Tetromino.S] = Resources.Load<Sprite>("Blocks/SBlock");
        tetrominoSprites[Tetromino.T] = Resources.Load<Sprite>("Blocks/TBlock");
        tetrominoSprites[Tetromino.Z] = Resources.Load<Sprite>("Blocks/ZBlock");
    }
}