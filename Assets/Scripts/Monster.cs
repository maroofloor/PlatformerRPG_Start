using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{    
    public float movepower = 1f;
    Animator anim;
    Vector3 movement;
    Rigidbody2D rigid;
    int moveFlag = 0; // 0 정지 1 왼쪽 2 오른쪽
    
    void Start()
    {        
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
}
