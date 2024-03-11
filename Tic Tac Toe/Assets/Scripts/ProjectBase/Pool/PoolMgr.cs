
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据 池子中的一列容器
/// </summary>
public class GameObjectPoolData
{
    //一个抽屉 有他的标签
    //和它所装的所有物体
    public GameObject fatherObj;

    public List<GameObject> poolList;

    public GameObjectPoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象
        //并且把它作为我们pool对象的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() { obj};
        PushObj(obj);
    }

    /// <summary>
    /// 往抽屉里面压东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj( GameObject obj)
    {
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
        obj.SetActive(false);
    }

    /// <summary>
    /// 从抽屉里面取东西
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
/// 缓存池模块
/// 1.Dictionary List
/// 2.GameObject  和 Resources 两个公共类中的API
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //缓存池容器 字典
    private Dictionary<string, GameObjectPoolData> gameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();
    private Dictionary<string, ObjectData> objectPoolDic = new Dictionary<string, ObjectData>();

    public GameObject poolObj;


    //外部从池子里借东西
    /// <summary>
    /// 外部从池子里借东西
    /// name就为要接的东西的预制体路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetGameObj(string name,UnityAction<GameObject> callBack = null)
    {
        GameObject gameObj = null;
        //判断池子中是否有
        //有抽屉 并且抽屉有东西 就能拿
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

    //还给我暂时不用的东西
    public void PushGameObj(string name, GameObject obj)
    {
        if(poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }
       
        //里面有抽屉
        if (gameObjectPoolDic.ContainsKey(name))
        {
            gameObjectPoolDic[name].PushObj(obj);
        }
        //里面没有抽屉
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
    /// 删除全部
    /// </summary>
    /// <param name="clearGameObject">是否删除游戏物体</param>
    /// <param name="clearCObject">是否删除普通C#对象</param>
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
