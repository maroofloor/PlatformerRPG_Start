using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    Transform savebuttonTr;
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
    Transform mobileControllerTr;
    [SerializeField]
    Transform warningMsgTr;
    Text warningInfoTxt;
    bool isMenuOn;
    bool isEnforceOn;

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
        else // ��ȭâ�� �����ִ� ��쿡��
        {
            uiEnforceTr.gameObject.SetActive(false); // ��ȭâ�� ���� �޴� �ѱ�
            isEnforceOn = false;

            uiMenuTr.gameObject.SetActive(true);
            isMenuOn = true;
        }

    }

    public void UI_EnforceBotton()
    {
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
        else // �޴��� �����ִ� ��쿡��
        {
            uiMenuTr.gameObject.SetActive(false);
            isMenuOn = false; // �޴��� ���� ��ȭâ �ѱ�

            uiEnforceTr.gameObject.SetActive(true);
            isEnforceOn = true;
        }
    }

    public void PointerDown()
    {
        savebuttonTr.GetChild(0).gameObject.SetActive(false);
        savebuttonTr.GetChild(1).gameObject.SetActive(true);
    }

    public void PointerUP()
    {
        savebuttonTr.GetChild(0).gameObject.SetActive(true);
        savebuttonTr.GetChild(1).gameObject.SetActive(false);
    }
}
