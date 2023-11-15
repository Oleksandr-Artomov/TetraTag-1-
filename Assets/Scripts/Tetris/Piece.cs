using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class Piece : MonoBehaviour
{

    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f; //Block is floating ------------- 

    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    public float lockTime; //Block is placed ---------

    private float timeElapsed = 0f;
    private float timeSinceLastDebug = 0f;

    public float speedIncreaseRate = 20f; // Adjust this rate for your desired speed increase (3% increase per second)


    private float lastDroppedPieceStepDelay; // Store the stepDelay of the last dropped piece
    private bool isHardDropping = false; // Add this flag


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        // Set the stepDelay to the value of the last dropped piece


        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        this.tag = "Board";

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    struct Inputs
    {
        public bool leftRotate;
        public bool rightRotate;
        public bool hardDrop;
        public bool softDrop;
        public bool moveRight;
        public bool moveLeft;
    }

    Inputs HandleInputs()
    {
        Inputs inputs = new();

        var device = InputSystem.GetDeviceById(board.DeviceID);

        if (device is Gamepad gamepad)
        {
            inputs.leftRotate = gamepad[board.LeftRotate].wasPressedThisFrame;
            inputs.rightRotate = gamepad[board.RightRotate].wasPressedThisFrame;
            inputs.hardDrop = gamepad[board.HardDrop].isPressed; /*wasPressedThisFrame;*/
            inputs.softDrop = gamepad[board.SoftDrop].isPressed;
            inputs.moveRight = gamepad[board.MoveRight].isPressed;
            inputs.moveLeft = gamepad[board.MoveLeft].isPressed;
        }

        Keyboard keyboard = Keyboard.current;

        inputs.leftRotate |= keyboard[board.LeftRotate].wasPressedThisFrame;
        inputs.rightRotate |= keyboard[board.RightRotate].wasPressedThisFrame;
        inputs.hardDrop |= keyboard[board.HardDrop].isPressed; /*wasPressedThisFrame;*/
        inputs.softDrop |= keyboard[board.SoftDrop].isPressed;
        inputs.moveRight |= keyboard[board.MoveRight].isPressed;
        inputs.moveLeft |= keyboard[board.MoveLeft].isPressed;

        return inputs;
    }
    private void Update()
    {
        board.Clear(this);

        // Check if it's time to increase the speed
        // Update the time elapsed
        timeElapsed += Time.deltaTime;

        // Display the current piece falling speed every second
        timeSinceLastDebug += Time.deltaTime;

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        lockTime += Time.deltaTime;
        var inputs = HandleInputs();

        // Handle rotation outside of the hard drop phase
        if (!isHardDropping)
        {
            if (inputs.leftRotate)
            {
                Rotate(-1);
            }
            else if (inputs.rightRotate)
            {
                Rotate(1);
            }
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > moveTime)
        {
            // Left/right movement
            if (!isHardDropping)
            {
                if (inputs.moveLeft)
                {
                    Move(Vector2Int.left);
                }
                else if (inputs.moveRight)
                {
                    Move(Vector2Int.right);
                }
            }

            // Handle hard drop
            if (inputs.hardDrop)
            {
                isHardDropping = true; // Set the flag to indicate hard drop in progress
            }

            // Continue moving downward if hard drop is in progress
            if (isHardDropping && Move(Vector2Int.down))
            {
                // Update the step time to prevent double movement
                stepTime = Time.time + stepDelay;
            }
            else if (isHardDropping)
            {
                // Lock the piece in place when hard drop is complete
                Lock();
                isHardDropping = false; // Reset the flag
            }
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(this);
    }



    private void Step()
    {
        stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (lockTime >= lockDelay)
        {
            Lock();
        }
        else
        {

        }
    }

    /* 
     * private void HardDrop()
    {
        while (Move(Vector2Int.down))
         {
             continue;
         }

         Lock();
    }
    */

    private void Lock()
    {
        // Shake the blocks when locked
        StartCoroutine(ShakeBlocks());

        // Rest of your existing code...

        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();

        // Store the stepDelay of the last dropped piece
        lastDroppedPieceStepDelay = stepDelay;

        // Lower the stepDelay to increase falling speed, even when soft dropping
        stepDelay -= speedIncreaseRate * Time.deltaTime;

        // Ensure stepDelay doesn't go below a minimum value
        stepDelay = Mathf.Max(stepDelay, 0.1f); // Adjust the minimum value as needed
    }

    private IEnumerator ShakeBlocks()
    {
        float shakeDuration = 0.3f;
        float shakeMagnitude = 0.1f;

        Vector3 originalPosition = transform.position;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = originalPosition.y + Random.Range(-shakeMagnitude, shakeMagnitude);

            transform.position = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Reset the position after shaking
        transform.position = originalPosition;
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}