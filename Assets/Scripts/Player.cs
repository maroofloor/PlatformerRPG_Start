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

    int Life;
    public int GetLife()
    {
        return Life;
    }
    int potionNum; // 플레이어가 소유한 포션의 개수
    public int GetPotionNum()
    {
        return potionNum;
    }
    public void AddPotionNum()
    {
        potionNum++;
    }

    bool isMove => vec.x != 0f; // vec.x가 0이 아니라면 플레이어가 이동키를 입력중...
    bool isLeft;
    bool isJump; // 체공중일 때 true
    bool isAttack1; // 공격 1번 모션중일 때 true
    bool isAttack2; // 공격 2번 모션중일 때 true
    bool isRoll; // 구르는 모션중일 때 true
    public bool GetIsRoll()
    {
        return isRoll;
    }

    public bool isAlive => myStat.HP > 0; // HP가 0보다 크면 살아있음
    bool isHit;
    public bool GetIsHit()
    {
        return isHit;
    }

    int attackNum = 0; // 공격콤보
    int jumpNum = 0; // 점프 횟수
    float speed = 6; // 플레이어 속도

    [SerializeField]
    int killCount;
    public int GetKillCount()
    {
        return killCount;
    }
    public void AddKillCount()
    {
        killCount++;
    }public void RemoveKillCount()
    {
        killCount -= 10;
    }

    //LayerMask playerLayer;
    //Vector2 pointVec;
    Vector2 rayVec;
    RaycastHit2D[] hits;
    LayerMask enemyLayer;

    void Start()
    {
        potionNum = 0;
        rollCool = 0f;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isAttack1 = false;
        isAttack2 = false;
        isRoll = false;
        myStat = new AllStruct.Stat(
            GameManager.Instance.GetEneforceInfo(AllEnum.EnforceType.MaxHP, 0),
            GameManager.Instance.GetEneforceInfo(AllEnum.EnforceType.Attack, 0),
            GameManager.Instance.GetEneforceInfo(AllEnum.EnforceType.Defense, 0)
            );
        isHit = false;
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        //playerLayer = 1 << LayerMask.NameToLayer("Player");
        rayVec = Vector2.zero;
        killCount = 0;
        Life = 3;
    }

    public void Revive()
    {
        if (Life > 0)
        {
            Life--;
            UIManager.Instance.MinusLife(Life);
        }
        else
        {
            UIManager.Instance.PrintWarningMsg("남은 라이프가 없어 부활할 수 없습니다.");
            return;
        }

        transform.position = Vector3.zero;
        anim.SetTrigger("IsRevive");
        myStat.HP = myStat.MaxHP;
        GameManager.Instance.ExitDeadScreen();
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
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), (isLeft ? Vector2.left : Vector2.right) * 2f, new Color(1, 0, 0));

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

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Hit(10 ,transform.position);
        //    Debug.Log("플레이어 체력 : " + myStat.HP + " / " + myStat.MaxHP);
        //}
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
                rigid.velocity = Vector3.zero; // 없으면 캐릭터가 공격중 착지했을때에 미끄러짐
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
        //for (int i = 0; i < hits.Length; i++)
        //        hits[i].transform.GetComponent<Monster>().Hit(myStat.Att, transform.position);
        rayVec = transform.position;
        rayVec.y += 0.5f;
        hits = Physics2D.RaycastAll(rayVec, isLeft ? Vector2.left : Vector2.right, 2f, enemyLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null /*&& hits[i].transform.GetComponent<Monster>().isAlive*/)
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
        if (isJump) // 공중에 있으면 점프 못하도록
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

    public void Hit(float damage, Vector2 pos)
    {
        if (isAlive)
        {
            float damageVal = damage - myStat.Def;
            if (damageVal > 0f)
                myStat.HP = Mathf.Clamp(myStat.HP - damageVal, 0, myStat.MaxHP);

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
            {
                Physics2D.IgnoreLayerCollision(3, 6);
                anim.SetTrigger("IsDeath");
                GameManager.Instance.PrintDeadScreen();
            }
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
        Physics2D.IgnoreLayerCollision(3, 6);
        yield return new WaitForSeconds(1f);
        isHit = false;
        if (isAlive)
            Physics2D.IgnoreLayerCollision(3, 6, false);

        rigid.velocity = Vector2.zero;
    }

    public void UsePotion()
    {
        if (potionNum > 0 && isAlive && myStat.HP < myStat.MaxHP)
        {
            potionNum--;
            myStat.HP = Mathf.Clamp(myStat.HP + (int)(myStat.MaxHP * 0.3f), 0, myStat.MaxHP);
            UIManager.Instance.PotionNumUpdate();
        }
        else
        {
            if (potionNum <= 0)
                UIManager.Instance.PrintWarningMsg("소지한 포션이 없습니다.");
            else if ((myStat.HP < myStat.MaxHP) == false)
                UIManager.Instance.PrintWarningMsg("이미 최대체력입니다.");
        }
    }
}
