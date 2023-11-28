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
    // 0.해골_Hit_, 1.해골_Hit_01, 2. 해골_Die, 3.해골_Swing, 4.플레이어_Hit, 5.플레이어_Run, 6.플레이어_Die, 7.플레이어_Roll, 8.플레이어_Landing
    // 9.플레이어_Heal, 10.플레이어_Swing, 11.보스_Attack, 12~14. 보스_Hit, 15.보스_Skill, 16.플레이어_Skill, 17.보스_Die, 18.보스_Walk

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
    [SerializeField]
    Slider sliderSFX;

    private void Start()
    {
        BGM.volume = SaveManager.Instance.LoadVolumeInfo(true);
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i].volume = SaveManager.Instance.LoadVolumeInfo(false);
        }
        sliderBGM.value = BGM.volume;
        sliderSFX.value = Effects[0].volume;
    }

    void Update()
    {
        transform.position = GameManager.Instance.player.transform.position;
    }

    public void SetSoundBGM(int songNum)
    {
        if (BGM.isPlaying)
            BGM.Stop();
        BGM.clip = allSoundSource_BGM[songNum];
        BGM.Play();
    }

    public void VolumeBGM()
    {
        BGM.volume = sliderBGM.value;
        SaveManager.Instance.SaveVolumeInfo(BGM.volume, true);
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

    public void VolumeSFX()
    {
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i].volume = sliderSFX.value;
        }
        SaveManager.Instance.SaveVolumeInfo(Effects[0].volume, false);
    }
}
