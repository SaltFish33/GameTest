using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesMgr : BaseManager<SpritesMgr>
{
    private static readonly Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();

    private static readonly string ABName = "sprites";

    public async UniTask Init()
    {
        await AssetBundleHelper.LoadAssetBundle(ABName);
        spritesDic.Clear();
    }

    public async UniTask<Sprite> LoadSprite(string name)
    {
        if(!spritesDic.TryGetValue(name, out var sprite))
        {
            sprite = (Sprite)await AssetBundleHelper.LoadAsset(name, ABName, typeof(Sprite));
        }
        return sprite;
    }



    public void OnDestroy()
    {
        List<string> removeList = new List<string>();
        foreach (var sprite in spritesDic.Keys)
        {
            removeList.Add(sprite);
        }
        spritesDic.Clear();
        foreach (var sprite in removeList) 
        {
            AssetBundleHelper.UnLoadAsset(ABName, sprite);
        }
        AssetBundleHelper.UnLoadAssetBundle(ABName);
        
    }


}
