using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
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

    void Update()
    {
        AttTxt.text = "Att : " + string.Format("{0:0000}", player.myStat.Att);
        DefTxt.text = "Def : " + string.Format("{0:0000}", player.myStat.Def);
        HPTxt.text = "H P : " + string.Format("{0:0000}", player.myStat.MaxHP);
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
