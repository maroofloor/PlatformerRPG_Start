using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, AllInterface.IHit
{
    Animator anim;
    Rigidbody2D rigid;

    public float rollCool;
    Coroutine rollCor = null;

    public Vector3 vec = Vector3.zero;
    public Vector3 scaleVec = Vector3.one;
    Vector3 jumpVec = Vector3.zero;
    Vector3 rollVec = Vector3.zero;

    public AllStruct.Stat myStat; // 플레이어의 스탯(구조체)... scripts폴더에 AllStruct스크립트...

    public int potionNum; // 플레이어가 소유한 포션의 개수

    bool isMove => vec.x != 0f; // vec.x가 0이 아니라면 플레이어가 이동키를 입력중...
    public bool isLeft;
    bool isJump; // 체공중일 때 true
    bool isAttack1; // 공격 1번 모션중일 때 true
    bool isAttack2; // 공격 2번 모션중일 때 true
    bool isRoll; // 구르는 모션중일 때 true
    bool isAlive => myStat.HP > 0; // HP가 0보다 크면 살아있음
    bool isHit;

    int attackNum = 0; // 공격콤보
    int jumpNum = 0; // 점프 횟수
    float speed = 6; // 플레이어 속도

    [SerializeField]
    int enforceVal;
    float[] enforceAtt = new float[11] { 10, 20, 40, 70, 110, 160, 220, 290, 370, 460, 600 };
    float[] enforceDef = new float[11] { 0, 2, 6, 12, 20, 30, 42, 56, 72, 90, 550 };
    float[] enforceHP = new float[11] { 100, 200, 330, 490, 680, 900, 1150, 1440, 1770, 2140, 3000 };

    Vector2 rayVec;
    RaycastHit2D[] hits;
    public LayerMask enemyLayer;
    LayerMask playerLayer;


    void Start()
    {
        potionNum = 0;
        rollCool = 0f;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isAttack1 = false;
        isAttack2 = false;
        isRoll = false;
        myStat = new AllStruct.Stat(enforceHP[0], enforceAtt[0], enforceDef[0]);
        enforceVal = 0;
        isHit = false;
        // enemyLayer = 1 << LayerMask.NameToLayer("Enemy");       
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        rayVec = Vector2.zero;
    }

    public void Jump()
    {
        if (jumpNum < 2 && !isRoll && isAlive && !isHit) // C키로 점프
        {
            jumpNum++;
            if (vec.x != 0)
            {
                scaleVec.x = vec.x;

                if (vec.x < 0)
                    isLeft = true;
                else
                    isLeft = false;
            }

            transform.localScale = scaleVec;
            jumpVec = new Vector3(vec.x, 1f, 0);
            rigid.velocity = jumpVec * speed;
        }
    }
    public void Attack()
    {
        if (attackNum < 2 && !isRoll && isAlive && !isHit)
        {
            if (vec.x != 0)
            {
                scaleVec.x = vec.x;

                if (vec.x < 0)
                    isLeft = true;
                else
                    isLeft = false;
            }

            transform.localScale = scaleVec;

            rigid.velocity = Vector3.zero; // 공격시 체공중이라도 그자리에 멈춰서 공격
            attackNum++;

            StartCoroutine(PlayAttack());
        }
    }
    public void Roll()
    {
        if (!isAttack1 && !isAttack2 && !isRoll && !isJump && isAlive && rollCool <= 0f && !isHit)
            StartCoroutine(PlayRoll());
    }

    void Update()
    {
        rayVec = transform.position;
        rayVec.y += 0.5f;
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), (isLeft ? Vector2.left : Vector2.right) * 2f, new Color(1, 0, 0));
        hits = Physics2D.RaycastAll(rayVec, isLeft ? Vector2.left : Vector2.right, 2f, enemyLayer);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    myStat.HP -= 10f;
        //    Debug.Log("플레이어 체력 : " + myStat.HP + " / " + myStat.MaxHP);
        //    if (!isAlive)
        //        anim.SetTrigger("IsDeath"); // HP 가 0보다 높으면 true, 낮으면 false...
        //}
    }
    void LateUpdate()
    {
        if (vec.x != 0 && !isAttack1 && !isAttack2 && isAlive && !isJump && !isRoll && !isHit)
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
        if (isAlive && !isAttack1 && !isAttack2 && !isRoll && !isJump && !isHit) // 공격중, 회피중에 이동 할 수 없음
            transform.Translate(vec.normalized * Time.fixedDeltaTime * speed); // 캐릭터의 이동
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // 땅에 착지할 때...
        {
            isJump = false;
            anim.SetBool("IsJump", isJump);
            if (collision.contacts[0].point.y <= (transform.position.y + 0.3f))
            {
                jumpNum = 0;
                jumpVec = Vector3.zero;
                rigid.velocity = Vector3.zero; // 없으면 캐릭터가 공격중 착지했을때에 이동이 가능한데 모션이 이상함...
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
            Hit(collision.transform.GetComponent<Monster>().enemy_stat.Att, collision.transform.position);
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
        if (collision.gameObject.CompareTag("Ground") && isAlive && !isHit) // 땅에서 접촉이 떨어졌을 때...
        {
            isJump = true;
            anim.SetBool("IsJump", isJump);
            if (jumpVec.y == 0f)
                rigid.velocity = Vector3.right * (isLeft ? -1 : 1) * speed;
        }
    }

    IEnumerator PlayAttack()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].transform.GetComponent<Monster>().isAlive)
                hits[i].transform.GetComponent<Monster>().Hit(myStat.Att, transform.position);
        }

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
        Physics2D.IgnoreLayerCollision(3, 6);
        rollVec.x = isLeft ? -1f : 1f;
        rigid.velocity = rollVec * 8;
        yield return new WaitForSeconds(1f);
        if (!isJump)
        {
            rollVec.x = 0f;
            rigid.velocity = rollVec;
        }
        else
            jumpNum += 2;
        isRoll = false;
        Physics2D.IgnoreLayerCollision(3, 6, false);
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

    public void EnforceStat(int enforceNum)
    {
        AllEnum.EnforceType enforceType = (AllEnum.EnforceType)enforceNum;
        if (enforceVal > 0)
        {
            switch (enforceType)
            {
                case AllEnum.EnforceType.Attack:
                    for (int i = 0; i < enforceAtt.Length; i++)
                    {
                        if (myStat.Att < enforceAtt[i] && i != 0)
                        {
                            myStat.Att = enforceAtt[i];
                            enforceVal--;
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 공격력을 강화 합니다.\n{enforceAtt[i - 1]} -> {enforceAtt[i]}");
                            return;
                        }
                    }
                    break;
                case AllEnum.EnforceType.Defense:
                    for (int i = 0; i < enforceDef.Length; i++)
                    {
                        if (myStat.Def < enforceDef[i] && i != 0)
                        {
                            myStat.Def = enforceDef[i];
                            enforceVal--;
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 방어력을 강화 합니다.\n{enforceDef[i - 1]} -> {enforceDef[i]}");
                            return;
                        }
                    }
                    break;
                case AllEnum.EnforceType.MaxHP:
                    for (int i = 0; i < enforceHP.Length; i++)
                    {
                        if (myStat.MaxHP < enforceHP[i] && i != 0)
                        {
                            myStat.MaxHP = enforceHP[i];
                            myStat.HP += enforceHP[i] - enforceHP[i - 1]; // 증가한 값 만큼 HP에 더하기
                            enforceVal--;
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 최대체력을 강화 합니다.\n{enforceHP[i - 1]} -> {enforceHP[i]}");
                            return;
                        }
                    }
                    break;
            }
        }
        else
        {
            UIManager.Instance.PrintWarningMsg("강화개수가 부족하여 강화할 수 없습니다.");
        }
    }

    public void Hit(float damage, Vector2 pos)
    {
        if (isAlive)
        {
            myStat.HP = Mathf.Clamp(myStat.HP - damage, 0, myStat.MaxHP);
            #region 넉백될 벡터 구하기
            Vector2 KnockVec = Vector2.zero;
            KnockVec = (Vector2)transform.position - pos;
            bool dirIsLeft = KnockVec.x < 0f;
            KnockVec = dirIsLeft ? Vector2.left : Vector2.right;
            KnockVec.y = 0.5f;
            KnockVec *= 5f;
            #endregion
            rigid.velocity = KnockVec;

            if (!isAlive)
                anim.SetTrigger("IsDeath");
            else
            {
                anim.SetTrigger("IsHit");
                StartCoroutine(HitWait());
            }
        }
    }

    IEnumerator HitWait()
    {
        isHit = true;
        yield return new WaitForSeconds(1f);
        isHit = false;
        rigid.velocity = Vector2.zero;
    }
}
