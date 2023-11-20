using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;

    public float rollCool;
    Coroutine rollCor = null;

    public Vector3 vec = Vector3.zero;
    public Vector3 scaleVec = Vector3.one;
    Vector3 jumpVec = Vector3.zero;
    Vector3 rollVec = Vector3.zero;

    public AllStruct.Stat myStat;

    bool isMove => vec.x != 0f; // vec.x가 0이 아니라면 플레이어가 이동키를 입력중...
    public bool isLeft;
    bool isJump; // 체공중일 때 true
    bool isAttack1; // 공격 1번 모션중일 때 true
    bool isAttack2; // 공격 2번 모션중일 때 true
    bool isRoll; // 구르는 모션중일 때 true
    bool isAlive => myStat.HP > 0; // HP가 0보다 크면 살아있음

    int attackNum = 0; // 공격콤보
    public int jumpNum = 0; // 점프 횟수
    float speed = 6; // 플레이어 속도

    void Start()
    {
        rollCool = 0f;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isAttack1 = false;
        isAttack2 = false;
        isRoll = false;
        myStat = new AllStruct.Stat(100, 10); // 일단 체력100, 공격력 10으로 시작
    }

    public void Jump()
    {
        if (jumpNum < 2 && !isRoll && isAlive) // C키로 점프
        {
            jumpNum++;
            if (vec.x != 0)
                scaleVec.x = vec.x;

            if (vec.x < 0)
                isLeft = true;
            else
                isLeft = false;

            transform.localScale = scaleVec;
            jumpVec = new Vector3(vec.x, 1f, 0);
            rigid.velocity = jumpVec * speed;
        }
    }
    public void Attack()
    {
        if (attackNum < 2 && !isRoll && isAlive)
        {
            if (vec.x != 0)
                scaleVec.x = vec.x;

            if (vec.x < 0)
                isLeft = true;
            else
                isLeft = false;

            transform.localScale = scaleVec;

            rigid.velocity = Vector3.zero; // 공격시 체공중이라도 그자리에 멈춰서 공격
            attackNum++;

            StartCoroutine(PlayAttack());
        }
    }
    public void Roll()
    {
        if (!isAttack1 && !isAttack2 && !isRoll && !isJump && isAlive && rollCool <= 0f)
            StartCoroutine(PlayRoll());
    }

    void Update()
    {
        vec.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myStat.HP -= 10f;
            Debug.Log("플레이어 체력 : " + myStat.HP + " / " + myStat.MaxHP);
            if (!isAlive)
                anim.SetTrigger("IsDeath"); // HP 가 0보다 높으면 true, 낮으면 false...
        }
    }
    void LateUpdate()
    {
        if (vec.x != 0 && !isAttack1 && !isAttack2 && isAlive && !isJump && !isRoll)
        {
            scaleVec.x = vec.x; // scaleVec이 기본으로 Vector3.one(1, 1, 1)이기 때문에 x만 -1 혹은 1로 바꾸면...
            transform.localScale = scaleVec; // 플레이어가 바라보는 방향이 바뀜.
            if (vec.x < 0)
                isLeft = true;
            else
                isLeft = false;
        }
        anim.SetBool("IsMove", isMove); // vec.x가 0이 아닐 때 isMove가 true, 0이면 false...
        
    }

    private void FixedUpdate()
    {
        if (isAlive && !isAttack1 && !isAttack2 && !isRoll && !isJump)// 공격중, 회피중에 이동 할 수 없음
            transform.Translate(vec.normalized * Time.fixedDeltaTime * speed); // 캐릭터의 이동
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // 땅에 착지할 때...
        {
            rigid.velocity = Vector3.zero; // 없으면 캐릭터가 공격중 착지했을때에 이동이 가능한데 모션이 이상함...
            isJump = false;
            anim.SetBool("IsJump", isJump);
            if (collision.contacts[0].point.y <= (transform.position.y + 0.1f))
            {
                jumpNum = 0;
                jumpVec = Vector3.zero;
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // 땅에 접촉중일 때...
        {
            isJump = false;
            anim.SetBool("IsJump", isJump);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // 땅에서 접촉이 떨어졌을 때...
        {
            isJump = true;
            anim.SetBool("IsJump", isJump);
            if (jumpVec.y == 0f)
            rigid.velocity = Vector3.right * (isLeft ? -1 : 1) * speed;
        }
    }
    IEnumerator PlayAttack()
    {
        if (attackNum == 1)
        {
            anim.SetTrigger("IsAttack1");
            isAttack1 = true;
            yield return new WaitForSeconds(0.4f);
            isAttack1 = false;
        }
        else if (attackNum == 2)
        {
            anim.SetTrigger("IsAttack2");
            isAttack2 = true;
            yield return new WaitForSeconds(0.6f);
            isAttack2 = false;

            if (attackNum >= 2)
                attackNum = 0;
        }
    }

    IEnumerator PlayRoll()
    {
        anim.SetTrigger("IsRoll");
        isRoll = true;
        rollVec.x = isLeft ? -1f : 1f;
        rigid.velocity = rollVec * 8;
        yield return new WaitForSeconds(1f);
        if (!isJump)
        {
            rollVec.x = 0f;
            rigid.velocity = rollVec;
        }
        else
        {
            jumpNum += 2;
        }
        isRoll = false;
        rollCool = 3f; // 쿨타임 3초
        if (rollCor == null)
            rollCor = StartCoroutine(CoolRoll());
    }

    IEnumerator CoolRoll()
    {
        while (rollCool > 0)
        {
            yield return new WaitForSeconds(0.1f);
            rollCool -= 0.1f;
        }
        StopCoroutine(rollCor);
        rollCor = null;
    }
}
