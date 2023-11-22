using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Monster : MonoBehaviour
{    
    public float movepower = 1f;
    Animator anim;
    Vector3 movement;
    Rigidbody2D rigid;
    int moveFlag = 0; // 0 정지 1 왼쪽 2 오른쪽
    public AllStruct.Stat enemy_stat;
    Player player;
    

    [SerializeField]
    Slider HPBar;
    
    void Start()
    {
        enemy_stat = new AllStruct.Stat(500,50); // 몬스터 스탯 임시로 적용
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Changemovement());       
    }

    IEnumerator Changemovement()
    {
        while(true)
        {
            moveFlag = Random.Range(0, 3);
            if (moveFlag == 0)
            {
                anim.SetBool("Walk", false);
            }
            else
            {
                anim.SetBool("Walk", true);
            }
            yield return new WaitForSeconds(3f);

        }
    }

   
    void Update()
    {
        Move();        
        StartCoroutine(Hit());
    }
   

    void Move()
    {
        Vector3 movevelocity = Vector3.zero;
        if (moveFlag == 1) 
        { 
            movevelocity = Vector3.left;
            transform.localScale = new Vector3(-5, 5, 1);
            rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;      

        }
        else if (moveFlag == 2)
        {
            movevelocity = Vector3.right;
            transform.localScale = new Vector3(5, 5, 1);
            rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;      
        }
       
        
    }

    IEnumerator Hit()
    {
        if (Input.GetKeyDown(KeyCode.A)) // 임시로 A키 누를시 몬스터 체력 깎임 추후 플레이어 공격으로 바꿀 예정
        {
            enemy_stat.HP -= 100; // 플레이어의 공격력만큼 데미지 입음

            HPBar.value = enemy_stat.HP;
            Debug.Log("몬스터 체력 : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

            if (enemy_stat.HP <= 0)
            {
                //사망 애니메이션 만들예정
                yield return new WaitForSeconds(1.5f);
                Debug.Log("몬스터 사망");
                Destroy(gameObject); // HP = 0일시 없어짐
            }
        }
    }
}
