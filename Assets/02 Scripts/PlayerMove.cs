using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float movespeed = 6f;

    

    [Header("PlayerObject")]
    public Rigidbody rb;
    

    private Vector3 inputMove;
    private Vector3 targetPos;
    private bool isMoving;

    public bool PlayerMoveLock = false;
    public bool PlayerRotateLock = false;

    private Quaternion lockRotation;

    private bool isJump;

    private bool isGrounded;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
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

        if(!PlayerRotateLock)
        {

            //플레이어 마우스 위치에 따른 회전 기능 구현
        }
        

    }
    void FixedUpdate()
    {
        UpdateMove();

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

       public void SetRotateLock(bool isLock)
    {
        PlayerRotateLock = isLock;

        if (isLock)
        {
            lockRotation = transform.rotation;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    private void GetInput()
    {        
        Vector2 move = Vector2.zero;
        if(Keyboard.current.aKey.isPressed) move.x -= 1f;
        if(Keyboard.current.dKey.isPressed) move.x += 1f;
        if(Keyboard.current.sKey.isPressed) move.y -= 1f;
        if(Keyboard.current.wKey.isPressed) move.y += 1f;

        move = move.normalized;
        inputMove = new Vector3(move.x, 0f, move.y);
    }
}
