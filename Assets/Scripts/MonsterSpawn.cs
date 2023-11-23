using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    public GameObject Enemy;
    [SerializeField]
    int count = 0; //몬스터 마릿수
    
    private BoxCollider2D area;
    private List<GameObject> MonsterList = new List<GameObject>();

    void Start()
    {        
        area = GetComponent<BoxCollider2D>();
        StartCoroutine("Spawn", 60); 
    }
    
    private IEnumerator Spawn(float delayTime)
    {
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = GetRandomPosition();
                GameObject instance = Instantiate(Enemy, spawnPos, Quaternion.identity);
                MonsterList.Add(instance);
            }
            area.enabled = false;
            yield return new WaitForSeconds(60f);   

            for (int i = 0; i < count; i++)
                Destroy(MonsterList[i].gameObject);

            MonsterList.Clear();
            area.enabled = true;
        }
    }

    
    private Vector2 GetRandomPosition()
    {
        Vector2 basePosition = transform.position;  
        Vector2 size = area.size;                   
       
        float posX = basePosition.x + Random.Range(-size.x / 2f, size.x / 2f);
        float posY = basePosition.y + Random.Range(-size.y / 2f, size.y / 2f);

        Vector2 spawnPos = new Vector2(posX, posY);

        return spawnPos;
    }




}
