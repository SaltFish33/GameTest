using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CreateABCompare
{
    //[MenuItem("AB包工具/创建对比文件")]
    public static void CreateABCompareFile()
    {
        //遍历AB包文件夹 获取所有AB包文件信息
        //获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/PC/");
        //后去该目录下的所有文件信息
        FileInfo[] fileInofs = directory.GetFiles();

        //用于存储信息的字符串
        string abCompareInfo = "";


        foreach (var info in fileInofs)
        {
            //没有后缀的才是AB包
            if (info.Extension == "")
            {
                //拼接一个AB包的信息
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                //用一个分隔符分开不同文件之间的信息
                abCompareInfo += "|";
            }
        }
        //因为循环完毕后 会在最后有一个符号| 所以把它去掉
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        
        //存储拼接好的 AB包资源信息
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/PC/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
        Debug.Log("AB包资源对比文件生成成功");
        
    
    }


    public static string GetMD5(string filePath)
    {
        //将文件以流的形式打开
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            //声明一个MD5对象
            //用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            //利用API得到 数据的MD5码 16个字节
            byte[] md5Info = md5.ComputeHash(file);

            //关闭文件流
            file.Close();

            //把16个字节转换为 16进制 拼接成字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
