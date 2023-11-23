using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Monster : MonoBehaviour, AllInterface.IHit
{
    public AllStruct.Stat stat;

    private void Start()
    {
        stat = new AllStruct.Stat(50,10);
    }

    private void FixedUpdate()
    {
        Debug.Log("몬스터 체력 : " + stat.HP + " / " + stat.MaxHP);
    }

    public void Hit(float damage, Vector2 pos)
    {
        Mathf.Clamp(stat.HP -= damage, 0, stat.MaxHP);
    }
}
