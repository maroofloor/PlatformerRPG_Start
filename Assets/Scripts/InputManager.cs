using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//인풋매니저에다가 bool 선언한고 
//awake 단계에서

//#if UNITY_ANDROID
    //IsAndorid = true;
//#else
    //IsAndroid = false;
//#endif

public class InputManager : MonoBehaviour
{
    public static bool IsAndroid;
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
            IsAndroid = true;
        else
            IsAndroid = false;
    }

    void Update()
    {
        if (!IsAndroid)
        {
            if (Input.GetKeyDown(KeyCode.C))
                GameManager.Instance.player.Jump();
            if (Input.GetKeyDown(KeyCode.Z))
                GameManager.Instance.player.Attack();
            if (Input.GetKeyDown(KeyCode.X))
                GameManager.Instance.player.Roll();
        }
    }
}
