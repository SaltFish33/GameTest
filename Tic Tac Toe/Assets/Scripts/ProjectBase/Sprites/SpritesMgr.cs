using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpritesMgr : BaseManager<SpritesMgr>
{
    private static readonly Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();

    private static readonly Dictionary<string, SpriteAtlas> altasDic = new Dictionary<string, SpriteAtlas>();

    private static readonly string ABName = "sprites";

    public async UniTask Init()
    {
        await AssetBundleHelper.LoadAssetBundle(ABName);
        altasDic.Clear();
        spritesDic.Clear();
    }

    private async UniTask<SpriteAtlas> LoadSpriteAltas(string name)
    {
        if (!altasDic.ContainsKey(name))
        {
            var altas = (SpriteAtlas)await AssetBundleHelper.LoadAsset(name, ABName, typeof(SpriteAtlas));
            altasDic[name] = altas;
        }
        return altasDic[name];
    }


    public async UniTask<Sprite> LoadSprite(string spriteName, string altasName)
    {
        if(!spritesDic.TryGetValue(spriteName, out var sprite))
        {
            if(!altasDic.TryGetValue(altasName, out var altas))
            {
                altas = await LoadSpriteAltas(altasName);
            }
            sprite = altas.GetSprite(spriteName);
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
        foreach (var altas in altasDic.Keys)
        {
            removeList.Add(altas);
        }
        altasDic.Clear();
        foreach (var sprite in removeList) 
        {
            AssetBundleHelper.UnLoadAsset(ABName, sprite);
        }
        AssetBundleHelper.UnLoadAssetBundle(ABName);
        
    }


}
