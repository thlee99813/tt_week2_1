using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float movespeed = 6f;

    

    [Header("PlayerObject")]
    public Rigidbody rb;

    [Header("Player Jump & Slide")]

    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float slideJumpUpForce = 7f;
    [SerializeField] private float slideJumpForwardForce = 3f;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private float slopeLimit = 35f;
    [SerializeField] private LayerMask groundMask = ~0;

    [SerializeField] private float slideForwardSpeed = 8f;
    [SerializeField] private float slideStrafeSpeed = 3.5f;

    private float spinX;
    private Tween SpinTween;

    
    [Header("Player Rotate")]

    [SerializeField] private float mouseSensitivity = 0.15f;
    private float yaw;
    

    private Vector3 inputMove;
    private Vector3 targetPos;
    private bool isMoving;

    public bool PlayerMoveLock = false;
    public bool PlayerRotateLock = false;
    private Quaternion lockRotation;

    
    private bool jumpRequest;
    private bool isGrounded;
    private bool isSliding;
    private bool isSlidingSurface;




    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.eulerAngles.y;

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(!PlayerMoveLock)
        {
            GetInput();  
        }
        else
        {
            inputMove = Vector3.zero;
        }

        if (!PlayerRotateLock && Mouse.current != null)
        {
            float mouseX = Mouse.current.delta.ReadValue().x;
            yaw += mouseX * mouseSensitivity;
        }
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequest  = true;
 
        }
        

    }
    void FixedUpdate()
    {        
        UpdateGround();

        if (isSliding)
        {
            Sliding();   // 자동 아래로 미끄러짐
        }
        else
        {
            UpdateMove();
        }
        HandleJump();
        UpdateRotate();

    }
    public void UpdateMove()
        {
            Vector3 pos = rb.position + inputMove * movespeed * Time.fixedDeltaTime;
            rb.MovePosition(pos);
            if (isMoving)
            {
                Vector3 dir = targetPos - rb.position;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.05f)
                {
                    isMoving = false;
                    return;
                }

                dir = dir.normalized;

                Vector3 nextPos = rb.position + dir * movespeed * Time.fixedDeltaTime;
                rb.MovePosition(nextPos);
            }
        }
    public void UpdateRotate()
    {
        if (!PlayerRotateLock)
        {
            Quaternion targetRot = Quaternion.Euler(spinX, yaw, 0f);
            rb.MoveRotation(targetRot);
        }
    }    
    private void UpdateGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, groundMask))
        {
            isGrounded = true;
            isSlidingSurface = hit.collider.CompareTag("Slope");
        }
        else
        {
            isGrounded = false;
            isSlidingSurface = false;
        }

        isSliding = isGrounded && isSlidingSurface;
    }
    public void SetRotateLock(bool isLock)
        {
            PlayerRotateLock = isLock;
            if (isLock)
            {
                lockRotation = transform.rotation;
                rb.angularVelocity = Vector3.zero;
            }
        }

    private void HandleJump()
        {
            if (!jumpRequest) return;
            jumpRequest = false;

            if (!isGrounded) return; // 이단 점프 방지

            rb.AddForce(Vector3.up * -rb.linearVelocity.y, ForceMode.VelocityChange);

            if (isSliding) 
            {
                SlideJump();
            }
            else 
            {
                NormalJump();
            }
            isGrounded = false;
        }

    private void NormalJump()
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    private void Sliding()
        {
            Vector3 autoForward = transform.forward * slideForwardSpeed;
            Vector3 sideMove = transform.right * inputMove.x * slideStrafeSpeed;
            Vector3 move = (autoForward + sideMove) * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

        }
    private void SlideJump()
    {
        rb.AddForce(Vector3.up * slideJumpUpForce, ForceMode.Impulse);

        SpinTween?.Kill();
        spinX = 0f;
        SpinTween = DOTween.Sequence()
            .Append(DOTween.To(() => spinX, x => spinX = x, 1440f, 1.5f).SetEase(Ease.OutCubic));
            //.Append(DOTween.To(() => spinX, x => spinX = x, 0f, 0.2f).SetEase(Ease.OutQuad));
    }
    
    private void GetInput()
    {        
        Vector2 move = Vector2.zero;

        if (Keyboard.current.aKey.isPressed) move.x -= 1f;
        if (Keyboard.current.dKey.isPressed) move.x += 1f;

        if (!isSliding)
        {
            if (Keyboard.current.sKey.isPressed) move.y -= 1f;
            if (Keyboard.current.wKey.isPressed) move.y += 1f;
        }

        move = move.normalized;
        inputMove = new Vector3(move.x, 0f, move.y);
    }
}
