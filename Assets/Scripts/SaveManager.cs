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
            UIManager.Instance.SetLife(PlayerPrefs.GetInt("Life"));

            player.transform.position = Vector3.zero;
            UIManager.Instance.UI_MenuBotton();
        }
    }
    
    public void SaveVolumeInfo(float vol, bool isBGM) // BGM이면 true, SFX면 false
    {
        if (isBGM == false)
            PlayerPrefs.SetFloat("volBGM", vol);
        else
            PlayerPrefs.SetFloat("volSFX", vol);
    }

    public float LoadVolumeInfo(bool isBGM)
    {
        if (isBGM)
            return PlayerPrefs.GetFloat("volBGM");
        else
            return PlayerPrefs.GetFloat("volSFX");
    }
}
