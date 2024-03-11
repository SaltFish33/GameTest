using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetBundleInfo
{
    /// <summary>
    /// AB包名
    /// </summary>
    public string bundleName;
    /// <summary>
    /// 引用计数
    /// </summary>
    public uint refCount;
    /// <summary>
    /// AB包
    /// </summary>
    public AssetBundle assetBundle;


    private Dictionary<string, Object> resources;

    private Dictionary<string, int> assetRef;

    /// <summary>
    /// 构造函数，创建AB包信息类，后续可优化为对象池
    /// </summary>
    /// <param name="name"></param>
    /// <param name="assetBundle"></param>
    public AssetBundleInfo()
    {
       
    }

    public void Init(string name, AssetBundle assetBundle = null, bool loadAsRef = false)
    {
        bundleName = name;
        refCount = 0;
        this.assetBundle = assetBundle;
        resources = new Dictionary<string, Object>();
        assetRef = new Dictionary<string, int>();
        if (loadAsRef) ++refCount;
    }

    /// <summary>
    /// ReSet方法，用于后续加入对象池
    /// </summary>
    private void Dispose()
    {
        bundleName = null;
        refCount = 0;
        assetBundle = null;
        resources = null;
        assetRef = null;
    }

    /// <summary>
    /// 同步卸载AB包方法
    /// </summary>
    /// <param name="unload">如果为false的话只会卸载包，不会卸载实例化的资源</param>
    public bool Destroy(bool unload = false)
    {
        --refCount;
        if (refCount > 0)
            return false;
        if (assetBundle != null)
            assetBundle.Unload(unload);
        Dispose();
        return true;
    }

    /// <summary>
    /// 异步卸载AB包方法
    /// </summary>
    /// <param name="unload"></param>
    /// <returns></returns>
    public async UniTask<bool> DestroyAsync(bool unload = false)
    {
        --refCount;
        if (refCount > 0)
            return false;
        if (assetBundle != null)
        {
            var ab = assetBundle;
             await ab.UnloadAsync(unload);
        }
        Dispose();
        return true;
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset"></param>
    public void UnloadAsset(string assetName, bool disposeAll = false)
    {
        if (resources.ContainsKey(assetName) && assetRef.ContainsKey(assetName))
        {
            if (--assetRef[assetName] > 0)
                return;
            var asset = resources[assetName];
            resources.Remove(assetName);
            assetRef.Remove(assetName);
            Resources.UnloadAsset(asset);
            Destroy(disposeAll);
            //if (assetRef.Count == 0 && autoDispose)
            //    Destroy(true);
        }
    }

    /// <summary>
    /// 检查AB包内是否含有该资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public bool HasAsset(string assetName)
    {
        return assetBundle.Contains(assetName);
    }

    /// <summary>
    /// 同步加载包内资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="loadType"></param>
    /// <returns></returns>
    public Object LoadAsset(string assetName, Type loadType)
    {
        if (resources.TryGetValue(assetName, out var asset))
        {
            ++assetRef[assetName];
            return asset;
        }
        asset = assetBundle.LoadAsset(assetName, loadType);
        resources.TryAdd(assetName, asset);
        assetRef.TryAdd(assetName, 1);
        return asset;
    }

    /// <summary>
    /// 异步加载文件
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="loadType"></param>
    /// <returns></returns>
    public async UniTask<Object> LoadAssetAsync(string assetName, Type loadType)
    {
        if (resources.TryGetValue(assetName, out var asset))
        {
            ++assetRef[assetName];
            return asset;
        }
        var result = assetBundle.LoadAssetAsync(assetName, loadType);
        await result;
        resources.TryAdd(assetName, result.asset);
        assetRef.TryAdd(assetName, 1);
        return result.asset;
    }


}
