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
    //[MenuItem("AB������/�ϴ�AB���ͶԱ��ļ�")]
    private static void UploadAllABFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/PC/");
        //��ȥ��Ŀ¼�µ������ļ���Ϣ
        FileInfo[] fileInofs = directory.GetFiles();

      

        foreach (var info in fileInofs)
        {
            //û�к�׺�Ĳ���AB��,������Ҫ��ȡ��Դ�Ա��ļ� ��ʽ��.txt
            //���ļ�����ֻ�жԱ��ļ��ĸ�ʽ��txt
            if (info.Extension == "" || info.Extension == ".txt")
            {
                //�ϴ����ļ�
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
                //1.����FTP���� �����ϴ�
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://127.0.0.1/AB/PC/" + fileName)) as FtpWebRequest;
                //2.����һ��ͨ��ƾ֤ ���������ϴ�
                NetworkCredential n = new NetworkCredential("Sun", "szy200284123");
                req.Credentials = n;

                //3.��������
                //  ���ô���Ϊnull
                req.Proxy = null;
                //  ������Ϻ� �Ƿ�رտ������ӵĲ���
                req.KeepAlive = false;
                //  ��������-�ϴ�
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //  ָ���������� 2����
                req.UseBinary = true;
                //4.�ϴ��ļ�
                //  ftp������
                Stream upLoadStream = req.GetRequestStream();
                
                //  ��ȡ�ļ���Ϣ д���������
                using (FileStream file = File.OpenRead(filePath))
                {
                    //һ��һ����ϴ�����
                    byte[] bytes = new byte[2048];
                    //����ֵ�����ȡ�˶��ٸ��ֽ�
                    int contentLength = file.Read(bytes, 0, bytes.Length);

                    //ѭ���ϴ��ļ��е�����
                    while (contentLength != 0)
                    {
                        upLoadStream.Write(bytes, 0, contentLength);
                        //д���ٶ�
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    //ѭ����Ϻ� ֤���ϴ�����
                    file.Close();
                    upLoadStream.Close();
                }
                Debug.Log(fileName + "�ϴ��ɹ�");
            }
            catch (Exception ex)
            {
                Debug.Log("�ϴ�ʧ��" + ex.Message);

            }
        });
        
    }




}
