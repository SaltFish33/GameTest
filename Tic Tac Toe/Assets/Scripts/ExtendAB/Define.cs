
using UnityEngine;

public static class Define
{
    /// <summary>
    /// ���ļ���Ŀ¼
    /// </summary>
    public static string StreamingAssetPath => Application.streamingAssetsPath;

    /// <summary>
    /// �ɶ�д�ļ���Ŀ¼
    /// </summary>
    public static string PersistentDataPath => Application.persistentDataPath;

    public static readonly string LocalAssetBundlePath = Application.dataPath + "/Bundles";

    public static readonly string AssetBundlesOutputPath = Application.dataPath + "/ArtRes" + "/AB";


    public static string UnityToWindowsPath(this string self)
    {
        return self.Replace('/', '\\');
    }
}
