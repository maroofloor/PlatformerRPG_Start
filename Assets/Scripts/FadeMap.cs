using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMap : MonoBehaviour
{
    SpriteRenderer spriter;
    float alpha;
    Vector4 colorVec;
    void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        colorVec = new Vector4(1f, 1f, 1f, 0f);
        spriter.color = colorVec;
    }
    void Update()
    {
        if (GameManager.Instance.player.transform.position.x >= 40f && GameManager.Instance.player.transform.position.x <= 41f)
            colorVec.w = GameManager.Instance.player.transform.position.x - 40;
        else if (GameManager.Instance.player.transform.position.x < 40f)
            colorVec.w = 0f;
        else if (GameManager.Instance.player.transform.position.x > 41f)
            colorVec.w = 1f;
            spriter.color = colorVec;
    }
}
