using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Передвижение")]
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode run = KeyCode.LeftShift;

    [Header("Взаимодействие")]
    public KeyCode interact = KeyCode.Mouse0;
    public KeyCode placeOrThrow = KeyCode.Mouse1;

    [Header("Настройки")]
    public float mouseSensitivity = 2.0f;
    public bool invertY = false;
    public bool lockCursor = true;
    public bool useCursor = true;

    [Header("Интерфейс")]
    [SerializeField] private GameObject ClosePanel;

    public void SetUseCursor(bool useCursor) { this.useCursor = useCursor; }
    public event Action OnCursorLockChanged;

    private bool isCanBePaused;
    public void SetCanBePaused(bool canBePaused) { isCanBePaused = canBePaused; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        if (lockCursor) SetCursorLocked(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
            SetCursorLocked(false);

        }
        if (!useCursor && Input.GetMouseButtonDown(0) && !lockCursor)
        {
            SetCursorLocked(true);
        }
    }
    private void PauseToggle()
    {
        if (!isCanBePaused) return;

        bool isPaused = !ClosePanel.activeSelf;
        ClosePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        SetCursorLocked(!isPaused);
        SetUseCursor(isPaused);
    }
    public Vector2 GetMouseDelta()
    {
        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }

    public Vector2 GetMoveInput()
    {
        Vector2 v = Vector2.zero;
        if (Input.GetKey(forward)) v.y += 1;
        if (Input.GetKey(back)) v.y -= 1;
        if (Input.GetKey(right)) v.x += 1;
        if (Input.GetKey(left)) v.x -= 1;
        v = Vector2.ClampMagnitude(v, 1f);
        return v;
    }

    public bool GetInteractDown() => Input.GetKeyDown(interact);
    public bool GetInteractUp() => Input.GetKeyUp(interact);
    public bool GetPlaceOrThrowDown() => Input.GetKeyDown(placeOrThrow);
    public bool GetPlaceOrThrowUp() => Input.GetKeyUp(placeOrThrow);
    public bool GetPlaceOrThrow() => Input.GetKey(placeOrThrow);
    public bool GetIsRun() => Input.GetKey(run);

    public void SetCursorLocked(bool locked)
    {
        lockCursor = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
        OnCursorLockChanged?.Invoke();
    }
}
