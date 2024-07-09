using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerMovement;

    [SerializeField] private GameObject sprite;
    [SerializeField] private Rigidbody2D rb;

    private void Awake()
    {
        playerCntrls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerMovement = playerCntrls.Player.Movement;
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
    }
    private void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            rb.velocity = Vector2.right * Time.deltaTime;
        }
    }
}
