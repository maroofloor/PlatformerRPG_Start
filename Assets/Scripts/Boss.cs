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
    public BoxCollider2D col;
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

    float skillCool = 0;
    Coroutine skillCor = null;

    Vector2 hitVec = Vector2.zero;

    RaycastHit2D hit;

    [SerializeField]
    GameObject SkillPrefab;
    Queue<GameObject> skills = new Queue<GameObject>();

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
        for (int i = 0; i < 5; i++)
        {
            GameObject tmp = Instantiate(SkillPrefab, this.transform.parent);
            skills.Enqueue(tmp);
            tmp.gameObject.SetActive(false);
        }
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

            if (isdetected && isattack == false)
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
                    if (dis > 3f)
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
        //Debug.Log("UseSkill 불림");
        Move();
        Attack();
        if (skillCool <= 0f && skillCor == null)
        {
            StartCoroutine(PlaySkill());
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
        //Debug.Log("SkillCoolDown 불림");
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
        //Debug.Log("PlaySkill 불림");
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
        if (isAlive == false)
        {
            Debug.Log("죽음");
            Die();
        }
        else
        {
            Boss_stat.HP -= damage; // 플레이어의 공격력만큼 데미지 입음
            SoundManager.Instance.SetSoundEffect(Random.Range(12,15), transform.position);
            if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
                movepower = 3f;
            if (attackCool <= 3.5f)
                anim.SetTrigger("Boss_HIt");
            HPBar.value = Boss_stat.HP;
            Debug.Log("보스몬스터 체력 : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Boss_stat.HP -= damage; // 플레이어의 공격력만큼 데미지 입음
        //    if (Boss_stat.HP <= (Boss_stat.MaxHP) * 0.3f)
        //        movepower = 3f;
        //    anim.SetTrigger("Boss_HIt");
        //    HPBar.value = Boss_stat.HP;
        //    Debug.Log("보스몬스터 체력 : " + Boss_stat.HP + " / " + Boss_stat.MaxHP);
        //}
        //if (Boss_stat.HP <= 0)
        //{
        //    Debug.Log("죽음");
        //    Die();
        //}
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


