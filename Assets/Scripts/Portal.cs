using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    Transform portalPosTr;
    Vector2 portalPosVec;

    [SerializeField]
    bool OnPotral;

    void Start()
    {
        portalPosVec = portalPosTr.position;
        OnPotral = false;
    }

    void Update()
    {
        if (OnPotral && Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameManager.Instance.player.transform.position = portalPosVec;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            OnPotral = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            OnPotral = false;
    }
}
