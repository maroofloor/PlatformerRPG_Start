using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Monster : MonoBehaviour
{
    public AllStruct.Stat stat;

    private void Start()
    {
        stat = new AllStruct.Stat(50,10);
    }

    private void FixedUpdate()
    {
        Debug.Log("���� ü�� : " + stat.HP + " / " + stat.MaxHP);
    }

    
}
