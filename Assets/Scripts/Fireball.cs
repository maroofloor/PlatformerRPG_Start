using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Singleton<Fireball>
{
    public Transform[] points;
    public GameObject Trap;
    public Queue<GameObject> TrapQueue = new Queue<GameObject>();
    public float createTime = 1f;
    public int max = 10;

    void Start()
    {
        Transform parent = GameObject.Find("Trap").transform;
        points = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            points[i] = parent.GetChild(i);
        }
        //points = GameObject.Find("Trap").GetComponentsInChildren<Transform>();

        for (int i = 0; i < max; i++)
        {
            GameObject tmp = Instantiate(Trap);
            tmp.gameObject.SetActive(false);
            TrapQueue.Enqueue(tmp);
        }
        if (points.Length > 0)
        {
            StartCoroutine(CreateTrap());
        }
    }

    void ActiveTrap()
    {
        int area = Random.Range(0, points.Length);
        if (TrapQueue.Count > 0)
        {
            GameObject tmp = TrapQueue.Dequeue();
            tmp.transform.position = points[area].position;
            tmp.SetActive(true);
        }
        else
            return;
    }
 
    IEnumerator CreateTrap()
    {
        while (true)
        {
            ActiveTrap();
            yield return new WaitForSeconds(createTime);
        }
    }

    public void TrapEnqueue(GameObject Trap)
    {
        TrapQueue.Enqueue(Trap);
        Trap.SetActive(false);
    }
}
   

