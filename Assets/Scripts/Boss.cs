using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AllStruct;

public class Boss : MonoBehaviour, AllInterface.IHit
{
    Animator anim;
    public float movepower = 1f;
    public Transform Target;
    Rigidbody2D rigid;
    public AllStruct.Stat Boss_stat;
    public BoxCollider2D col;
    public float attspeed;
    SpriteRenderer sprite;
    [SerializeField]
    Slider HPBar;
    float dir;
    bool isAlive => Boss_stat.HP > 0;
    bool isPhase2 => Boss_stat.HP <= Boss_stat.MaxHP * 0.3f;
    bool ismove;
    bool isattack;
    bool isdetected;
    bool isLeft;
    bool isNextPhase;

    float attackCool = 0;
    Coroutine attackCor = null;

    float skillCool = 0;
    Coroutine skillCor = null;

    Vector2 hitVec = Vector2.zero;

    RaycastHit2D hit;

    [SerializeField]
    GameObject SkillPrefab;
    Queue<GameObject> skills = new Queue<GameObject>();

    void Start()
    {
<<<<<<< HEAD
        Boss_stat = new AllStruct.Stat(10000, 300, 100);
=======
        Boss_stat = new AllStruct.Stat(15000, 300, 100);
>>>>>>> main
        HPBar.maxValue = Boss_stat.MaxHP;
        HPBar.value = Boss_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ismove = false;
        isLeft = true;
        isdetected = false;
        for (int i = 0; i < 5; i++)
        {
            GameObject tmp = Instantiate(SkillPrefab, this.transform.parent);
            skills.Enqueue(tmp);
            tmp.gameObject.SetActive(false);
        }
        isNextPhase = false;
    }

