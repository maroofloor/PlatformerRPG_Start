using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillScripts : MonoBehaviour
{
    [SerializeField]
    Player player;
    Vector2 startPos = Vector2.zero; // ���ư��� �����ϴ� ����
    Vector2 scaleVec = Vector2.one; // ���ư� ũ�� �� �¿������
    Vector2 shotDir; // ���ư��� ����
    float size; // ũ��
    float speed; // ���ư��� �ӵ�

    Coroutine cor = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.transform.TryGetComponent<Boss>(out Boss boss))
                boss.Hit(player.myStat.Att * 2, player.transform.position);
            else
                collision.GetComponent<Monster>().Hit(player.myStat.Att * 2, player.transform.position);
        }
    }

    public void SetInfo()
    {
        shotDir = player.GetIsLeft() ? Vector2.left : Vector2.right;
        speed = 3;
        size = 3;
        startPos = player.transform.position;
        startPos.x += player.GetIsLeft() ?  -0.7f : 0.7f;
        startPos.y += 0.7f;
        scaleVec = player.scaleVec * size;
        transform.position = startPos;
        transform.localScale = scaleVec;
        if (cor == null)
            cor = StartCoroutine(ShotSkill());
    }

    IEnumerator ShotSkill()
    {
        SoundManager.Instance.SetSoundEffect(16, transform.position);
        float Distance = Vector2.Distance(startPos, transform.position); ;
        while (Distance < 5f)
        {
            transform.Translate(shotDir * Time.fixedDeltaTime * speed);
            Distance = Vector2.Distance(startPos, transform.position);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        if (Distance >= 5f)
        {
            if (cor != null)
            {
                StopCoroutine(cor);
                cor = null;
            }
            gameObject.SetActive(false);
        }
    }
}
