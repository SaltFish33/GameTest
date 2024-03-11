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
    //服务器默认IP地址
    private string serverIP = "ftp://127.0.0.1";

    [MenuItem("AB包工具/打开工具窗口")]
    private static void OpenWindow()
    {
        //获取一个ABTools编辑器窗口对象
        ABTools window = EditorWindow.GetWindowWithRect(typeof(ABTools), new Rect(0, 0, 350, 300)) as ABTools;
        window.Show();


    }

    private void OnGUI()
    {
        
        GUI.Label(new Rect(10, 10, 150, 15), "平台选择");
        //页签显示 是从数组中取出字符串内容来显示
        nowSelIndex = GUI.Toolbar(new Rect(10, 30, 250, 20), nowSelIndex, targetStrings);
        //资源服务器IP地址设置
        GUI.Label(new Rect(10, 60, 150, 15), "资源服务器地址");

        serverIP = GUI.TextField(new Rect(10, 80, 150, 20),serverIP);
        ABUpdateMgr.Instance.serverIP = this.serverIP;

        if(GUI.Button(new Rect(10, 120, 100, 40), "打包"))
        {
            CreateAssetBundles();
        }
        if(GUI.Button(new Rect(115, 120, 225, 40), "设置AB包Lable"))
        {
            SetAssetBundleLable();
        }

        //创建对比文件按钮
        if(GUI.Button(new Rect(10, 180, 100, 40), "创建对比文件"))
        {
            CreateABCompareFile();
        }
        //保存默认资源到SA
        if (GUI.Button(new Rect(115, 180, 225, 40), "保存默认资源到StreamingAssets"))
        {
            MoveABToStreamAssets();
        }
        //上传AB包和对比文件按钮
        if (GUI.Button(new Rect(10, 240, 330, 40), "上传AB包和对比文件"))
        {
            UploadAllABFile();
        }
    }

    public static void SetAssetBundleLable()
    {
        //移除所有未使用的标记
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //获得本地AB包文件下的所有文件夹
        DirectoryInfo directoryInfo = new DirectoryInfo(Define.LocalAssetBundlePath);
        var directories = directoryInfo.GetDirectories();
        foreach (var info in directories)
        {
            string prefixPath = Define.LocalAssetBundlePath + "/" + info.Name;
            var inDirectories = new DirectoryInfo(prefixPath);
            if (inDirectories == null)
            {
                Debug.Log($"{info.Name}内不包含文件");
            }
            else
            {
                //Debug.Log(info.Name);
                DFSForFile(inDirectories, info.Name);
            }
        }
        Debug.Log("AB包标记成功!");
    }

    /// <summary>
    /// 深搜文件
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
                //如果无法转换为文件，说明为文件夹，需要深搜，并添加前缀
                //Debug.Log(prefix + "/" + file.Name);
                DFSForFile(file, prefix + "/" + file.Name);
            }
            else
            {
                //为文件，设置其AB包Lable
                SetLable(info, prefix);
            }
        }
    }


    /// <summary>
    /// 设置AB包标记
    /// </summary>
    /// <param name="file"></param>
    /// <param name="prefix"></param>
    private static void SetLable(FileInfo file, string prefix)
    {
        //meta文件滚！
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


    //生成AB包对比文件
    public  void CreateABCompareFile()
    {
        //遍历AB包文件夹 获取所有AB包文件信息
        //获取文件夹信息
        //要根据选择的平台读取对应平台文件夹下的内容
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
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
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] +"/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
        Debug.Log("AB包资源对比文件生成成功");

    }

    //获取文件MD5码
    public  string GetMD5(string filePath)
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

    //将选中资源移动到StreamingAssets文件夹中
    public  void MoveABToStreamAssets()
    {
        //通过编辑器获取在Project中选中的资源
        UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        //如果一个资源都没有选择 就退出
        if (selectedAsset.Length == 0)
            return;
        //用于拼接默认本地AB宝资源信息的字符串
        string abCompareInfo = "";


        foreach (UnityEngine.Object asset in selectedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            //截取路径当中的文件名 用于作为StreamingAssets中的文件名
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

            //判断是否有.符号 如果有 证明有后缀

            //string externName = fileName.Substring(fileName.LastIndexOf("."));
            //Debug.Log(externName);
            //if (externName != null && (externName != ".assetbundle" || externName != ".text"))
            //    continue;

            if (fileName.IndexOf('.') != -1)
                continue;


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

    //上传AB包文件到服务器
    private  void UploadAllABFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] + "/");
        //获取该目录下的所有文件信息
        FileInfo[] fileInofs = directory.GetFiles();



        foreach (var info in fileInofs)
        {
            //没有后缀的才是AB包,还有需要获取资源对比文件 格式是.txt
            //该文件夹中只有对比文件的格式是txt
            if (info.Extension == ".assetbundle" || info.Extension == ".txt")
            {
                //上传该文件
                FtpUploadFile(info.FullName, info.Name);

            }
        }
    }

    //上传文件到服务器
    private async  void FtpUploadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                //1.创建FTP连接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + targetStrings[nowSelIndex] +"/" + fileName)) as FtpWebRequest;
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
