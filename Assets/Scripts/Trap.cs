using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.GetComponent<Player>().GetIsRoll() == false)
        {
            GameManager.Instance.player.Hit(10, transform.position);
            Fireball.Instance.TrapEnqueue(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Fireball.Instance.TrapEnqueue(this.gameObject);
        }
    }
}
