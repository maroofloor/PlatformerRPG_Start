using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    public Transform[] points;
    public GameObject monsterPrefab;
    //List<GameObject> MonsterList = new List<GameObject>();
    public Queue<GameObject> MonsterQueue = new Queue<GameObject>();

    public float createTime;
    public int maxMonster;

    void Start()
    {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        for (int i = 0; i < 8; i++)
        {
            GameObject tmp = Instantiate(monsterPrefab, this.transform);
            if (i < 5)
                tmp.GetComponent<Monster>().areaNum = 1;
            else if (i == 5)
                tmp.GetComponent<Monster>().areaNum = 2;
            else
                tmp.GetComponent<Monster>().areaNum = 3;
            tmp.gameObject.SetActive(false);
            MonsterQueue.Enqueue(tmp);
        }

        if (points.Length > 0)
            StartCoroutine(CreateMonster());
    }

    void ActiveMonster()
    {
        if (MonsterQueue.Count > 0)
        {
            GameObject tmp = MonsterQueue.Dequeue();
            tmp.GetComponent<Monster>().SetInfo(points[tmp.GetComponent<Monster>().areaNum].position);
            tmp.SetActive(true);
            tmp.GetComponent<Monster>().StartChangeMovement();

            //if (tmp.GetComponent<Monster>().cor == null)
            //    tmp.GetComponent<Monster>().cor = StartCoroutine(tmp.GetComponent<Monster>().Changemovement());
            //tmp.transform.position = ;
            //tmp.GetComponent<Monster>().enemy_stat.HP = tmp.GetComponent<Monster>().enemy_stat.MaxHP;
            //if (tmp.GetComponent<Monster>().col.enabled == false)
            //    tmp.GetComponent<Monster>().col.enabled = true;
        }
        else
            return;
    }

    public void MonsterEnqueue(GameObject monster)
    {
        MonsterQueue.Enqueue(monster);
        monster.SetActive(false);
    }

    IEnumerator CreateMonster()
    {
        while (true)
        {
            ActiveMonster();
            yield return new WaitForSeconds(createTime);
        }

        //while (true)
        //{
        //    int monsterCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length;
        //    if (monsterCount <= maxMonster)
        //    {
        //        yield return new WaitForSeconds(createTime);
        //        int area = Random.Range(1, points.Length);
        //        GameObject instance = Instantiate(monsterPrefab, points[area].position, Quaternion.identity);
        //        MonsterList.Add(instance);
        //        Debug.Log($"현재 몬스터 수 : {monsterCount}/최대 몬스터 수 : {maxMonster}");
        //    }
        //    else
        //    {
        //        yield return null;
        //    }
        //}
    }

}
