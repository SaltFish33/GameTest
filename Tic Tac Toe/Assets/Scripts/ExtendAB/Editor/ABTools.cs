using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;


public class ABTools : EditorWindow
{
    private int nowSelIndex = 0;
    private string[] targetStrings = new string[] { "PC", "IOS", "ANDROID" };
    //������Ĭ��IP��ַ
    private string serverIP = "ftp://127.0.0.1";

    [MenuItem("AB������/�򿪹��ߴ���")]
    private static void OpenWindow()
    {
        //��ȡһ��ABTools�༭�����ڶ���
        ABTools window = EditorWindow.GetWindowWithRect(typeof(ABTools), new Rect(0, 0, 350, 300)) as ABTools;
        window.Show();


    }

    private void OnGUI()
    {
        
        GUI.Label(new Rect(10, 10, 150, 15), "ƽ̨ѡ��");
        //ҳǩ��ʾ �Ǵ�������ȡ���ַ�����������ʾ
        nowSelIndex = GUI.Toolbar(new Rect(10, 30, 250, 20), nowSelIndex, targetStrings);
        //��Դ������IP��ַ����
        GUI.Label(new Rect(10, 60, 150, 15), "��Դ��������ַ");

        serverIP = GUI.TextField(new Rect(10, 80, 150, 20),serverIP);
        ABUpdateMgr.Instance.serverIP = this.serverIP;

        if(GUI.Button(new Rect(10, 120, 100, 40), "���"))
        {
            CreateAssetBundles();
        }
        if(GUI.Button(new Rect(115, 120, 225, 40), "����AB��Lable"))
        {
            SetAssetBundleLable();
        }

        //�����Ա��ļ���ť
        if(GUI.Button(new Rect(10, 180, 100, 40), "�����Ա��ļ�"))
        {
            CreateABCompareFile();
        }
        //����Ĭ����Դ��SA
        if (GUI.Button(new Rect(115, 180, 225, 40), "����Ĭ����Դ��StreamingAssets"))
        {
            MoveABToStreamAssets();
        }
        //�ϴ�AB���ͶԱ��ļ���ť
        if (GUI.Button(new Rect(10, 240, 330, 40), "�ϴ�AB���ͶԱ��ļ�"))
        {
            UploadAllABFile();
        }
    }

    public static void SetAssetBundleLable()
    {
        //�Ƴ�����δʹ�õı��
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //��ñ���AB���ļ��µ������ļ���
        DirectoryInfo directoryInfo = new DirectoryInfo(Define.LocalAssetBundlePath);
        var directories = directoryInfo.GetDirectories();
        foreach (var info in directories)
        {
            string prefixPath = Define.LocalAssetBundlePath + "/" + info.Name;
            var inDirectories = new DirectoryInfo(prefixPath);
            if (inDirectories == null)
            {
                Debug.Log($"{info.Name}�ڲ������ļ�");
            }
            else
            {
                //Debug.Log(info.Name);
                DFSForFile(inDirectories, info.Name);
            }
        }
        Debug.Log("AB����ǳɹ�!");
    }

    /// <summary>
    /// �����ļ�
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="prefix"></param>
    private static void DFSForFile(FileSystemInfo fileInfo, string prefix)
    {
        if (!fileInfo.Exists)
        {
            return;
        }
        DirectoryInfo directoryInfo = fileInfo as DirectoryInfo;
        FileSystemInfo[] files = directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo file in files)
        {
            FileInfo info = file as FileInfo;
            if (info == null)
            {
                //����޷�ת��Ϊ�ļ���˵��Ϊ�ļ��У���Ҫ���ѣ������ǰ׺
                //Debug.Log(prefix + "/" + file.Name);
                DFSForFile(file, prefix + "/" + file.Name);
            }
            else
            {
                //Ϊ�ļ���������AB��Lable
                SetLable(info, prefix);
            }
        }
    }


    /// <summary>
    /// ����AB�����
    /// </summary>
    /// <param name="file"></param>
    /// <param name="prefix"></param>
    private static void SetLable(FileInfo file, string prefix)
    {
        //meta�ļ�����
        if (file.Extension == ".meta")
            return;
        string bundleName = prefix + "/" + file.Name;
        string extension = bundleName.Substring(0, bundleName.IndexOf('/'));
        string path = (Define.LocalAssetBundlePath + "/" + bundleName).UnityToWindowsPath();
        int index = path.IndexOf("Assets");
        AssetImporter importer = AssetImporter.GetAtPath(path.Substring(index));
        importer.SetAssetBundleNameAndVariant(prefix, null);
    }




    private void CreateAssetBundles()
    {
        string outputPath = Define.AssetBundlesOutputPath + "/" + targetStrings[nowSelIndex];

        DirectoryInfo output = new DirectoryInfo(outputPath);
        foreach (FileInfo file in output.GetFiles())
            file.Delete();

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        AssetDatabase.Refresh();
    }


    //����AB���Ա��ļ�
    public  void CreateABCompareFile()
    {
        //����AB���ļ��� ��ȡ����AB���ļ���Ϣ
        //��ȡ�ļ�����Ϣ
        //Ҫ����ѡ���ƽ̨��ȡ��Ӧƽ̨�ļ����µ�����
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
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
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] +"/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
        Debug.Log("AB����Դ�Ա��ļ����ɳɹ�");

    }

    //��ȡ�ļ�MD5��
    public  string GetMD5(string filePath)
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

    //��ѡ����Դ�ƶ���StreamingAssets�ļ�����
    public  void MoveABToStreamAssets()
    {
        //ͨ���༭����ȡ��Project��ѡ�е���Դ
        UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        //���һ����Դ��û��ѡ�� ���˳�
        if (selectedAsset.Length == 0)
            return;
        //����ƴ��Ĭ�ϱ���AB����Դ��Ϣ���ַ���
        string abCompareInfo = "";


        foreach (UnityEngine.Object asset in selectedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            //��ȡ·�����е��ļ��� ������ΪStreamingAssets�е��ļ���
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

            //�ж��Ƿ���.���� ����� ֤���к�׺

            //string externName = fileName.Substring(fileName.LastIndexOf("."));
            //Debug.Log(externName);
            //if (externName != null && (externName != ".assetbundle" || externName != ".text"))
            //    continue;

            if (fileName.IndexOf('.') != -1)
                continue;


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

    //�ϴ�AB���ļ���������
    private  void UploadAllABFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
        //��ȡ��Ŀ¼�µ������ļ���Ϣ
        FileInfo[] fileInofs = directory.GetFiles();



        foreach (var info in fileInofs)
        {
            //û�к�׺�Ĳ���AB��,������Ҫ��ȡ��Դ�Ա��ļ� ��ʽ��.txt
            //���ļ�����ֻ�жԱ��ļ��ĸ�ʽ��txt
            if (info.Extension == ".assetbundle" || info.Extension == ".txt")
            {
                //�ϴ����ļ�
                FtpUploadFile(info.FullName, info.Name);

            }
        }
    }

    //�ϴ��ļ���������
    private async  void FtpUploadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                //1.����FTP���� �����ϴ�
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + targetStrings[nowSelIndex] +"/" + fileName)) as FtpWebRequest;
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
