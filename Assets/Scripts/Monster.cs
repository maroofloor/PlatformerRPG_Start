using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Monster : MonoBehaviour
{
    public float movepower = 1f;
    Animator anim;
    Vector3 movement = Vector3.zero;    
    Rigidbody2D rigid;      
    bool isRight; // 움직이던 정지해있던 간에 바라보는 방향 == true면 오른쪽, false면 왼쪽 
    bool isMove; // true면 움직이고 false면 정지. 트루일때 움직일건데 일단 한쪽방향으로만 진행하는데 isRight가 트루면 *1 펄스면 *(-1)

    public AllStruct.Stat enemy_stat;
    Player player;

    [SerializeField]
    Slider HPBar;

    Vector2 e_attack = Vector2.zero;
    RaycastHit2D frontperception;
    RaycastHit2D backperception;


    void Start()
    {
        enemy_stat = new AllStruct.Stat(500, 50); // 몬스터 스탯 임시로 적용
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Changemovement());
        isRight = true;
    }

    IEnumerator Changemovement()
    {
        while (true)
        {
            isMove = true;
            anim.SetBool("Walk", isMove);
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            isMove = false;
            isRight = Random.Range(0, 2) == 0 ? true : false;           
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            anim.SetBool("Walk", isMove);
        }
    }


    void Update()
    {
       //Move();
        StartCoroutine(Hit());
    }
    void FixedUpdate()
    {        
        //낭떠러지 앞에서 방향 전환
        Vector2 frontVec = new Vector2(rigid.position.x + (isRight? 0.5f:- 0.5f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        if (rayHit.collider == null)
        {           
            isRight = isRight ? false : true;
        }

        //공격범위
        e_attack.x = rigid.position.x;
        e_attack.y = rigid.position.y + 0.5f;
        
        if (isRight == false) // 왼쪽일 때...
        {           
            Debug.DrawRay(e_attack, Vector3.left, new Color(1, 0, 0));
            frontperception = Physics2D.Raycast(e_attack, Vector3.left, 1, LayerMask.GetMask("Player"));
            Debug.DrawRay(e_attack, Vector3.right, new Color(0, 0, 1));
            backperception = Physics2D.Raycast(e_attack, Vector3.right, 1, LayerMask.GetMask("Player"));
        }
        else 
        {           
            Debug.DrawRay(e_attack, Vector3.right, new Color(1, 0, 0));
            frontperception = Physics2D.Raycast(e_attack, Vector3.right, 1, LayerMask.GetMask("Player"));
            Debug.DrawRay(e_attack, Vector3.left, new Color(0, 0, 1));
            backperception = Physics2D.Raycast(e_attack, Vector3.left, 1, LayerMask.GetMask("Player"));
        }
        //뒤에 플레이어 감지
        if (backperception.collider != null)
        {
            //Debug.Log("뒤에 플레이어 감지");
            isRight = isRight ? false : true;
        }

        if (frontperception.collider != null)
        {
            isMove = false;
            anim.SetBool("Attack", true);
        }
        else
        {
            isMove = true;
            anim.SetBool("Attack", false);
        }

        if (enemy_stat.HP <= 0)
        {
            isMove = false;
            StartCoroutine(Hit());
        }
    }
    void LateUpdate()
    {
        if (isMove)
        {

            if (isRight == true)
            {
                movement = Vector3.right;
                transform.localScale = new Vector3(5, 5, 1);
                rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;
                HPBar.transform.localScale = new Vector3(0.02f, 0.02f, 1);
            }
            else if (isRight == false)
            {
                movement = Vector3.left;
                transform.localScale = new Vector3(-5, 5, 1);
                rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;
                HPBar.transform.localScale = new Vector3(-0.02f, 0.02f, 1);
            }
        }
    }

    //void Move()
    //{
    //    Vector3 movevelocity = Vector3.zero;
    //    if (moveFlag == -1)
    //    {
    //        movevelocity = Vector3.left;
    //        transform.localScale = new Vector3(-5, 5, 1);
    //        rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;
    //        HPBar.transform.localScale = new Vector3(-0.02f, 0.02f, 1);
    //        attFlag = -1;
    //    }
    //    else if (moveFlag == 1)
    //    {
    //        movevelocity = Vector3.right;
    //        transform.localScale = new Vector3(5, 5, 1);
    //        rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;
    //        HPBar.transform.localScale = new Vector3(0.02f, 0.02f, 1);
    //        attFlag = 1;
    //    }

    //    if (enemy_stat.HP <= 0)
    //    {
    //        moveFlag = 0;
    //    }
    //}

    IEnumerator Hit()
    {
        if (Input.GetKeyDown(KeyCode.A)) // 임시로 A키 누를시 몬스터 체력 깎임 추후 플레이어 공격으로 바꿀 예정
        {
            enemy_stat.HP -= 100; // 플레이어의 공격력만큼 데미지 입음
            anim.SetTrigger("Hit");
            HPBar.value = enemy_stat.HP;
            Debug.Log("몬스터 체력 : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

            if (enemy_stat.HP <= 0)
            {               
                anim.SetTrigger("Dead");
                yield return new WaitForSeconds(1f);
                Debug.Log("몬스터 사망");
                Destroy(gameObject); // HP = 0일시 없어짐
            }
        }
    }
    //IEnumerator Battle()
    //{
        
    //    yield return new WaitForSeconds(1f);
    //}
    
}
