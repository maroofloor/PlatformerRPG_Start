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

    bool isMenuOn;
    bool isEnforceOn;

    int enforceNum => (GameManager.Instance.player.killCount - (GameManager.Instance.player.killCount % 10)) / 10;

    void Start()
    {
        isMenuOn = false;
        isEnforceOn = false;
        warningInfoTxt = warningMsgTr.GetChild(0).GetComponent<Text>();
        warningMsgTr.gameObject.SetActive(false);
        uiMenuTr.gameObject.SetActive(false);
        uiEnforceTr.gameObject.SetActive(false);
        mobileControllerTr.gameObject.SetActive(true);
    }

    void Update()
    {
        AttTxt.text = "Att : " + string.Format("{0:0000}", player.myStat.Att);
        DefTxt.text = "Def : " + string.Format("{0:0000}", player.myStat.Def);
        HPTxt.text = "H P : " + string.Format("{0:0000}", player.myStat.MaxHP);

        if (Input.GetKeyDown(KeyCode.Escape))
            UI_MenuBotton();

        if (Input.GetKeyDown(KeyCode.B))
            UI_EnforceBotton();
    }

    public void killCountUpdate()
    {
        killCountTxt.text = "Kill : " + string.Format("{0:000}", player.killCount);
    }

    public void PotionNumUpdate()
    {
        potionNumTxt.text = "x " + string.Format("{0:00}", player.potionNum);
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
            uiEnforceTr.gameObject.SetActive(false); // 강화창을 끄고 메뉴 켜기
            isEnforceOn = false;

            uiMenuTr.gameObject.SetActive(true);
            isMenuOn = true;
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
            isMenuOn = false; // 메뉴를 끄고 강화창 켜기

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
