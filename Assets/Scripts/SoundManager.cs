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
    // 0.해골_Hit_00, 1.해골_Hit_01, 2. 해골_Die_00, 3.해골_Swing_00, 4.플레이어_Hit, 5.플레이어_Run, 6.플레이어_Die, 7.플레이어_Roll, 8.플레이어_Landing
    // 9.플레이어_Heal, 10.플레이어_Swing, 11.보스_Attack

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
