using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour, AllInterface.IHit
{
    Animator anim;
    public float movepower = 1f;
    public Transform Target;
    Rigidbody2D rigid;
    public AllStruct.Stat Boss_stat;
    public CapsuleCollider2D col;
    public float attspeed;

    [SerializeField]
    Slider HPBar;
    float dir;
    bool isAlive => Boss_stat.HP > 0;
    bool ismove;
    bool isattack;
    bool isdetected;
    bool isLeft;

    float attackCool = 0;
    Coroutine attackCor = null;

    Vector2 hitVec = Vector2.zero;

    RaycastHit2D hit;

    void Start()
    {
        Boss_stat = new AllStruct.Stat(10000, 1/*300*/, 100);
        HPBar.maxValue = Boss_stat.MaxHP;
        HPBar.value = Boss_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ismove = false;
        isLeft = true;
        isdetected = false;
    }

    void Update()
    {
        if (isAlive)
        {
            //ismove = false;
            anim.SetBool("Boss_Walk", ismove);
            float dis = Vector3.Distance(transform.position, Target.position);
            if (dis <= 10 /*&& dis > 5*/)
                isdetected = true;
            //else if (dis <= 5)
            //{
            //    Attack();
            //}

            if (isdetected && isattack == false)
            {
                hitVec.x = transform.position.x;
                hitVec.y = transform.position.y + 0.5f;
                Debug.DrawRay(hitVec, (isLeft ? Vector2.left : Vector2.right) * 5.5f, Color.magenta);
                hit = Physics2D.Raycast(hitVec, isLeft ? Vector2.left : Vector2.right, 5.5f, 1 << LayerMask.NameToLayer("Player"));

                if (hit.collider != null && attackCool <= 0)
                {
                    Attack();
                }
                else
                    Move();

                Hit(1000, Vector2.zero);
            }
            else
            {
                ismove = false;
                anim.SetBool("Boss_Walk", ismove);
            }

        }
    }

    void Move()
    {
        dir = Target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        isLeft = dir < 0;
        ismove = true;
        anim.SetBool("Boss_Walk", ismove);
        if (ismove)
        {
            transform.Translate(new Vector2(dir, 0) * movepower * Time.deltaTime);
            if (dir == -1)
            {
                transform.localScale = new Vector3(5, 5, 1);
                HPBar.transform.localScale = new Vector3(0.2f, 0.2f, 1);
            }
            else if (dir == 1)
            {
                transform.localScale = new Vector3(-5, 5, 1);
                HPBar.transform.localScale = new Vector3(-0.2f, 0.2f, 1);
            }
        }

    }

    void Attack()
    {
        StartCoroutine(WaitAttack());
        anim.SetTrigger("Boss_Attack");
        attackCool = 5f;
        if (attackCor == null)
            attackCor = StartCoroutine(AttackCoolDown());
        if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
        {
            movepower = 3f;
        }
    }

    public void AttackEvent()
    {
        if (hit.collider != null)
            hit.collider.transform.GetComponent<Player>().Hit(Boss_stat.Att, transform.position);
    }

    IEnumerator WaitAttack()
    {
        isattack = true;
        yield return new WaitForSeconds(1.3f);
        isattack = false;

        hitVec.x = transform.position.x;
        hitVec.y = transform.position.y + 0.5f;
        hit = Physics2D.Raycast(hitVec, isLeft ? Vector2.left : Vector2.right, 5.5f, 1 << LayerMask.NameToLayer("Player"));
        if (hit.collider != null)
        {
            hit.collider.transform.GetComponent<Player>().Hit(Boss_stat.Att, transform.position);
        }
    }

    IEnumerator AttackCoolDown()
    {
        while (attackCool > 0)
        {
            attackCool -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (attackCool <= 0)
        {
            StopCoroutine(attackCor);
            attackCor = null;
        }
    }
    //IEnumerator Hit()
    //{
    //    if (Input.GetKeyDown(KeyCode.A)) // 임시로 A키 누를시 몬스터 체력 깎임 추후 플레이어 공격으로 바꿀 예정
    //    {
    //        Boss_stat.HP -= 1000; // 플레이어의 공격력만큼 데미지 입음

    //        anim.SetTrigger("Boss_HIt");
    //        HPBar.value = Boss_stat.HP;
    //        Debug.Log("보스몬스터 체력 : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);

    //        if (Boss_stat.HP <= 0)
    //        {
    //            ismove = false;
    //            anim.SetTrigger("Boss_Death");
    //            yield return new WaitForSeconds(1.5f);                
    //            Destroy(gameObject); // HP = 0일시 없어짐
    //        }
    //    }
    //}

    public void Hit(float damage, Vector2 pos)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Boss_stat.HP -= damage; // 플레이어의 공격력만큼 데미지 입음
            anim.SetTrigger("Boss_HIt");
            HPBar.value = Boss_stat.HP;
            Debug.Log("보스몬스터 체력 : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);
        }
        if (Boss_stat.HP <= 0)
        {
            Debug.Log("죽음");
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Boss_Death");
        ismove = false;
        rigid.gravityScale = 0;
    }
}


