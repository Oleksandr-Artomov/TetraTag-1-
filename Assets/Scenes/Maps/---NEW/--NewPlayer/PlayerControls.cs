using UnityEngine.InputSystem;

public class PlayerControls
{
    public PlayerInput playerInput;

    public InputAction Move;
    public InputAction Jump;

    public PlayerControls(PlayerInput input)
    {
        playerInput = input;
        Move = playerInput.actions["Move"];
        Jump = playerInput.actions["Jump"];
    }
}