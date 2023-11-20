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

    bool isMove => vec.x != 0f; // vec.x�� 0�� �ƴ϶�� �÷��̾ �̵�Ű�� �Է���...
    public bool isLeft;
    bool isJump; // ü������ �� true
    bool isAttack1; // ���� 1�� ������� �� true
    bool isAttack2; // ���� 2�� ������� �� true
    bool isRoll; // ������ ������� �� true
    bool isAlive => myStat.HP > 0; // HP�� 0���� ũ�� �������

    int attackNum = 0; // �����޺�
    public int jumpNum = 0; // ���� Ƚ��
    float speed = 6; // �÷��̾� �ӵ�

    void Start()
    {
        rollCool = 0f;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isAttack1 = false;
        isAttack2 = false;
        isRoll = false;
        myStat = new AllStruct.Stat(100, 10); // �ϴ� ü��100, ���ݷ� 10���� ����
    }

    public void Jump()
    {
        if (jumpNum < 2 && !isRoll && isAlive) // CŰ�� ����
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

            rigid.velocity = Vector3.zero; // ���ݽ� ü�����̶� ���ڸ��� ���缭 ����
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
            Debug.Log("�÷��̾� ü�� : " + myStat.HP + " / " + myStat.MaxHP);
            if (!isAlive)
                anim.SetTrigger("IsDeath"); // HP �� 0���� ������ true, ������ false...
        }
    }
    void LateUpdate()
    {
        if (vec.x != 0 && !isAttack1 && !isAttack2 && isAlive && !isJump && !isRoll)
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
        if (isAlive && !isAttack1 && !isAttack2 && !isRoll && !isJump)// ������, ȸ���߿� �̵� �� �� ����
            transform.Translate(vec.normalized * Time.fixedDeltaTime * speed); // ĳ������ �̵�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // ���� ������ ��...
        {
            rigid.velocity = Vector3.zero; // ������ ĳ���Ͱ� ������ ������������ �̵��� �����ѵ� ����� �̻���...
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
        if (collision.gameObject.CompareTag("Ground") && isAlive) // ���� �������� ��...
        {
            isJump = false;
            anim.SetBool("IsJump", isJump);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive) // ������ ������ �������� ��...
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
}
