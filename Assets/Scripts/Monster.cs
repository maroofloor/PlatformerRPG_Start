using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Monster : MonoBehaviour, AllInterface.IHit
{
    public int areaNum;
    public float movepower = 1f;
    Animator anim;
    Vector3 movement = Vector3.zero;
    Rigidbody2D rigid;
    bool isRight; // �����̴� �������ִ� ���� �ٶ󺸴� ���� == true�� ������, false�� ���� 
    bool isMove; // true�� �����̰� false�� ����. Ʈ���϶� �����ϰǵ� �ϴ� ���ʹ������θ� �����ϴµ� isRight�� Ʈ��� *1 �޽��� *(-1)
    public bool isAlive => enemy_stat.HP > 0;
    bool isAttack;
    bool isHit;
    float attackCool;
    Coroutine attCoolCor = null;

    public AllStruct.Stat enemy_stat;
    public CapsuleCollider2D col;

    [SerializeField]
    Slider HPBar;
    [SerializeField]
    Text text;

    Vector2 e_attack = Vector2.zero;
    RaycastHit2D frontperception;
    RaycastHit2D backperception;

    Coroutine cor = null;
    bool isDetect;
    Vector3 chaseDirVec = Vector3.zero;

    void Start()
    {
        enemy_stat = new AllStruct.Stat(100, 20); // ���� ���� �ӽ÷� ����
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isRight = true;
        col = GetComponent<CapsuleCollider2D>();
        isAttack = false;
        isHit = false;
        isDetect = false;
        attackCool = 0f;
        //if (cor == null)
        //    cor = StartCoroutine(Changemovement());
    }

    public void StartChangeMovement()
    {
        if (cor == null)
            cor = StartCoroutine(Changemovement());
    }

    IEnumerator Changemovement()
    {
        while (isAlive)
        {
            isMove = true;
            anim.SetBool("Walk", true);
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            isMove = false;
            anim.SetBool("Walk", false);
            isRight = Random.Range(0, 2) == 0 ? true : false;
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        }

        if (isAlive == false && cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    void Update()
    {
        if (isAlive && GameManager.Instance.player.isAlive)
        {
            text.text = $"{enemy_stat.HP} / {enemy_stat.MaxHP}";
            HPBar.value = enemy_stat.HP;

            //���ݹ���
            e_attack.x = rigid.position.x;
            e_attack.y = rigid.position.y + 0.5f;

            Debug.DrawRay(e_attack, isRight ? Vector3.right : Vector3.left, new Color(1, 0, 0));
            frontperception = Physics2D.Raycast(e_attack, isRight ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Player"));
            //Debug.DrawRay(e_attack, isRight ? Vector3.left : Vector3.right, new Color(0, 0, 1));
            //backperception = Physics2D.Raycast(e_attack, isRight ? Vector3.left : Vector3.right, 1, LayerMask.GetMask("Player"));
            //if (isRight == false) // ������ ��...
            //{
            //    Debug.DrawRay(e_attack, Vector3.left, new Color(1, 0, 0));
            //    frontperception = Physics2D.Raycast(e_attack, Vector3.left, 1, LayerMask.GetMask("Player"));
            //    Debug.DrawRay(e_attack, Vector3.right, new Color(0, 0, 1));
            //    backperception = Physics2D.Raycast(e_attack, Vector3.right, 1, LayerMask.GetMask("Player"));
            //}
            //else
            //{
            //    Debug.DrawRay(e_attack, Vector3.right, new Color(1, 0, 0));
            //    frontperception = Physics2D.Raycast(e_attack, Vector3.right, 1, LayerMask.GetMask("Player"));
            //    Debug.DrawRay(e_attack, Vector3.left, new Color(0, 0, 1));
            //    backperception = Physics2D.Raycast(e_attack, Vector3.left, 1, LayerMask.GetMask("Player"));
            //}

            ////�ڿ� �÷��̾� ����
            //if (backperception.collider != null && isHit ==false)
            //{
            //    //Debug.Log("�ڿ� �÷��̾� ����");
            //    isRight = isRight ? false : true;
            //    transform.localScale = new Vector3(isRight ? 5f : -5f, 5, 1);
            //}
        }
    }

    void LateUpdate()
    {
        if (isAlive && frontperception.collider != null && attackCool <= 0f)
        {
            anim.SetTrigger("Attack");
            StartCoroutine(WaitAttack());
            attackCool = 5f; // ������ ������Ÿ�� 5��
            if (attCoolCor == null)
                attCoolCor = StartCoroutine(AttackCoolDown());
            isDetect = true;
        }
        else
        {
            //isMove = true;
            //anim.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        if (isDetect && Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) > 6f)
        {
            isDetect = false;
        }

        // �������� ����
        Vector2 frontVec = new Vector2(rigid.position.x + (isRight ? 0.5f : -0.5f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (isAlive && isDetect == false)
        {
            if (isMove && isHit == false && isAttack == false)
            {
                if (anim.GetBool("Walk") == false)
                    anim.SetBool("Walk", true);
                movement = isRight ? Vector3.right : Vector3.left;
                transform.Translate(movement * Time.fixedDeltaTime * movepower);
                
                //if (isRight == true)
                //{
                //    movement = Vector3.right;
                //    transform.localScale = new Vector3(5, 5, 1);
                //    //rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;
                //    transform.Translate(movement * Time.fixedDeltaTime);
                //    HPBar.transform.localScale = new Vector3(0.02f, 0.02f, 1);
                //    text.transform.localScale = new Vector3(0.03f, 0.01f, 1);
                //}
                //else if (isRight == false)
                //{
                //    movement = Vector3.left;
                //    transform.localScale = new Vector3(-5, 5, 1);
                //    //rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;
                //    transform.Translate(movement * Time.fixedDeltaTime);
                //    HPBar.transform.localScale = new Vector3(-0.02f, 0.02f, 1);
                //    text.transform.localScale = new Vector3(-0.03f, 0.01f, 1);
                //}
            }

            //�������� �տ��� ���� ��ȯ
            if (rayHit.collider == null)
                isRight = isRight ? false : true;
            else if (transform.position.x > 73)
                isRight = false;
        }
        else if (isAlive && isDetect) // �÷��̾ �����ϸ� �÷��̾ ����ٴ�
        {
            isRight = chaseDirVec.x > 0f;
            if (rayHit.collider != null)
            {
                if (isAttack == false && isHit == false)
                {
                    isMove = true;
                    anim.SetBool("Walk", true);
                    chaseDirVec = GameManager.Instance.player.transform.position - transform.position;
                    chaseDirVec.y = 0;
                    transform.Translate(chaseDirVec.normalized * Time.fixedDeltaTime);
                }
            }
            else
            {
                isRight = isRight ? false : true;
                isMove = false;
                anim.SetBool("Walk", false);
            }
        }
        transform.localScale = new Vector3(isRight ? 5 : -5, 5, 1);
        HPBar.transform.localScale = new Vector3(isRight ? 0.02f : -0.02f, 0.02f, 1);
        text.transform.localScale = new Vector3(isRight ? 0.03f : -0.03f, 0.01f, 1);
    }

    public void Attack()
    {
        if (frontperception.collider != null && GameManager.Instance.player.GetIsRoll() == false && GameManager.Instance.player.GetIsHit() == false)
            frontperception.collider.transform.GetComponent<Player>().Hit(enemy_stat.Att, transform.position);
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
    //    if (Input.GetKeyDown(KeyCode.A)) // �ӽ÷� AŰ ������ ���� ü�� ���� ���� �÷��̾� �������� �ٲ� ����
    //    {
    //        enemy_stat.HP -= 100; // �÷��̾��� ���ݷ¸�ŭ ������ ����

    //        anim.SetTrigger("Hit");
    //        HPBar.value = enemy_stat.HP;
    //        Debug.Log("���� ü�� : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

    //        if (enemy_stat.HP <= 0)
    //        {                
    //            isMove = false;
    //            anim.SetTrigger("Dead");
    //            yield return new WaitForSeconds(1.5f);
    //            Debug.Log("���� ���");
    //            Destroy(gameObject); // HP = 0�Ͻ� ������
    //        }
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision) 
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        enemy_stat.HP -= 100; // �÷��̾��� ���ݷ¸�ŭ ������ ����
    //        anim.SetTrigger("Hit");
    //        HPBar.value = enemy_stat.HP;
    //        //direction = (transform.position - collision.transform.position).normalized;
    //        //direction.x += 1;
    //        //direction.y += 2;
    //        //direction *= Knockbackpower;
    //        //collision.transform.GetComponent<GameObject>();
    //        //Hit(direction);
    //        Debug.Log("���� ü�� : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

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
    //    enemy_stat.HP -= 100; // �÷��̾��� ���ݷ¸�ŭ ������ ����
    //    anim.SetTrigger("Hit");
    //    HPBar.value = enemy_stat.HP;
    //}

    public void SetInfo(Vector3 pos)
    {
        if (areaNum == 1)
            pos.x = Random.Range(50,71);

        transform.position = pos;
        enemy_stat.MaxHP = GameManager.Instance.player.myStat.Att * 5; // �÷��̾��� ���ݷ����� 5�� ������ �׵��� ����
        enemy_stat.HP = enemy_stat.MaxHP;
        enemy_stat.Att = GameManager.Instance.player.myStat.MaxHP / 10; // �÷��̾ 10�� ������ �׵��� ����

        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;

        if (col == null)
            col = GetComponent<CapsuleCollider2D>();

        if (rigid == null)
            rigid = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponent<Animator>();

        col.enabled = true;
        rigid.gravityScale = 1;
        transform.GetChild(0).gameObject.SetActive(true);
        isDetect = false;
    }

    void Die()
    {
        anim.SetTrigger("Dead");
        isMove = false;
        rigid.gravityScale = 0;
        col.enabled = false;
        StartCoroutine(WaitEnque());
        Debug.Log("���� ���");
        GameManager.Instance.player.AddKillCount();
        UIManager.Instance.killCountUpdate();
        if (Random.Range(0, 6) == 3) // 20% Ȯ���� ���ǵ��
            GameManager.Instance.player.AddPotionNum();
        UIManager.Instance.PotionNumUpdate();
        transform.GetChild(0).gameObject.SetActive(false);
        //Destroy(gameObject); // HP = 0�Ͻ� ������

    }
    public void Hit(float damage, Vector2 pos)
    {
        enemy_stat.HP = Mathf.Clamp(enemy_stat.HP - damage, 0f, enemy_stat.MaxHP);
        text.text = $"{enemy_stat.HP} / {enemy_stat.MaxHP}";
        HPBar.value = enemy_stat.HP;

        if (isAlive == false)
            Die();
        else
        {
            StartCoroutine(WaitHit());
            if (isAttack == false)
                anim.SetTrigger("Hit");
            isDetect = true;
            //#region �˹�� ���� ���ϱ�
            //Vector2 KnockVec = Vector2.zero;
            //KnockVec = (Vector2)transform.position - pos;
            //bool dirIsLeft = KnockVec.x < 0f;
            //KnockVec = dirIsLeft ? Vector2.left : Vector2.right;
            //KnockVec.y = 0.5f;
            //#endregion
            //rigid.velocity = KnockVec;
        }
    }

    IEnumerator WaitEnque() // �װ� 10���Ŀ� ���ͽ������� ť�� ���ư�
    {
        yield return new WaitForSeconds(10f);
        MonsterSpawner.Instance.MonsterEnqueue(this.gameObject);
    }
    IEnumerator WaitAttack()
    {
        isAttack = true;
        isMove = false;
        yield return new WaitForSeconds(1f);
        isAttack = false;
        isMove = true;
    }
    IEnumerator WaitHit()
    {
        isHit = true;
        isMove = false;
        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(1f);
        isHit = false;
        isMove = true;
        anim.SetBool("Walk", true);
    }
    IEnumerator AttackCoolDown()
    {
        while (attackCool > 0f)
        {
            attackCool -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (attackCool <= 0f)
        {
            StopCoroutine(attCoolCor);
            attCoolCor = null;
        }
    }
}
