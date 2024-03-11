using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MoveABToSA
{
   //[MenuItem("AB������/�ƶ�ѡ����Դ��StreamingAssets")]
   public static void MoveABToStreamAssets()
    {
        //ͨ���༭����ȡ��Project��ѡ�е���Դ
        Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //���һ����Դ��û��ѡ�� ���˳�
        if (selectedAsset.Length == 0)
            return;
        //����ƴ��Ĭ�ϱ���AB����Դ��Ϣ���ַ���
        string abCompareInfo = "";


        foreach (Object asset in selectedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            //��ȡ·�����е��ļ��� ������ΪStreamingAssets�е��ļ���
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

            //�ж��Ƿ���.���� ����� ֤���к�׺
            if (fileName.IndexOf('.') != -1)
                continue;
            
            //�㻹�����ڿ���֮ǰ ȥ��ȡȫ·�� �ú�ͨ��FileInfoȥ��ȡ��׺���ж�

            //����AssetDtaBase�е�API ��ѡ���ļ� ����
            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);
            //��ȡ������SA�ļ����е��ļ���ȫ·��
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            //ƴ��AB����Ϣ���ַ�����
            abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " + CreateABCompare.GetMD5(fileInfo.FullName);
            //��һ�����Ÿ������AB����Ϣ
            abCompareInfo += "|";

        }
        //��ȡ���һ������
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        //������Ĭ����Դ����Ϣ�����ļ�
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);

        AssetDatabase.Refresh();
    }
}
