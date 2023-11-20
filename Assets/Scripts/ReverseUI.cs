using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseUI : MonoBehaviour
{
    void Update()
    {
        transform.localScale = GameManager.Instance.player.scaleVec;
    }
}
