using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//인풋매니저에다가 bool 선언한고 
//awake 단계에서

//#if UNITY_ANDROID
//IsAndorid = true;
//#else
//IsAndroid = false;
//#endif

public class InputManager : MonoBehaviour
{
    [SerializeField]
    VariableJoystick variableJoystick;

    public static bool IsAndroid;
    [SerializeField]
    Player player;
    public Vector3 dir = Vector3.zero;
    public Vector3 joyDir = Vector3.zero;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
            IsAndroid = true;
        else
            IsAndroid = false;
    }

    void Update()
    {
        joyDir.x = Vector3.right.x * variableJoystick.Horizontal;
        player.vec.x = joyDir.x;

        if (!IsAndroid)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                player.vec.x = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.S))
                GameManager.Instance.player.Skill();
            if (Input.GetKeyDown(KeyCode.C))
                player.Jump();
            if (Input.GetKeyDown(KeyCode.Z))
                player.Attack();
            if (Input.GetKeyDown(KeyCode.X))
                player.Roll();
            if (Input.GetKeyDown(KeyCode.Q))
                player.UsePotion();
            if (Input.GetKeyDown(KeyCode.B))
                UIManager.Instance.UI_EnforceBotton();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (UIManager.Instance.GetIsEnforceOn())
                {
                    UIManager.Instance.UI_EnforceBotton();
                    return;
                }
                UIManager.Instance.UI_MenuBotton();
            }
        }
    }

    private void FixedUpdate()
    {

    }
}