    void Update()
    {
        if (isAlive)
        {
            //ismove = false;
            //anim.SetBool("Boss_Walk", ismove);
            float dis = Vector3.Distance(transform.position, Target.position);
            if (dis <= 10 /*&& dis > 5*/)
                isdetected = true;
            //else if (dis <= 5)
            //{
            //    Attack();
            //}

            if (isdetected && isattack == false && isNextPhase == false)
            {
                hitVec.x = transform.position.x;
                hitVec.y = transform.position.y + 0.5f;
                Debug.DrawRay(hitVec, (isLeft ? Vector2.left : Vector2.right) * 5.5f, Color.magenta);
                hit = Physics2D.Raycast(hitVec, isLeft ? Vector2.left : Vector2.right, 5.5f, 1 << LayerMask.NameToLayer("Player"));

                if (skillCool <= 0f)
                    UseSkill();
                else if (hit.collider != null && attackCool <= 0f)
                    Attack();
                else
                {
                    if (dis > 3f )
                    {
                        Move();
                    }
                    else
                    {
                        ismove = false;
                        anim.SetBool("Boss_Walk", ismove);
                    }
                }
                //Hit(1000, Vector2.zero);
            }
            else
            {
                ismove = false;
                anim.SetBool("Boss_Walk", ismove);
            }

            if (Input.GetKeyDown(KeyCode.A))
                Hit(10000, transform.position);
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.player.isAlive == false)
        {
            isdetected = false;
            transform.localPosition = new Vector2(18, 0);
            Boss_stat.HP = Boss_stat.MaxHP;
<<<<<<< HEAD
            Boss_stat.Att = 300;
            Boss_stat.Def = 100;
=======
            HPBar.value = Boss_stat.HP;
            Boss_stat.Att = 300;
            Boss_stat.Def = 100;
            count = 0;
>>>>>>> main
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

   void UseSkill()
    {
        //Debug.Log("UseSkill �Ҹ�");
        Move();
        Attack();
        if (skillCool <= 0f && skillCor == null)
        {
            StartCoroutine(PlaySkill());
            if (isPhase2)
                skillCool = 5f;
            else
                skillCool = 10f;
            if (skillCor == null)
                skillCor = StartCoroutine(SkillCoolDown());
        }
    }

    void Attack()
    {
        StartCoroutine(WaitAttack());
        anim.SetTrigger("Boss_Attack");
        attackCool = 5f;
        if (attackCor == null)
            attackCor = StartCoroutine(AttackCoolDown());
    }

    public void AttackEvent()
    {
        SoundManager.Instance.SetSoundEffect(11, transform.position);

        hitVec.x = transform.position.x;
        hitVec.y = transform.position.y + 0.5f;
        hit = Physics2D.Raycast(hitVec, isLeft ? Vector2.left : Vector2.right, 5.5f, 1 << LayerMask.NameToLayer("Player"));
        if (hit.collider != null && GameManager.Instance.player.GetIsRoll() == false)
            hit.collider.transform.GetComponent<Player>().Hit(Boss_stat.Att, transform.position);
    }

    IEnumerator SkillCoolDown()
    {
        //Debug.Log("SkillCoolDown �Ҹ�");
        while (skillCool > 0f)
        {
            skillCool -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (skillCool <= 0f && skillCor != null)
        {
            StopCoroutine(skillCor);
            skillCor = null;
        }
    }

    IEnumerator PlaySkill()
    {
        //Debug.Log("PlaySkill �Ҹ�");
        Vector2 startVec; //new Vector2(transform.position.x + (5.5f * (isLeft ? -1f : 1f)), transform.position.y - 0.4f);
        startVec.x = transform.position.x + (5.5f * (isLeft ? -1f : 1f));
        startVec.y = transform.position.y - 0.4f;

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.8f);
            GameObject tmp = skills.Dequeue();
            if (i == 0)
                tmp.transform.position = startVec;
            else
                startVec.x += isLeft ? -3f : 3f;

            if (startVec.x > 121.5f)
                startVec.x = 121.5f;
            else if (startVec.x < 86.5f)
                startVec.x = 86.5f;

            tmp.transform.position = startVec;
            tmp.SetActive(true);
            tmp.GetComponent<BossSkillScripts>().SetInfo();
        }
    }

    public void SkillsEnqueue(GameObject skill)
    {
        skills.Enqueue(skill);
        skill.gameObject.SetActive(false);
    }

    IEnumerator WaitAttack()
    {
        isattack = true;
        if (isPhase2)
            yield return new WaitForSeconds(2f);
        else
            yield return new WaitForSeconds(3.5f);
        isattack = false;
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
    //    if (Input.GetKeyDown(KeyCode.A)) // �ӽ÷� AŰ ������ ���� ü�� ���� ���� �÷��̾� �������� �ٲ� ����
    //    {
    //        Boss_stat.HP -= 1000; // �÷��̾��� ���ݷ¸�ŭ ������ ����

    //        anim.SetTrigger("Boss_HIt");
    //        HPBar.value = Boss_stat.HP;
    //        Debug.Log("�������� ü�� : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);

    //        if (Boss_stat.HP <= 0)
    //        {
    //            ismove = false;
    //            anim.SetTrigger("Boss_Death");
    //            yield return new WaitForSeconds(1.5f);                
    //            Destroy(gameObject); // HP = 0�Ͻ� ������
    //        }
    //    }
    //}
    int count = 0;
    public void Hit(float damage, Vector2 pos)
    {
<<<<<<< HEAD
        float damageVal = damage - Boss_stat.Def;
=======
        if (isNextPhase)
            return;

>>>>>>> main
        if (isAlive == false)
        {
            Debug.Log("����");
            Die();
        }
        else
        {
<<<<<<< HEAD
            Boss_stat.HP -= damageVal; // �÷��̾��� ���ݷ¸�ŭ ������ ����
            SoundManager.Instance.SetSoundEffect(Random.Range(12,15), transform.position);
            if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
            { 
                movepower = 3f;
                Boss_stat.Att = 1000;
                Boss_stat.Def = 300;                
            }
                
            if (attackCool <= 3.5f)
=======
            float damageValue;
            damageValue = Mathf.Clamp(damage - Boss_stat.Def, 0, Mathf.Infinity);
            Boss_stat.HP -= damageValue;
            SoundManager.Instance.SetSoundEffect(Random.Range(12,15), transform.position);
            if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
            {
                count++;
                movepower = 3f;
                Boss_stat.Att = 500;
                Boss_stat.Def = 300;
                if (count == 1)
                {
                    anim.SetTrigger("Boss_NextPhase");
                    StartCoroutine(WaitNextPhase());
                }
               
            }

            if ((attackCool <= 2f && isPhase2) || (attackCool <= 3.5f && isPhase2 == false))
>>>>>>> main
                anim.SetTrigger("Boss_HIt");
            HPBar.value = Boss_stat.HP;
            Debug.Log("�������� ü�� : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Boss_stat.HP -= damage; // �÷��̾��� ���ݷ¸�ŭ ������ ����
        //    if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
        //        movepower = 3f;
        //    anim.SetTrigger("Boss_HIt");
        //    HPBar.value = Boss_stat.HP;
        //    Debug.Log("�������� ü�� : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);
        //}
        //if (Boss_stat.HP <= 0)
        //{
        //    Debug.Log("����");
        //    Die();
        //}
    }

    IEnumerator WaitNextPhase()
    {
        isNextPhase = true;
        yield return new WaitForSeconds(0.75f);
        isNextPhase = false;

    }

    void Die()
    {
        SoundManager.Instance.SetSoundEffect(17, transform.position);
        anim.SetTrigger("Boss_Death");
        ismove = false;
        rigid.gravityScale = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        GameManager.Instance.PrintClearScreen();
    }

    public void WalkSound()
    {
        SoundManager.Instance.SetSoundEffect(18, transform.position);
    }
}


