using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    Transform portalPosTr;

    [SerializeField]
    bool OnPotral;

    void Start()
    {
        OnPotral = false;
    }

    public void PortalYes()
    {
        GameManager.Instance.player.transform.position = portalPosTr.position;
        UIManager.Instance.PrintPortalInfo();
    }
    public void PortalNo()
    {
        UIManager.Instance.PrintPortalInfo();
    }

    void Update()
    {
        if (OnPotral && Input.GetKeyDown(KeyCode.DownArrow))
        {
            UIManager.Instance.PrintPortalInfo();
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
