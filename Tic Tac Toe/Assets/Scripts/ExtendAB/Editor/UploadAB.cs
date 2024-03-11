using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;

public class UploadAB 
{
    //[MenuItem("AB包工具/上传AB包和对比文件")]
    private static void UploadAllABFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/PC/");
        //后去该目录下的所有文件信息
        FileInfo[] fileInofs = directory.GetFiles();

      

        foreach (var info in fileInofs)
        {
            //没有后缀的才是AB包,还有需要获取资源对比文件 格式是.txt
            //该文件夹中只有对比文件的格式是txt
            if (info.Extension == "" || info.Extension == ".txt")
            {
                //上传该文件
                FtpUploadFile(info.FullName, info.Name);

            }
        }
    }


    private async static void FtpUploadFile(string filePath,string fileName)
    {
        await Task.Run(() =>
        {

            try
            {
                //1.创建FTP连接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://127.0.0.1/AB/PC/" + fileName)) as FtpWebRequest;
                //2.设置一个通信凭证 这样才能上传
                NetworkCredential n = new NetworkCredential("Sun", "szy200284123");
                req.Credentials = n;

                //3.其他设置
                //  设置代理为null
                req.Proxy = null;
                //  请求完毕后 是否关闭控制连接的参数
                req.KeepAlive = false;
                //  操作命令-上传
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //  指定传输类型 2进制
                req.UseBinary = true;
                //4.上传文件
                //  ftp流对象
                Stream upLoadStream = req.GetRequestStream();
                
                //  读取文件信息 写入该流对象
                using (FileStream file = File.OpenRead(filePath))
                {
                    //一点一点的上传内容
                    byte[] bytes = new byte[2048];
                    //返回值代表读取了多少个字节
                    int contentLength = file.Read(bytes, 0, bytes.Length);

                    //循环上传文件中的数据
                    while (contentLength != 0)
                    {
                        upLoadStream.Write(bytes, 0, contentLength);
                        //写完再读
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    //循环完毕后 证明上传结束
                    file.Close();
                    upLoadStream.Close();
                }
                Debug.Log(fileName + "上传成功");
            }
            catch (Exception ex)
            {
                Debug.Log("上传失败" + ex.Message);

            }
        });
        
    }




}
