
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// ���������Yooasset�����޸ģ�������ϵ��Ϊ��Ծ����ļ����������ü��������
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
    /// Init����
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
                Debug.Log($"��Դ������,��Դ��Ϊ{assetName}");
                return null;
            }
        }
        else
        {
            //TODO:����AB��
            await LoadAssetBundle(bundleName);
            if(AssetBundles.TryGetValue(bundleName, out var abInfo))
            {
                return abInfo.LoadAsset(assetName, type);
            }
            else
            {
                Debug.LogError($"��Դ������,��Դ��Ϊ{assetName}");
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
            Debug.LogError($"���󣬲�����AB��{bundleName}");
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
        Debug.Log("������ϵ�������");
    } 

    private static void GetManiFest()
    {
        var maniFestPath = GetAssetBundlePath(MainABName);
        if(maniFestPath == null)
        {
            Debug.LogError($"�����޷��ҵ�������������ȷ�������ļ��л�ɶ�д�ļ����а����������ļ�");
            return;
        }
        var maniAB = AssetBundle.LoadFromFile(maniFestPath);
        Manifest = maniAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        Debug.Log("������ȡ�ɹ�");
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
                Debug.LogError($"����AB��·������{path}");
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
            Debug.LogError($"AB����δ����,����Ϊ{bundleName}");
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
