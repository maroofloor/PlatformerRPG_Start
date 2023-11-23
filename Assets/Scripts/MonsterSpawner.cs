using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform[] points;  
    public GameObject monsterPrefab;
    List<GameObject> MonsterList = new List<GameObject>();

    public float createTime;    
    public int maxMonster;
    
    void Start()
    {        
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        if (points.Length > 0)
        {            
            StartCoroutine(CreateMonster());
        }
    }

    IEnumerator CreateMonster()
    {
        
        while (true)
        {             
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("Monster").Length;
            if (monsterCount <= maxMonster)
            {                
                yield return new WaitForSeconds(createTime);                
                int area = Random.Range(1, points.Length);                
                GameObject instance = Instantiate(monsterPrefab, points[area].position, Quaternion.identity);
                MonsterList.Add(instance);
                Debug.Log($"현재 몬스터 수 : {monsterCount}/최대 몬스터 수 : {maxMonster}");
            }
            else
            {
                yield return null;
            }
        }
    }
   
}
