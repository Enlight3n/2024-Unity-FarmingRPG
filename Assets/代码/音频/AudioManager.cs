using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    
    [SerializeField] private GameObject soundPrefab = null;

    //环境声和音乐的声源
    [Header("Audio Sources")] 
    [SerializeField] private AudioSource ambientSoundAudioSource = null;
    [SerializeField] private AudioSource gameMusicAudioSource = null;

    
    [Header("Audio Mixers")] 
    [SerializeField] private AudioMixer gameAudioMixer = null;

    
    [Header("Audio Snapshots")] 
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null;

    
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null;

    [Header("Other")]
    [SerializeField] private SO_SoundList so_soundList = null;
    
    
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;
    
    private Dictionary<SoundName, SoundItem> soundDictionary;

    private Coroutine playSceneSoundsCoroutine;

    protected override void Awake()
    {
        base.Awake();
        
        //初始化
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        //从数据容器中加载，保存到soundDictionary
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }
    }

    

    #region 场景音乐和环境音
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSounds;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSounds;
    }
    
    
    //环境音和背景音乐无时无刻都有，因此写在AudioManager中，AfterSceneLoadEvent事件中调用
    //我们对每个场景，使用一套环境音+背景音乐
    private void PlaySceneSounds()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        //获取当前场景
        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, 
                true, out SceneName currentSceneName))
        {
            if (currentSceneName == SceneName.室内)
            {
                soundDictionary.TryGetValue(SoundName.BGM1, out musicSoundItem);
                soundDictionary.TryGetValue(SoundName.室内噪声, out ambientSoundItem);
            }

            else
            {
                soundDictionary.TryGetValue(SoundName.BGM2, out musicSoundItem);
                soundDictionary.TryGetValue(SoundName.野外噪声1, out ambientSoundItem);
            }
        }

        //停止上个场景的协程
        if (playSceneSoundsCoroutine != null)
        {
            StopCoroutine(playSceneSoundsCoroutine);
        }
        
        //开启新协程
        playSceneSoundsCoroutine =
            StartCoroutine(PlaySceneSoundsRoutine(120f, musicSoundItem, ambientSoundItem));
    }


    
    private IEnumerator PlaySceneSoundsRoutine(float musicPlaySeconds, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {

        if (musicSoundItem != null && ambientSoundItem != null)
        {
            //以环境音开始
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            //等待随机时间，然后播放BGM
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, 
                sceneMusicStartMaxSecs));

            //播放BGM
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            //等待音乐播放完毕，然后播放环境音
            yield return new WaitForSeconds(musicPlaySeconds);

            //播放环境音
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    
    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        //设置音量
        gameAudioMixer.SetFloat("MusicVolume", 
            ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        //设置音频片段并播放 
        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        //过渡到音乐snapshot
        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    
    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)  
    {  
        //设置音量  
        gameAudioMixer.SetFloat("AmbientVolume", 
            ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));  
  
        //设置音频片段并播放  
        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;  
        ambientSoundAudioSource.Play();  
  
        //过渡到环境snapshot
        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);  
    }

    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        return (volumeDecimalFraction * 100f - 80f);
    }
    
    #endregion

    
    #region 播放音效
    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            //从对象池里取出来soundPrefab
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            //获取sound脚本
            Sound sound = soundGameObject.GetComponent<Sound>();

            //设置播放的音效
            sound.SetSound(soundItem);
            
            //SetActive(true)，会触发MonoBehaviour.OnEnable()事件，就算对象之前本就是activeSelf==true，事件依然会发生； 
            //SetActive(false)，会触发MonoBehaviour.OnDisable()事件,就算对象之前本就是activeSelf==false，事件依然会发生；
            soundGameObject.SetActive(true);
            
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));
        }
    }

    
    //将0到1转换为-80到20
    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        
        soundGameObject.SetActive(false);
    }
    #endregion
    
}