using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour, AllInterface.IHit
{
    Animator anim;
    public float movepower = 3f;
    public Transform Target;
    //Rigidbody2D rigid;
    public AllStruct.Stat Boss_stat;

    [SerializeField]
    Slider HPBar;
    float dir;
    bool ismove;
    //bool isAlive;

    void Start()
    {        
        Boss_stat = new AllStruct.Stat(10000, 300, 100);
        HPBar.maxValue = Boss_stat.MaxHP;
        HPBar.value = Boss_stat.HP;
        //rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }
    void Update()
    {
        ismove = false;
        anim.SetBool("Boss_Walk", ismove);
        float dis = Vector3.Distance(transform.position, Target.position);
        if (dis <= 10 && dis > 5)
        {
            Move();
        }
        else if (dis <= 5)
        {
            Attack();
        }
        else
        {
            anim.SetBool("Boss_Attack", false);
            return; 
        }
    }

    void Move()
    {
        dir = Target.position.x - transform.position.x; 
        dir = (dir < 0) ? -1 : 1;         
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
    void Attack()
    {        
        dir = (dir < 0) ? -1 : 1;        
        anim.SetBool("Boss_Attack",true);
    }
    

    public void Hit(float damage, Vector2 pos)
    {
       
    }
}

