using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamereManager : Singleton<CamereManager>
{
    [SerializeField]
    CinemachineVirtualCamera MainCamera;
    [SerializeField]
    CinemachineVirtualCamera BossCamera;

    void Start()
    {
        MainCamera.gameObject.SetActive(true);
        BossCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance.player.transform.position.x > 80)
        {
            BossCamera.gameObject.SetActive(true);
            MainCamera.gameObject.SetActive(false);
        }
        else
        {            
            MainCamera.gameObject.SetActive(true);
            BossCamera.gameObject.SetActive(false);
        }
        
    }
}
