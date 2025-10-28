using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public bool isCanMove = true;
    public bool isCanMoveLook = true;
    [Header("Компоненты")]
    [SerializeField] private Transform cameraTransform;

    [Header("Настройки")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool instantTargetRotate = false;

    [Header("Звуки")]
    [SerializeField] private AudioClip escapeSound;
    [SerializeField] private AudioClip LoseSound;

    [Header("События")]
    [SerializeField] private UnityEvent OnLose;

    private CharacterController cc;
    private float verticalVelocity = 0f;
    private float pitch = 0f;
    private float yaw = 0f;
    private Transform lookTarget;
    private bool isGameOver;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.localEulerAngles.x;
        InputManager.Instance.OnCursorLockChanged += OnCursorLockChanged;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnCursorLockChanged -= OnCursorLockChanged;
    }

    private void OnCursorLockChanged()
    {
        AudioManager.Instance.PlaySoundGlobal(escapeSound, 0.1f);
    }

    private void Update()
    {
        if (lookTarget == null)
        {
            if (isCanMoveLook)
                HandleLook();
        }
        else
        {
            LookingAtTarget();
        }

        if (isCanMove)
            HandleMove();
    }

    private void HandleLook()
    {
        if (InputManager.Instance == null) return;

        Vector2 md = InputManager.Instance.GetMouseDelta() * InputManager.Instance.mouseSensitivity;
        yaw += md.x;
        pitch += (InputManager.Instance.invertY ? md.y : -md.y);
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMove()
    {
        if (InputManager.Instance == null) return;

        Vector2 inp = InputManager.Instance.GetMoveInput();
        Vector3 forward = transform.forward * inp.y;
        Vector3 right = transform.right * inp.x;
        Vector3 move = (forward + right).normalized;

        float speed = walkSpeed;
        if (InputManager.Instance.GetIsRun()) speed = runSpeed;

        if (cc.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 velocity = move * speed + Vector3.up * verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    public void LookAtTarget(Transform target)
    {
        lookTarget = target;
        isCanMove = false;
    }
    public void StopLookTarget()
    {
        lookTarget = null;
        isCanMove = true;
    }
    public void LookingAtTarget()
    {
        if (lookTarget == null) return;

        Vector3 dir = lookTarget.position - transform.position;
        dir.y = cameraTransform.position.y;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        if (instantTargetRotate)
        {
            transform.rotation = targetRot;
            yaw = transform.eulerAngles.y;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 5f
            );
            yaw = transform.eulerAngles.y;
        }

        Vector3 lookDir = (lookTarget.position - cameraTransform.position).normalized;
        Quaternion camRot = Quaternion.LookRotation(lookDir, Vector3.up);

        pitch = camRot.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    public void Lose(Monster monster)
    {
        if (isGameOver) return;
        isGameOver = true;
        LockMovement(false);
        LookAtTarget(monster.transform);
        AudioManager.Instance.PlaySoundGlobal(LoseSound, 0.075f);
        InputManager.Instance.SetCursorLocked(false);
        InputManager.Instance.SetUseCursor(true);

        OnLose.Invoke();

        Invoke("RestartGame", 5);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LockMovement(bool isCanMove)
    {
        this.isCanMove = isCanMove;
    }
    public void LockLookMove(bool isCanMoveLook)
    {
        this.isCanMoveLook = isCanMoveLook;
    }
    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
    }

}
