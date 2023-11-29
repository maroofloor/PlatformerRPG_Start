using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    Transform[] buttonsTr;
    [SerializeField]
    Player player;
    [SerializeField]
    Text AttTxt;
    [SerializeField]
    Text DefTxt;
    [SerializeField]
    Text HPTxt;
    [SerializeField]
    Transform uiMenuTr;
    [SerializeField]
    Transform uiEnforceTr;
    [SerializeField]
    Transform warningMsgTr;
    Text warningInfoTxt;
    [SerializeField]
    Text killCountTxt;
    [SerializeField]
    Text potionNumTxt;
    [SerializeField]
    Text EnforceNumTxt;
    public Transform mobileControllerTr;
    [SerializeField]
    Transform uiPortalTr;
    [SerializeField]
    Transform LifeHeartTr;

    bool isMenuOn;
    bool isEnforceOn;
    bool isPortalInfoOn;

    public bool GetIsMenuOn()
    {
        return isMenuOn;
    }
    public bool GetIsEnforceOn()
    {
        return isEnforceOn;
    }

    public void MinusLife(int lifeNum)
    {
        LifeHeartTr.GetChild(lifeNum).gameObject.SetActive(false);
    }

    public void SetLife(int lifeNum)
    {
        for (int i = 0; i < 3; i++)
        {
            LifeHeartTr.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < lifeNum; i++)
        {
            LifeHeartTr.GetChild(i).gameObject.SetActive(true);
        }
    }

    int enforceNum => (GameManager.Instance.player.GetKillCount() - (GameManager.Instance.player.GetKillCount() % 10)) / 10;

void Start()
{
    isMenuOn = false;
    isEnforceOn = false;
    isPortalInfoOn = false;
    warningInfoTxt = warningMsgTr.GetChild(0).GetComponent<Text>();
    warningMsgTr.gameObject.SetActive(false);
    uiMenuTr.gameObject.SetActive(false);
    uiEnforceTr.gameObject.SetActive(false);
    mobileControllerTr.gameObject.SetActive(true);
    uiPortalTr.gameObject.SetActive(false);
}

void Update()
{
    AttTxt.text = "Att : " + string.Format("{0:0000}", player.myStat.Att);
    DefTxt.text = "Def : " + string.Format("{0:0000}", player.myStat.Def);
    HPTxt.text = "H P : " + string.Format("{0:0000}", player.myStat.MaxHP);
}

public void killCountUpdate()
{
    killCountTxt.text = "Point : " + string.Format("{0:000}", player.GetKillCount());
}

public void PotionNumUpdate()
{
    potionNumTxt.text = "x " + string.Format("{0:00}", player.GetPotionNum());
}

public void EnforceNumUpdate()
{
    EnforceNumTxt.text = "강화 개수 : " + string.Format("{0:00}", enforceNum);
}

public void PrintWarningMsg(string Msg)
{
    StartCoroutine(WaitMsg());
    warningInfoTxt.text = Msg;
}

IEnumerator WaitMsg()
{
    warningMsgTr.gameObject.SetActive(true);
    yield return new WaitForSeconds(3f);
    warningMsgTr.gameObject.SetActive(false);
}

public void UI_MenuBotton()
{
    if (!isEnforceOn)
    {
        if (isMenuOn)
        {
            mobileControllerTr.gameObject.SetActive(true);
            uiMenuTr.gameObject.SetActive(false);
            isMenuOn = false;
        }
        else
        {
            mobileControllerTr.gameObject.SetActive(false);
            uiMenuTr.gameObject.SetActive(true);
            isMenuOn = true;
        }
    }
    else // 강화창이 켜져있는 경우에는
    {
        uiEnforceTr.gameObject.SetActive(false);
        isEnforceOn = false;
        uiMenuTr.gameObject.SetActive(true);
        isMenuOn = true;
    }
}

public void PrintPortalInfo()
{
    if (isPortalInfoOn == false)
    {
        isPortalInfoOn = true;
        uiPortalTr.gameObject.SetActive(true);
    }
    else
    {
        isPortalInfoOn = false;
        uiPortalTr.gameObject.SetActive(false);
    }

}

public void UI_EnforceBotton()
{
    EnforceNumUpdate();
    if (!isMenuOn)
    {
        if (isEnforceOn)
        {
            mobileControllerTr.gameObject.SetActive(true);
            uiEnforceTr.gameObject.SetActive(false);
            isEnforceOn = false;
        }
        else
        {
            mobileControllerTr.gameObject.SetActive(false);
            uiEnforceTr.gameObject.SetActive(true);
            isEnforceOn = true;
        }
    }
    else // 메뉴가 켜져있는 경우에는
    {
        uiMenuTr.gameObject.SetActive(false);
        isMenuOn = false;
        uiEnforceTr.gameObject.SetActive(true);
        isEnforceOn = true;
    }
}

public void PointerDown(int num)
{
    buttonsTr[num].GetChild(0).gameObject.SetActive(false);
    buttonsTr[num].GetChild(1).gameObject.SetActive(true);
}

public void PointerUP(int num)
{
    buttonsTr[num].GetChild(0).gameObject.SetActive(true);
    buttonsTr[num].GetChild(1).gameObject.SetActive(false);
}
}
