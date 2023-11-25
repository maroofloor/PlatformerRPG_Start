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
    Image RollCoolImg;
    float rollMaxCool = 3f;
    public InputManager inputManager;

    float[] enforceAtt = new float[11] { 10, 20, 40, 70, 110, 160, 220, 290, 370, 460, 600 };
    float[] enforceDef = new float[11] { 0, 2, 6, 12, 20, 30, 42, 56, 72, 90, 550 };
    float[] enforceHP = new float[11] { 100, 200, 330, 490, 680, 900, 1150, 1440, 1770, 2140, 3000 };

    private void Start()
    {
        player.gameObject.SetActive(true);
    }
    void Update()
    {
        RollCoolImg.fillAmount = player.rollCool / rollMaxCool;
    }
    void FixedUpdate()
    {
        playerHPBar.fillAmount = player.myStat.HP / player.myStat.MaxHP;
    }

    public void SAVEButtonClicked()
    {
    }

    public void EnforceStat(int enforceNum)
    {
        AllEnum.EnforceType enforceType = (AllEnum.EnforceType)enforceNum;
        if (player.killCount >= 10)
        {
            switch (enforceType)
            {
                case AllEnum.EnforceType.Attack:
                    for (int i = 0; i < enforceAtt.Length; i++)
                    {
                        if (player.myStat.Att < enforceAtt[i] && i != 0)
                        {
                            player.myStat.Att = enforceAtt[i];
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� ���ݷ��� ��ȭ �մϴ�.\n{enforceAtt[i - 1]} -> {enforceAtt[i]}");
                            player.killCount -= 10;
                            UIManager.Instance.killCountUpdate();
                            UIManager.Instance.EnforceNumUpdate();
                            return;
                        }
                    }
                    break;
                case AllEnum.EnforceType.Defense:
                    for (int i = 0; i < enforceDef.Length; i++)
                    {
                        if (player.myStat.Def < enforceDef[i] && i != 0)
                        {
                            player.myStat.Def = enforceDef[i];
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� ������ ��ȭ �մϴ�.\n{enforceDef[i - 1]} -> {enforceDef[i]}");
                            player.killCount -= 10;
                            UIManager.Instance.killCountUpdate();
                            UIManager.Instance.EnforceNumUpdate();
                            return;
                        }
                    }
                    break;
                case AllEnum.EnforceType.MaxHP:
                    for (int i = 0; i < enforceHP.Length; i++)
                    {
                        if (player.myStat.MaxHP < enforceHP[i] && i != 0)
                        {
                            player.myStat.MaxHP = enforceHP[i];
                            player.myStat.HP += enforceHP[i] - enforceHP[i - 1]; // ������ �� ��ŭ HP�� ���ϱ�
                            UIManager.Instance.PrintWarningMsg($"��ȭ ������ ����Ͽ� �ִ�ü���� ��ȭ �մϴ�.\n{enforceHP[i - 1]} -> {enforceHP[i]}");
                            player.killCount -= 10;
                            UIManager.Instance.killCountUpdate();
                            UIManager.Instance.EnforceNumUpdate();
                            return;
                        }
                    }
                    break;
            }
        }
        else
        {
            UIManager.Instance.PrintWarningMsg("��ȭ������ �����Ͽ� ��ȭ�� �� �����ϴ�.\nkill : 10 => ��ȭ ����");
        }
    }

    public float GetEneforceInfo(AllEnum.EnforceType enforceType, int num)
    {
        float value;
        switch (enforceType)
        {
            case AllEnum.EnforceType.Attack:
                value = enforceAtt[num];
                break;
            case AllEnum.EnforceType.Defense:
                value = enforceDef[num];
                break;
            case AllEnum.EnforceType.MaxHP:
                value = enforceHP[num];
                break;
            default:
                return -1;
        }
        return value;
    }
}
