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

    public AllStruct.Stat myStat; // �÷��̾��� ����(����ü)... scripts������ AllStruct��ũ��Ʈ...

    public int potionNum; // �÷��̾ ������ ������ ����

    bool isMove => vec.x != 0f; // vec.x�� 0�� �ƴ϶�� �÷��̾ �̵�Ű�� �Է���...
    public bool isLeft;
    bool isJump; // ü������ �� true
    bool isAttack1; // ���� 1�� ������� �� true
    bool isAttack2; // ���� 2�� ������� �� true
    bool isRoll; // ������ ������� �� true
    bool isAlive => myStat.HP > 0; // HP�� 0���� ũ�� �������
    bool isHit;

    int attackNum = 0; // �����޺�
    int jumpNum = 0; // ���� Ƚ��
    float speed = 6; // �÷��̾� �ӵ�

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
        if (jumpNum < 2 && !isRoll && isAlive && !isHit) // CŰ�� ����
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

            rigid.velocity = Vector3.zero; // ���ݽ� ü�����̶� ���ڸ��� ���缭 ����
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
        //    Debug.Log("�÷��̾� ü�� : " + myStat.HP + " / " + myStat.MaxHP);
        //    if (!isAlive)
        //        anim.SetTrigger("IsDeath"); // HP �� 0���� ������ true, ������ false...
        //}
    }
    void LateUpdate()
    {
        if (vec.x != 0 && !isAttack1 && !isAttack2 && isAlive && !isJump && !isRoll && !isHit)
        {
            scaleVec.x = vec.x; // scaleVec�� �⺻���� Vector3.one(1, 1, 1)�̱� ������ x�� -1 Ȥ�� 1�� �ٲٸ�...
            transform.localScale = scaleVec; // �÷��̾ �ٶ󺸴� ������ �ٲ�.
            if (vec.x < 0)
                isLeft = true;
            else
                isLeft = false;
        }
        anim.SetBool("IsMove", isMove); // vec.x�� 0�� �ƴ� �� isMove�� true, 0�̸� false...
    }

    private void FixedUpdate()
    {
        if (isAlive && !isAttack1 && !isAttack2 && !isRoll && !isJump && !isHit) // ������, ȸ���߿� �̵� �� �� ����
            transform.Translate(vec.normalized * Time.fixedDeltaTime * speed); // ĳ������ �̵�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // ���� ������ ��...
        {
            isJump = false;
            anim.SetBool("IsJump", isJump);
            if (collision.contacts[0].point.y <= (transform.position.y + 0.3f))
            {
                jumpNum = 0;
                jumpVec = Vector3.zero;
                rigid.velocity = Vector3.zero; // ������ ĳ���Ͱ� ������ ������������ �̵��� �����ѵ� ����� �̻���...
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
            Hit(collision.transform.GetComponent<Monster>().enemy_stat.Att, collision.transform.position);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // ���� �������� ��...
        {
            isJump = false;
            anim.SetBool("IsJump", isJump);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive && !isHit) // ������ ������ �������� ��...
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
        rollCool = 3f; // ��Ÿ�� 3��
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
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� ���ݷ��� ��ȭ �մϴ�.\n{enforceAtt[i - 1]} -> {enforceAtt[i]}");
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
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� ������ ��ȭ �մϴ�.\n{enforceDef[i - 1]} -> {enforceDef[i]}");
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
                            myStat.HP += enforceHP[i] - enforceHP[i - 1]; // ������ �� ��ŭ HP�� ���ϱ�
                            enforceVal--;
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� �ִ�ü���� ��ȭ �մϴ�.\n{enforceHP[i - 1]} -> {enforceHP[i]}");
                            return;
                        }
                    }
                    break;
            }
        }
        else
        {
            UIManager.Instance.PrintWarningMsg("��ȭ������ �����Ͽ� ��ȭ�� �� �����ϴ�.");
        }
    }

    public void Hit(float damage, Vector2 pos)
    {
        if (isAlive)
        {
            myStat.HP = Mathf.Clamp(myStat.HP - damage, 0, myStat.MaxHP);
            #region �˹�� ���� ���ϱ�
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
