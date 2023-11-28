using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Boss boss;
    public Player player;

    [SerializeField]
    Transform mainCanvasTr;
    [SerializeField]
    Image playerHPBar;
    [SerializeField]
    Image RollCoolImg;
    float rollMaxCool = 3f;
    public InputManager inputManager;
    
    [SerializeField]
    Transform gameOverTr;
    Text deadTxt;
    Button reviveBut;
    Color overBackColor = Color.black;
    Color deadTxtColor = new Color(0.6f, 0f, 0f, 1f);
    Coroutine fadeCor = null;

    float[] enforceAtt = new float[11] { 10, 20, 40, 70, 110, 160, 220, 290, 370, 460, 600 };
    float[] enforceDef = new float[11] { 0, 2, 6, 12, 20, 30, 42, 56, 72, 90, 110 };
    float[] enforceHP = new float[11] { 100, 200, 330, 490, 680, 900, 1150, 1440, 1770, 2140, 3000 };

    private void Start()
    {
        player.gameObject.SetActive(true);
        deadTxt = gameOverTr.GetChild(0).GetComponent<Text>();
        reviveBut = gameOverTr.GetChild(1).GetComponent<Button>();
        overBackColor.a = 0f;
        gameOverTr.GetComponent<Image>().color = overBackColor;
        deadTxtColor.a = 0f;
        deadTxt.color = deadTxtColor;
        reviveBut.gameObject.SetActive(false);
        gameOverTr.gameObject.SetActive(false);
    }
    void Update()
    {
        RollCoolImg.fillAmount = player.rollCool / rollMaxCool;
    }
    void FixedUpdate()
    {
        playerHPBar.fillAmount = player.myStat.HP / player.myStat.MaxHP;
    }

    public void EnforceStat(int enforceNum)
    {
        AllEnum.EnforceType enforceType = (AllEnum.EnforceType)enforceNum;
        if (player.GetKillCount() >= 10)
        {
            switch (enforceType)
            {
                case AllEnum.EnforceType.Attack:
                    for (int i = 0; i < enforceAtt.Length; i++)
                    {
                        if (player.myStat.Att < enforceAtt[i] && i != 0)
                        {
                            player.myStat.Att = enforceAtt[i];
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 공격력을 강화 합니다.\n{enforceAtt[i - 1]} -> {enforceAtt[i]}");
                            player.RemoveKillCount();
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
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 방어력을 강화 합니다.\n{enforceDef[i - 1]} -> {enforceDef[i]}");
                            player.RemoveKillCount();
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
                            player.myStat.HP += enforceHP[i] - enforceHP[i - 1]; // 증가한 값 만큼 HP에 더하기
                            UIManager.Instance.PrintWarningMsg($"강화 개수를 사용하여 최대체력을 강화 합니다.\n{enforceHP[i - 1]} -> {enforceHP[i]}");
                            player.RemoveKillCount();
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
            UIManager.Instance.PrintWarningMsg("강화개수가 부족하여 강화할 수 없습니다.\nkill : 10 => 강화 개수");
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

    public void PrintDeadScreen()
    {
        if (fadeCor == null)
            fadeCor = StartCoroutine(FadeInDeadScreen());
    }

    public void ExitDeadScreen()
    {
        overBackColor.a = 0f;
        gameOverTr.GetComponent<Image>().color = overBackColor;
        deadTxtColor.a = 0f;
        deadTxt.color = deadTxtColor;
        reviveBut.gameObject.SetActive(false);
        gameOverTr.gameObject.SetActive(false);
    }

    IEnumerator FadeInDeadScreen()
    {
        yield return new WaitForSeconds(2f);
        gameOverTr.gameObject.SetActive(true);
        while (overBackColor.a < 1f)
        {
            overBackColor.a = Mathf.Clamp(overBackColor.a + Time.fixedDeltaTime, 0, 1);
            gameOverTr.GetComponent<Image>().color = overBackColor;
            yield return new WaitForFixedUpdate();
        }
        if (overBackColor.a == 1f)
        {
            while (deadTxtColor.a < 1f)
            {
                deadTxtColor.a = Mathf.Clamp(deadTxtColor.a + Time.fixedDeltaTime, 0, 1);
                deadTxt.color = deadTxtColor;
                yield return new WaitForFixedUpdate();
            }
        }
        if (deadTxtColor.a == 1f)
        {
            yield return new WaitForSeconds(1f);
            reviveBut.gameObject.SetActive(true);
        }

        if (fadeCor != null)
        {
            StopCoroutine(fadeCor);
            fadeCor = null;
        }
    }
}
