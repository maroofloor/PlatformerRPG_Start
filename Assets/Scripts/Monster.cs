using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Monster : MonoBehaviour
{
    public float movepower = 1f;
    Animator anim;
    Vector3 movement = Vector3.zero;    
    Rigidbody2D rigid;      
    bool isRight; // �����̴� �������ִ� ���� �ٶ󺸴� ���� == true�� ������, false�� ���� 
    bool isMove; // true�� �����̰� false�� ����. Ʈ���϶� �����ϰǵ� �ϴ� ���ʹ������θ� �����ϴµ� isRight�� Ʈ��� *1 �޽��� *(-1)

    public AllStruct.Stat enemy_stat;
    Player player;

    [SerializeField]
    Slider HPBar;

    Vector2 e_attack = Vector2.zero;
    RaycastHit2D frontperception;
    RaycastHit2D backperception;


    void Start()
    {
        enemy_stat = new AllStruct.Stat(500, 50); // ���� ���� �ӽ÷� ����
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Changemovement());
        isRight = true;
    }

    IEnumerator Changemovement()
    {
        while (true)
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
        StartCoroutine(Hit());
    }
    void FixedUpdate()
    {        
        //�������� �տ��� ���� ��ȯ
        Vector2 frontVec = new Vector2(rigid.position.x + (isRight? 0.5f:- 0.5f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        if (rayHit.collider == null)
        {           
            isRight = isRight ? false : true;
        }

        //���ݹ���
        e_attack.x = rigid.position.x;
        e_attack.y = rigid.position.y + 0.5f;
        
        if (isRight == false) // ������ ��...
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
        //�ڿ� �÷��̾� ����
        if (backperception.collider != null)
        {
            //Debug.Log("�ڿ� �÷��̾� ����");
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

        if (enemy_stat.HP <= 0)
        {
            isMove = false;
            StartCoroutine(Hit());
        }
    }
    void LateUpdate()
    {
        if (isMove)
        {

            if (isRight == true)
            {
                movement = Vector3.right;
                transform.localScale = new Vector3(5, 5, 1);
                rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;
                HPBar.transform.localScale = new Vector3(0.02f, 0.02f, 1);
            }
            else if (isRight == false)
            {
                movement = Vector3.left;
                transform.localScale = new Vector3(-5, 5, 1);
                rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;
                HPBar.transform.localScale = new Vector3(-0.02f, 0.02f, 1);
            }
        }
    }

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

    IEnumerator Hit()
    {
        if (Input.GetKeyDown(KeyCode.A)) // �ӽ÷� AŰ ������ ���� ü�� ���� ���� �÷��̾� �������� �ٲ� ����
        {
            enemy_stat.HP -= 100; // �÷��̾��� ���ݷ¸�ŭ ������ ����
            anim.SetTrigger("Hit");
            HPBar.value = enemy_stat.HP;
            Debug.Log("���� ü�� : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

            if (enemy_stat.HP <= 0)
            {               
                anim.SetTrigger("Dead");
                yield return new WaitForSeconds(1f);
                Debug.Log("���� ���");
                Destroy(gameObject); // HP = 0�Ͻ� ������
            }
        }
    }
    //IEnumerator Battle()
    //{
        
    //    yield return new WaitForSeconds(1f);
    //}
    
}
