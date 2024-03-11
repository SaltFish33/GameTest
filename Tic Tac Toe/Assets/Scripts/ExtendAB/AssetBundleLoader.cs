using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class AssetBundleLoader
{
    private string AssetBundlePath;

    public AssetBundleInfo RefAssetBundle;

    private string AssetBundleName;

    public AssetBundleLoader(string path, string assetBundleName)
    {
        AssetBundlePath = path;
        AssetBundleName = assetBundleName;
    }

    public async UniTask LoadAssetBundleAsync(bool loadAsRef = false)
    {
        //UnLoadAssetBundle();
        var async = UnityWebRequestAssetBundle.GetAssetBundle(AssetBundlePath).SendWebRequest();
        await async;
        var request = async.webRequest;
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"����,AB��·��������,Path:{AssetBundlePath}, ErrorCode:{request.error}");
            return;
        }
        RefAssetBundle = new AssetBundleInfo();
        RefAssetBundle.Init(AssetBundleName, DownloadHandlerAssetBundle.GetContent(request), loadAsRef);
    }

    //public void UnLoadAssetBundle()
    //{
    //    if (RefAssetBundle != null)
    //        RefAssetBundle.Destroy();
    //}

    //public bool HasAsset(string assetName)
    //{
    //    return RefAssetBundle.HasAsset(assetName);
    //}

    ///// <summary>
    ///// ͬ�����ظ�AB���ڵ���Դ
    ///// </summary>
    ///// <param name="resPath"></param>
    ///// <param name="loadType"></param>
    ///// <returns></returns>
    //public Object Load(string resPath, Type loadType)
    //{
    //    var asset = RefAssetBundle.LoadAsset(resPath, loadType);
    //    return asset;
    //}

    ///// <summary>
    ///// �첽���ظ�AB���ڵ���Դ
    ///// </summary>
    ///// <param name="resPath"></param>
    ///// <param name="loadType"></param>
    ///// <returns></returns>
    //public async UniTask<Object> LoadAsync(string resPath, Type loadType)
    //{
    //    var request = RefAssetBundle.LoadAssetAsync(resPath, loadType);
       
    //    return await request;
    //}



}
