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
    // 0.�ذ�_Hit_, 1.�ذ�_Hit_01, 2. �ذ�_Die, 3.�ذ�_Swing, 4.�÷��̾�_Hit, 5.�÷��̾�_Run, 6.�÷��̾�_Die, 7.�÷��̾�_Roll, 8.�÷��̾�_Landing
    // 9.�÷��̾�_Heal, 10.�÷��̾�_Swing, 11.����_Attack, 12~14. ����_Hit, 15.����_Skill, 16.�÷��̾�_Skill, 17.����_Die, 18.����_Walk

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
