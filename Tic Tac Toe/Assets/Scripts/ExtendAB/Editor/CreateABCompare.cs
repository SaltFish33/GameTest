using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CreateABCompare
{
    //[MenuItem("AB������/�����Ա��ļ�")]
    public static void CreateABCompareFile()
    {
        //����AB���ļ��� ��ȡ����AB���ļ���Ϣ
        //��ȡ�ļ�����Ϣ
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/PC/");
        //��ȥ��Ŀ¼�µ������ļ���Ϣ
        FileInfo[] fileInofs = directory.GetFiles();

        //���ڴ洢��Ϣ���ַ���
        string abCompareInfo = "";


        foreach (var info in fileInofs)
        {
            //û�к�׺�Ĳ���AB��
            if (info.Extension == "")
            {
                //ƴ��һ��AB������Ϣ
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                //��һ���ָ����ֿ���ͬ�ļ�֮�����Ϣ
                abCompareInfo += "|";
            }
        }
        //��Ϊѭ����Ϻ� ���������һ������| ���԰���ȥ��
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        
        //�洢ƴ�Ӻõ� AB����Դ��Ϣ
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/PC/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
        Debug.Log("AB����Դ�Ա��ļ����ɳɹ�");
        
    
    }


    public static string GetMD5(string filePath)
    {
        //���ļ���������ʽ��
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            //����һ��MD5����
            //��������MD5��
            MD5 md5 = new MD5CryptoServiceProvider();
            //����API�õ� ���ݵ�MD5�� 16���ֽ�
            byte[] md5Info = md5.ComputeHash(file);

            //�ر��ļ���
            file.Close();

            //��16���ֽ�ת��Ϊ 16���� ƴ�ӳ��ַ���
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
