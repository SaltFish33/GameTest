using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    //Ψһ�ı����������
    private AudioSource bkMusic = null;

    //���ֺ���Ч��������С
    private float bkValue = 1;
    private float soundValue = 1;
    //��Ч��������
    private GameObject soundObj = null;
    //������Ч����Ч�б�
    private List<AudioSource> soundList = new List<AudioSource>();


    /// <summary>
    /// ��Update�м���ѭ������Ч�Ƿ񲥷����
    /// </summary>
    public MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);

            }
        }
    }


    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="name"></param>
    public void PlayBKMusic(string name)
    {
        if(bkMusic == null)
        {
            GameObject obj = new GameObject("BKMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //�첽���ر������� ������ɺ� ����
        ResMgr.Instance.LoadAsync<AudioClip>("Music/BK/" + name,(clip)=> {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });

    }

    /// <summary>
    /// �ı䱳������������С
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }



    /// <summary>
    /// ��ͣ��������
    /// </summary>
    public void PasueBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }


    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    public void PladySound(string name, bool isLoop,UnityAction<AudioSource> callBack = null)
    {
        if(soundObj == null)
        {
            soundObj = new GameObject("Sound");
        }
        //����Դ�첽������� �������Ч
        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/"+name,(clip)=> {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
            if (callBack != null)
                callBack(source);
        });

    }

    /// <summary>
    /// �ı���Ч��С
    /// </summary>
    /// <param name="value"></param>
    public void ChangedSoundValue(float value)
    {
        soundValue = value;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundValue;
        }
    }




    /// <summary>
    /// ֹͣ��Ч
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }


}
