using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    public Player player;
    [SerializeField]
    Transform mainCanvasTr;
    [SerializeField]
    Image playerHPBar;
    [SerializeField]
    Text potionNumTxt;
    [SerializeField]
    Image RollCoolImg;
    float rollMaxCool = 3f;
    public InputManager inputManager;

    private void Start()
    {
        player.gameObject.SetActive(true);
    }
    void Update()
    {
        RollCoolImg.fillAmount = player.rollCool / rollMaxCool;
    }
    private void LateUpdate()
    {
        potionNumTxt.text = "x " + string.Format("{0:00}",player.potionNum);
    }
    void FixedUpdate()
    {
        playerHPBar.fillAmount = player.myStat.HP / player.myStat.MaxHP;
    }

    public void SAVEButtonClicked()
    {
    }
    
}
