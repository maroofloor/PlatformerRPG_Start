using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Monster : MonoBehaviour
{    
    public float movepower = 1f;
    Animator anim;
    Vector3 movement;
    Rigidbody2D rigid;
    int moveFlag = 0; // 0 ���� 1 ���� 2 ������
    public AllStruct.Stat enemy_stat;
    Player player;
    

    [SerializeField]
    Slider HPBar;
    
    void Start()
    {
        enemy_stat = new AllStruct.Stat(500,50); // ���� ���� �ӽ÷� ����
        HPBar.maxValue = enemy_stat.MaxHP;
        HPBar.value = enemy_stat.HP;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Changemovement());       
    }

    IEnumerator Changemovement()
    {
        while(true)
        {
            moveFlag = Random.Range(0, 3);
            if (moveFlag == 0)
            {
                anim.SetBool("Walk", false);
            }
            else
            {
                anim.SetBool("Walk", true);
            }
            yield return new WaitForSeconds(3f);

        }
    }

   
    void Update()
    {
        Move();        
        StartCoroutine(Hit());
    }
   

    void Move()
    {
        Vector3 movevelocity = Vector3.zero;
        if (moveFlag == 1) 
        { 
            movevelocity = Vector3.left;
            transform.localScale = new Vector3(-5, 5, 1);
            rigid.velocity = new Vector2(-1, rigid.velocity.y) * movepower;      

        }
        else if (moveFlag == 2)
        {
            movevelocity = Vector3.right;
            transform.localScale = new Vector3(5, 5, 1);
            rigid.velocity = new Vector2(1, rigid.velocity.y) * movepower;      
        }
       
        
    }

    IEnumerator Hit()
    {
        if (Input.GetKeyDown(KeyCode.A)) // �ӽ÷� AŰ ������ ���� ü�� ���� ���� �÷��̾� �������� �ٲ� ����
        {
            enemy_stat.HP -= 100; // �÷��̾��� ���ݷ¸�ŭ ������ ����

            HPBar.value = enemy_stat.HP;
            Debug.Log("���� ü�� : " + enemy_stat.HP + " / " + enemy_stat.MaxHP);

            if (enemy_stat.HP <= 0)
            {
                //��� �ִϸ��̼� ���鿹��
                yield return new WaitForSeconds(1.5f);
                Debug.Log("���� ���");
                Destroy(gameObject); // HP = 0�Ͻ� ������
            }
        }
    }
}
