using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillScripts : MonoBehaviour
{
    Animator anim;
    BoxCollider2D col;

    public Animator GetAnim()
    {
        return anim;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.GetComponent<Player>().GetIsRoll() == false)
        {
            collision.transform.GetComponent<Player>().Hit(300, GameManager.Instance.boss.transform.position);
        }
    }

    public void Sound()
    {
        SoundManager.Instance.SetSoundEffect(15, transform.position);
    }

    public void SetInfo()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        if (col == null)
            col = GetComponent<BoxCollider2D>();
        anim.SetBool("Empty", true);
        anim.SetTrigger("Boom");
        col.enabled = false;
        StartCoroutine(WaitEnqueue());
    }

    public void SetColliderOn()
    {
        col.enabled = true;
    }

    IEnumerator WaitEnqueue()
    {
        yield return new WaitForSeconds(1.25f);
        col.enabled = false;
        GameManager.Instance.boss.SkillsEnqueue(this.gameObject);
    }
}
