using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField]
    Player player;

    public void SavePlayer()
    {
        PlayerPrefs.SetFloat("StatHP", player.myStat.HP);
        PlayerPrefs.SetFloat("StatMaxHP", player.myStat.MaxHP);
        PlayerPrefs.SetFloat("StatDef", player.myStat.Def);
        PlayerPrefs.SetFloat("StatAtt", player.myStat.Att);
        PlayerPrefs.SetInt("Life", player.GetLife());
        PlayerPrefs.SetInt("PotionNum", player.GetPotionNum());
        PlayerPrefs.SetInt("KillCount", player.GetKillCount());

        UIManager.Instance.UI_MenuBotton();
    }

    public void LoadPlayer()
    {
        if (PlayerPrefs.HasKey("StatHP") == false)
        {
            UIManager.Instance.PrintWarningMsg("저장된 데이터가 없습니다.");
            return;
        }
        else
        {
            player.myStat.HP = PlayerPrefs.GetFloat("StatHP");
            player.myStat.MaxHP = PlayerPrefs.GetFloat("StatMaxHP");
            player.myStat.Def = PlayerPrefs.GetFloat("StatDef");
            player.myStat.Att = PlayerPrefs.GetFloat("StatAtt");
            player.SetLife(PlayerPrefs.GetInt("Life"));
            player.SetPotionNum(PlayerPrefs.GetInt("PotionNum"));
            player.SetKillCount(PlayerPrefs.GetInt("KillCount"));

            UIManager.Instance.PotionNumUpdate();
            UIManager.Instance.killCountUpdate();
            UIManager.Instance.PlusLife(PlayerPrefs.GetInt("Life"));

            player.transform.position = Vector3.zero;
            UIManager.Instance.UI_MenuBotton();
        }
    }
}
