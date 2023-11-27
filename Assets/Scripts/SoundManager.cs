using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    AudioClip[] allSoundSource_BGM;
    [SerializeField]
    AudioClip[] allSoundSource_Effect; 
    // 0.�ذ�_Hit_00, 1.�ذ�_Hit_01, 2. �ذ�_Die_00, 3.�ذ�_Swing_00, 4.�÷��̾�_Hit, 5.�÷��̾�_Run, 6.�÷��̾�_Die, 7.�÷��̾�_Roll, 8.�÷��̾�_Landing
    // 9.�÷��̾�_Heal, 10.�÷��̾�_Swing, 11.����_Attack

    [SerializeField]
    AudioSource BGM;
    [SerializeField]
    AudioSource[] Effects;
    public AudioSource GetEffects(int effectNum)
    {
        return Effects[effectNum];
    }

    [SerializeField]
    Slider sliderBGM;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = GameManager.Instance.player.transform.position;
    }

    public void SetSoundBGM(int songNum)
    {
        BGM.clip = allSoundSource_BGM[songNum];
        BGM.Play();
    }
    public void VolumeBGM()
    {
        BGM.volume = sliderBGM.value;
    }

    public void SetPlayerSound(/*bool action, */int soundNum, Vector2 pos)
    {
        Effects[2].transform.position = pos;
        Effects[2].PlayOneShot(allSoundSource_Effect[soundNum]);
    }

    public void SetSoundEffect(int soundNum, Vector2 pos)
    {
        if (Effects[0].isPlaying)
        {
            Effects[1].transform.position = pos;
            Effects[1].PlayOneShot(allSoundSource_Effect[soundNum]);
        }
        else
        {
            Effects[0].transform.position = pos;
            Effects[0].PlayOneShot(allSoundSource_Effect[soundNum]);
        }
    }
}
