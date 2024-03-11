using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�ģ��
/// ֪ʶ��
/// 1.�����첽����
/// 2.Э��
/// 3.ί��
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    
    /// <summary>
    /// �л����� ͬ������
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction action)
    {
        //����ͬ������
        SceneManager.LoadScene(name);
        //������ɹ��� �Ż�ȥִ��
        action();
    }

    /// <summary>
    /// �ṩ���ⲿ�� �첽���صĽӿڷ���
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void LoadSceneAsyn(string name, UnityAction action)
    {
        //�����첽����
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name,action));
       
    }


    /// <summary>
    /// ʹ��UniTask�첽���س���
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="callBack"></param>
    public async void LoadSceneByUniTask(string name,UnityAction callBack = null)
    {
        await SceneManager.LoadSceneAsync(name).ToUniTask(
            (Progress.Create<float>((p) =>
           {
               Debug.Log("���س�����");
               EventCenter.Instance.EventTrigger<float>("Loading", p);
           }))
            );
        if(callBack != null)
        {
            callBack();
        }   
    }

    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name,UnityAction action)
    {

        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //���Եõ��������صĽ���
        while (!ao.isDone)
        {
            //ϣ����������ȥ���½�����
            //�¼����� ����ַ� ������� �������þ���
            EventCenter.Instance.EventTrigger("Loading", ao.progress);
            yield return ao.progress;
        }
        //������ɹ��� ִ�з���
        action();
    }


}
