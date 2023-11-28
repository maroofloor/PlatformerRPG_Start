using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillScripts : MonoBehaviour
{
    Animator anim;
    public Animator GetAnim()
    {
        return anim;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.GetComponent<Player>().GetIsRoll() == false)
        {
            collision.transform.GetComponent<Player>().Hit(300, GameManager.Instance.boss.transform.position);
        }
    }

    public void SetInfo()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        anim.SetBool("Empty", true);
        anim.SetTrigger("Boom");
        StartCoroutine(WaitEnqueue());
    }

    IEnumerator WaitEnqueue()
    {
        yield return new WaitForSeconds(1.25f);
        GameManager.Instance.boss.SkillsEnqueue(this.gameObject);
    }
}
