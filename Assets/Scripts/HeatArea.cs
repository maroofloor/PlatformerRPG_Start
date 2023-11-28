using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatArea : MonoBehaviour
{
    float DamageCool;
    Coroutine cor = null;

    private void Start()
    {
        DamageCool = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.PrintWarningMsg("열기로 인해 지속적인 피해를 입습니다.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.GetComponent<Player>().isAlive)
        {
            if (DamageCool <= 0f && cor == null)
            {
                collision.GetComponent<Player>().Hit(10, Vector2.zero);
                //collision.GetComponent<Animator>().SetTrigger("Heat");
                DamageCool = 5f;
                cor = StartCoroutine(DamageCoolDown());
            }
        }
    }

    IEnumerator DamageCoolDown()
    {
        while (DamageCool > 0f)
        {
            DamageCool -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (DamageCool <= 0f && cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }
}
