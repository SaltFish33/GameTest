
using UnityEngine;

public static class Define
{
    /// <summary>
    /// 流文件夹目录
    /// </summary>
    public static string StreamingAssetPath => Application.streamingAssetsPath;

    /// <summary>
    /// 可读写文件夹目录
    /// </summary>
    public static string PersistentDataPath => Application.persistentDataPath;

    public static readonly string LocalAssetBundlePath = Application.dataPath + "/Bundles";

    public static readonly string AssetBundlesOutputPath = Application.dataPath + "/ArtRes" + "/AB";


    public static string UnityToWindowsPath(this string self)
    {
        return self.Replace('/', '\\');
    }
}
