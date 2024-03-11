using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEditor;

/// <summary>
/// 配置基类
/// 少打点字罢了
/// </summary>

public class ConfigBase : SerializedScriptableObject
{
    public virtual void SaveConfig()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    protected virtual void OnDisable()
    {
        SaveConfig();
    }
}
