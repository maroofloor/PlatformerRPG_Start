using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Monster : MonoBehaviour, AllInterface.IHit
{
    public float movepower = 1f;
    Animator anim;
    Vector3 movement = Vector3.zero;
    Rigidbody2D rigid;
    bool isRight; // 움직이던 정지해있던 간에 바라보는 방향 == true면 오른쪽, false면 왼쪽 
    bool isMove; // true면 움직이고 false면 정지. 트루일때 움직일건데 일단 한쪽방향으로만 진행하는데 isRight가 트루면 *1 펄스면 *(-1)
    public bool isAlive => enemy_stat.HP > 0;
    public AllStruct.Stat enemy_stat;
    public CapsuleCollider2D col;

    [SerializeField]
    Slider HPBar;
    [SerializeField]
    Text text;

    Vector2 e_attack = Vector2.zero;
    RaycastHit2D frontperception;
    RaycastHit2D backperception;


    void Start()
    {
        enemy_stat = new AllStruct.Stat(50, 5); // 몬스터 스탯 임시로 적용
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Changemovement());
        isRight = true;
        col = GetComponent<CapsuleCollider2D>();
    }

    IEnumerator Changemovement()
    {
        while (isAlive)
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
        //StartCoroutine(Hit());
        text.text = $"{enemy_stat.HP} / {enemy_stat.MaxHP}";
        HPBar.value = enemy_stat.HP;
    }
    void FixedUpdate()
    {
        if (isAlive)
        {
            if (isMove)
            {
                if (isRight == true)
                {
                    movement = Vector3.right;
                    transform.localScale = new Vector3(5, 5, 1);
                    //rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;
                    transform.Translate(movement * Time.fixedDeltaTime);
                    HPBar.transform.localScale = new Vector3(0.02f, 0.02f, 1);
                    text.transform.localScale = new Vector3(0.01f, 0.01f, 1);
                }
                else if (isRight == false)
                {
                    movement = Vector3.left;
                    transform.localScale = new Vector3(-5, 5, 1);
                    //rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;
                    transform.Translate(movement * Time.fixedDeltaTime);
                    HPBar.transform.localScale = new Vector3(-0.02f, 0.02f, 1);
                    text.transform.localScale = new Vector3(-0.01f, 0.01f, 1);
                }
            }

            //낭떠러지 앞에서 방향 전환
            Vector2 frontVec = new Vector2(rigid.position.x + (isRight ? 0.5f : -0.5f), rigid.position.y);
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
        }
    }

    //void LateUpdate()
    //{

    //}

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

    //IEnumerator Hit()
    //{
    //    if (Input.GetKeyDown(KeyCode.A)) // 임시로 A키 누를시 몬스터 체력 깎임 추후 플레이어 공격으로 바꿀 예정
    //    {
    //        enemy_stat.HP -= 100; // 플레이어의 공격력만큼 데미지 입음

    //        anim.SetTrigger("Hit");
    //        HPBar.value = enemy_stat.HP;
    //        Debug.Log("몬스터 체력 : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

    //        if (enemy_stat.HP <= 0)
    //        {                
    //            isMove = false;
    //            anim.SetTrigger("Dead");
    //            yield return new WaitForSeconds(1.5f);
    //            Debug.Log("몬스터 사망");
    //            Destroy(gameObject); // HP = 0일시 없어짐
    //        }
    //    }
    //}
     
    //private void OnCollisionEnter2D(Collision2D collision) 
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        enemy_stat.HP -= 100; // 플레이어의 공격력만큼 데미지 입음
    //        anim.SetTrigger("Hit");
    //        HPBar.value = enemy_stat.HP;
    //        //direction = (transform.position - collision.transform.position).normalized;
    //        //direction.x += 1;
    //        //direction.y += 2;
    //        //direction *= Knockbackpower;
    //        //collision.transform.GetComponent<GameObject>();
    //        //Hit(direction);
    //        Debug.Log("몬스터 체력 : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

    //        if (enemy_stat.HP <= 0)
    //        {
    //            anim.SetTrigger("Dead");
    //            Invoke("Die", 1.5f);
    //        }
    //    }
    //}

    //void Hit(Vector3 dir)
    //{
    //   // rigid.AddForce(dir, ForceMode2D.Force);
    //    enemy_stat.HP -= 100; // 플레이어의 공격력만큼 데미지 입음
    //    anim.SetTrigger("Hit");
    //    HPBar.value = enemy_stat.HP;
    //}

    public void SetInfo(Vector3 pos)
    {
        transform.position = pos;
        enemy_stat.HP = enemy_stat.MaxHP;
        if (col == null)
        {
            col = GetComponent<CapsuleCollider2D>();
        }
        col.enabled = true;
        if (rigid == null)
            rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = 1;
    }

    void Die()
    {
        anim.SetTrigger("Dead");
        isMove = false;
        rigid.gravityScale = 0;
        col.enabled = false;
        StartCoroutine(WaitEnque());
        Debug.Log("몬스터 사망");

        //Destroy(gameObject); // HP = 0일시 없어짐

    }
    public void Hit(float damage, Vector2 pos)
    {
        enemy_stat.HP = Mathf.Clamp(enemy_stat.HP - damage, 0f, enemy_stat.MaxHP);
        //#region 넉백될 벡터 구하기
        //Vector2 KnockVec = Vector2.zero;
        //KnockVec = (Vector2)transform.position - pos;
        //bool dirIsLeft = KnockVec.x < 0f;
        //KnockVec = dirIsLeft ? Vector2.left : Vector2.right;
        //KnockVec.y = 0.5f;
        //#endregion

        if (isAlive == false)
            Die();
        else
        {
            //rigid.velocity = KnockVec;
            anim.SetTrigger("Hit");
        }
            
    }

    IEnumerator WaitEnque()
    {
        yield return new WaitForSeconds(10f);
        MonsterSpawner.Instance.MonsterEnqueue(this.gameObject);
    }
}
