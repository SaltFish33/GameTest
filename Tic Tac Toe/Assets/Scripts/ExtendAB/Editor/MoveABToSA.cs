using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MoveABToSA
{
   //[MenuItem("AB包工具/移动选中资源到StreamingAssets")]
   public static void MoveABToStreamAssets()
    {
        //通过编辑器获取在Project中选中的资源
        Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //如果一个资源都没有选择 就退出
        if (selectedAsset.Length == 0)
            return;
        //用于拼接默认本地AB宝资源信息的字符串
        string abCompareInfo = "";


        foreach (Object asset in selectedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            //截取路径当中的文件名 用于作为StreamingAssets中的文件名
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

            //判断是否有.符号 如果有 证明有后缀
            if (fileName.IndexOf('.') != -1)
                continue;
            
            //你还可以在拷贝之前 去获取全路径 让后通过FileInfo去获取后缀来判断

            //利用AssetDtaBase中的API 将选中文件 复制
            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);
            //获取拷贝到SA文件夹中的文件的全路径
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            //拼接AB包信息到字符串中
            abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " + CreateABCompare.GetMD5(fileInfo.FullName);
            //用一个符号隔开多个AB包信息
            abCompareInfo += "|";

        }
        //截取最后一个符号
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        //将本地默认资源的信息存入文件
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);

        AssetDatabase.Refresh();
    }
}
