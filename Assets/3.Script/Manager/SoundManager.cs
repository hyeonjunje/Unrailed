using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    #region Singleton

    public static SoundManager Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;
    public AudioSource audioSourdEngine;

    public Sound[] soundsEffect;
    public Sound[] soundsBgm;
    public string[] playSoundName;
    public bool isPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length + audioSourceBGM.Length];
    }
    public void PlaySoundEffect(string _Name)
    {
        //해당하는 사운드를 playSoundEffect를 불러 넣기
        for (int i = 0; i < soundsEffect.Length; i++)
        {
            if (_Name == soundsEffect[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = soundsEffect[i].name;

                        audioSourceEffects[j].clip = soundsEffect[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 Audio Source가 사용 중입니다.");
                return;
            }
        }
        Debug.Log(_Name + "사운드가 SoundManager에 등록되지 않았습니다");
    }
    public void PlaySoundBgm(string _Name)
    {
        for (int i = 0; i < soundsBgm.Length; i++)
        {
            if (_Name == soundsBgm[i].name)
            {
                for (int j = 0; j < audioSourceBGM.Length; j++)
                {
                    if (!audioSourceBGM[j].isPlaying)
                    {
                        playSoundName[j] = soundsBgm[i].name;

                        audioSourceBGM[j].clip = soundsBgm[i].clip;
                        audioSourceBGM[j].Play();
                        return;
                    }
                }
                return;
            }
        }
    }
    public void StopAllSound()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
        }
    }
    // Update is called once per frame
    public void StopSoundEffect(string _Name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(playSoundName[i] == _Name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
    }
    public void StopSoundBgm(string _Name)
    {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            if (playSoundName[i] == _Name)
            {
                audioSourceBGM[i].Stop();
                return;
            }
        }
    }
}
