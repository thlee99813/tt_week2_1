using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set;}
    public PlayerMove playerMove;
    public PlayerAttack playerAttack;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; // 싱글턴



        playerMove = gameObject.GetComponent<PlayerMove>();
        playerAttack = gameObject.GetComponent<PlayerAttack>();
        
        //플레이어에 여러 스크립트 부착 
    }
}