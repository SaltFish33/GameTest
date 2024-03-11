
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �������� �����е�һ������
/// </summary>
public class GameObjectPoolData
{
    //һ������ �����ı�ǩ
    //������װ����������
    public GameObject fatherObj;

    public List<GameObject> poolList;

    public GameObjectPoolData(GameObject obj, GameObject poolObj)
    {
        //�����ǵĳ��� ����һ��������
        //���Ұ�����Ϊ����pool�����������
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() { obj};
        PushObj(obj);
    }

    /// <summary>
    /// ����������ѹ����
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj( GameObject obj)
    {
        //������
        poolList.Add(obj);
        //���ø�����
        obj.transform.parent = fatherObj.transform;
        obj.SetActive(false);
    }

    /// <summary>
    /// �ӳ�������ȡ����
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        int count = poolList.Count - 1;
        obj = poolList[count];
        obj.SetActive(true);
        poolList.RemoveAt(count);
        obj.transform.parent = null;
        return obj;
    }

}



public class ObjectData
{
    public Queue<object> objQueue;
    public ObjectData(System.Object obj)
    {
        objQueue = new Queue<object>();
    }

    public void PushObj(System.Object obj)
    {
        objQueue.Enqueue(obj);
    }

    public System.Object GetObj()
    {
        return (System.Object)objQueue.Dequeue();
    }
}


/// <summary>
/// �����ģ��
/// 1.Dictionary List
/// 2.GameObject  �� Resources �����������е�API
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //��������� �ֵ�
    private Dictionary<string, GameObjectPoolData> gameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();
    private Dictionary<string, ObjectData> objectPoolDic = new Dictionary<string, ObjectData>();

    public GameObject poolObj;


    //�ⲿ�ӳ�����趫��
    /// <summary>
    /// �ⲿ�ӳ�����趫��
    /// name��ΪҪ�ӵĶ�����Ԥ����·��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetGameObj(string name,UnityAction<GameObject> callBack = null)
    {
        GameObject gameObj = null;
        //�жϳ������Ƿ���
        //�г��� ���ҳ����ж��� ������
        if(gameObjectPoolDic.ContainsKey(name) && gameObjectPoolDic[name].poolList.Count != 0)
        {
            gameObj = gameObjectPoolDic[name].GetObj();
            callBack?.Invoke(gameObj);
            return gameObj;
        }
        else
        {
            //gameObj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //ResMgr.Instance.LoadAsync<GameObject>(name, (obj) =>
            //{
            //    obj.name = name;
            //    callBack?.Invoke(obj);
            //    gameObj = obj;
            //});

            gameObj = ResMgr.Instance.Load<GameObject>(name);
            callBack?.Invoke(gameObj);
            return gameObj;
        }
       
    }

    //��������ʱ���õĶ���
    public void PushGameObj(string name, GameObject obj)
    {
        if(poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }
       
        //�����г���
        if (gameObjectPoolDic.ContainsKey(name))
        {
            gameObjectPoolDic[name].PushObj(obj);
        }
        //����û�г���
        else
        {
            gameObjectPoolDic.Add(name, new GameObjectPoolData(obj,poolObj));
        }
    }


    public T GetNormalObject<T>() where T:class, new()
    {
        if (CheckCanGetObject<T>())
        {
            return objectPoolDic[typeof(T).FullName].GetObj() as T;
        }
        else
        {
            return new T();
        }
    }

    public void PushObjectBack(System.Object obj)
    {
        string name = obj.GetType().FullName;
        if (objectPoolDic.ContainsKey(name))
            objectPoolDic[name].PushObj(obj);
        else
            objectPoolDic.Add(name, new ObjectData(obj));
    }





    public bool CheckCanGetObject<T>() where T : class, new()
    {
        if (objectPoolDic.ContainsKey(typeof(T).FullName) && objectPoolDic[typeof(T).FullName].objQueue.Count > 0) return true;
        return false;
    }





    /// <summary>
    /// ɾ��ȫ��
    /// </summary>
    /// <param name="clearGameObject">�Ƿ�ɾ����Ϸ����</param>
    /// <param name="clearCObject">�Ƿ�ɾ����ͨC#����</param>
    public void Clear(bool clearGameObject = true, bool clearCObject = true)
    {
        if (clearGameObject)
        {
            for (int i = 0; i < poolObj.transform.childCount; i++)
            {
                Object.Destroy(poolObj.transform.GetChild(i).gameObject);
            }
            gameObjectPoolDic.Clear();
        }

        if (clearCObject)
        {
            objectPoolDic.Clear();
        }
    }

    public void ClearAllGameObject()
    {
        Clear(true, false);
    }
    public void ClearGameObject(string prefabName)
    {
        GameObject go = poolObj.transform.Find(prefabName).gameObject;
        if (go != null)
        {
            UnityEngine.Object.Destroy(go);
            gameObjectPoolDic.Remove(prefabName);

        }

    }
    public void ClearGameObject(GameObject prefab)
    {
        ClearGameObject(prefab.name);
    }

    public void ClearAllObject()
    {
        Clear(false, true);
    }
    public void ClearObject<T>()
    {
        objectPoolDic.Remove(typeof(T).FullName);
    }
    public void ClearObject(System.Type type)
    {
        objectPoolDic.Remove(type.FullName);
    }


}
