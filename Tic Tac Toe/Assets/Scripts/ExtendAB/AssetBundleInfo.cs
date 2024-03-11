using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetBundleInfo
{
    /// <summary>
    /// AB����
    /// </summary>
    public string bundleName;
    /// <summary>
    /// ���ü���
    /// </summary>
    public uint refCount;
    /// <summary>
    /// AB��
    /// </summary>
    public AssetBundle assetBundle;


    private Dictionary<string, Object> resources;

    private Dictionary<string, int> assetRef;

    /// <summary>
    /// ���캯��������AB����Ϣ�࣬�������Ż�Ϊ�����
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
    /// ReSet���������ں�����������
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
    /// ͬ��ж��AB������
    /// </summary>
    /// <param name="unload">���Ϊfalse�Ļ�ֻ��ж�ذ�������ж��ʵ��������Դ</param>
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
    /// �첽ж��AB������
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
    /// ж����Դ
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
    /// ���AB�����Ƿ��и���Դ
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public bool HasAsset(string assetName)
    {
        return assetBundle.Contains(assetName);
    }

    /// <summary>
    /// ͬ�����ذ�����Դ
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
    /// �첽�����ļ�
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
