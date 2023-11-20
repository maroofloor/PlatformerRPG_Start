using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instace = null;
    public static T Instance => instace;
    void Awake()
    {
        if (instace == null)
        {
            instace = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instace != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
