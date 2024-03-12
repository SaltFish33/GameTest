using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Entry : MonoBehaviour
{
    [Header("�Ƿ���Ҫ�ȸ�")]
    public bool needUpdate;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        UIMgr.Instance.ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.Mid, async (panel) =>
        {
            if (needUpdate)
            {
                ABUpdateMgr.Instance.CheckUpdate(async (isOver) =>
                {
                    if (!isOver) return;
                    AssetBundleHelper.Init();
                    await SpritesMgr.Instance.Init();
                    await MusicMgr.Instance.Init();
                    TicTacToeManager.Instance.Init();
                },
                (str) =>
                {
                    panel.SetDescrition(str);
                });
            }
            else
            {
                AssetBundleHelper.Init();
                await SpritesMgr.Instance.Init();
                await MusicMgr.Instance.Init();
                TicTacToeManager.Instance.Init();
            }
        });

        
    }

    private void OnDestroy()
    {
        TicTacToeManager.Instance.OnDestroy();
        SpritesMgr.Instance.OnDestroy();
        MusicMgr.Instance.OnDestroy();
        AssetBundleHelper.Reset();
    }
}
