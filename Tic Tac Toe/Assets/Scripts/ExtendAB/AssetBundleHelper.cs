
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 后续需根据Yooasset进行修改，依赖关系改为针对具体文件构建，引用计数需更改
/// </summary>
public static class AssetBundleHelper 
{
    private static readonly Dictionary<string, AssetBundleInfo> AssetBundles = new Dictionary<string, AssetBundleInfo>();
    private static readonly Dictionary<string, string[]> DependencyDic = new Dictionary<string, string[]>();
    private static AssetBundleManifest Manifest;
    private static string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// Init方法
    /// </summary>
    public static void Init()
    {
        Reset();
        AssetBundles.Clear();
        DependencyDic.Clear();
        CollectDependency();
    }

    public static async UniTask<Object> LoadAsset(string assetName, string bundleName, Type type) 
    {
        if(AssetBundles.TryGetValue(bundleName, out var ab))
        {
            if (ab.HasAsset(assetName))
            {
                return ab.LoadAsset(assetName, type);
            }
            else
            {
                Debug.Log($"资源不存在,资源名为{assetName}");
                return null;
            }
        }
        else
        {
            //TODO:加载AB包
            await LoadAssetBundle(bundleName);
            if(AssetBundles.TryGetValue(bundleName, out var abInfo))
            {
                return abInfo.LoadAsset(assetName, type);
            }
            else
            {
                Debug.LogError($"资源不存在,资源名为{assetName}");
            }
            return null;
        }
    }

    public static async UniTask LoadAssetBundle(string bundleName, bool loadAsRef = false)
    {
        if(DependencyDic.TryGetValue(bundleName, out var strs))
        {
            foreach (var abName in strs)
            {
                if (AssetBundles.ContainsKey(abName))
                {
                    AssetBundles[abName].refCount++;
                    continue;
                }else
                    await LoadAssetBundle(abName, true);
            }
        }
        var path = GetAssetBundlePath(bundleName);
        if (path == null)
        {
            Debug.LogError($"错误，不存在AB包{bundleName}");
            return;
        }
        AssetBundleLoader loader = new AssetBundleLoader(path, bundleName);
        await loader.LoadAssetBundleAsync();
        AssetBundles.TryAdd(bundleName, loader.RefAssetBundle);
    }

    public static void CollectDependency()
    {
        if(Manifest == null)
            GetManiFest();
        var abName = Manifest.GetAllAssetBundles();
        foreach (var name in abName)
        {
            var dependency = Manifest.GetAllDependencies(name);
            DependencyDic.TryAdd(name, dependency);
        }
        Debug.Log("依赖关系构建完毕");
    } 

    private static void GetManiFest()
    {
        var maniFestPath = GetAssetBundlePath(MainABName);
        if(maniFestPath == null)
        {
            Debug.LogError($"错误，无法找到主包依赖，请确定在流文件夹或可读写文件夹中包含有依赖文件");
            return;
        }
        var maniAB = AssetBundle.LoadFromFile(maniFestPath);
        Manifest = maniAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        Debug.Log("主包获取成功");
        return ;
    }

    private static string GetAssetBundlePath(string name)
    {
        var path = Define.PersistentDataPath + "/" + MainABName + "/" + name;
        if (!File.Exists(path))
        {
            path = Define.StreamingAssetPath  + "/" + name;
            if (!File.Exists(path))
            {
                Debug.LogError($"错误，AB包路径错误{path}");
                return null;
            }
        }
        return path;
    }

    public static void UnLoadAssetBundle(string bundleName, bool unload = false)
    {
        if (AssetBundles.TryGetValue(bundleName, out var abInfo))
        {
            if (abInfo.Destroy())
            {
                foreach (var ab in DependencyDic[bundleName])
                {
                    AssetBundles[ab].Destroy(unload);
                }
                AssetBundles.Remove(bundleName);
            }
        }
        else
        {
            Debug.LogError($"AB包还未加载,包名为{bundleName}");
        }
    }

    public static void UnLoadAsset(string bundleName, string assetName, bool autoDispose = false)
    {
        if (AssetBundles.TryGetValue(bundleName, out var abInfo))
        {
            if (abInfo.HasAsset(assetName))
                abInfo.UnloadAsset(assetName, autoDispose);
        }
    }

    public static void Reset()
    {
        var removeList = new List<string>();
        foreach (var ab in AssetBundles)
        {
            if (ab.Value.Destroy())
                removeList.Add(ab.Key);
        }
        foreach (var name in removeList)
        {
            AssetBundles.Remove(name);
        }
        Resources.UnloadUnusedAssets();
    }




}
