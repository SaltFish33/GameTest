using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ABUpdateMgr : MonoBehaviour
{


    //存储远端资源对比文件数据的字典
    private Dictionary<string, ABInfo> remoteABInfo = new Dictionary<string, ABInfo>();

    //存储本地资源对比文件数据的字典
    private Dictionary<string, ABInfo> localABInfo = new Dictionary<string, ABInfo>();


    //这个是待下载的AB包列表文件 存储AB包的名字
    private List<string> downLoadList = new List<string>();

    private static ABUpdateMgr instance;

    public static ABUpdateMgr Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = GameObject.Find("ABUpdateMgr");
                if (obj != null)
                    instance = obj.GetComponent<ABUpdateMgr>();
                else
                {
                    obj = new GameObject("ABUpdateMgr");
                    instance = obj.AddComponent<ABUpdateMgr>();
                }
                
            }
            return instance;
        }
    }

    public string serverIP = "ftp://127.0.0.1";

    private void OnDestroy()
    {
        instance = null;
    }

    public async void DownLoadABCompareFile(UnityAction<bool> overCallBack)
    {
        //1.从资源服务器下载资源对比文件
        // www UnityWebRequest  Ftp相关API
        print(Application.persistentDataPath);
        string path = Application.persistentDataPath;
        bool isOver = false;
        int reDownLoadNum = 5;
        while (!isOver && reDownLoadNum > 0)
        {
            await Task.Run(()=> { 
                isOver = DownLoadFile("ABCompareInfo.txt",path + "/ABCompareInfo_TMP.txt");
            });
            --reDownLoadNum;
        }
        //告诉外部成功与否
        overCallBack?.Invoke(isOver);
        
        

    }

    /// <summary>
    /// 获取下载下来的AB包中的信息
    /// </summary>
    public void GetABCompareFileInfo(string info, Dictionary<string, ABInfo> ABInfo)
    {
       
            //2.后去资源对比文件中的 字符串信息 进行拆分
            //string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
            //不读取了 直接让外面传进来
            string[] strs = info.Split("|"); //通过|拆分字符串 把一个个AB包拆分出来
            string[] infos = null;

            for (int i = 0; i < strs.Length; i++)
            {
                //利用分隔符" "将各个信息拆分出来
                infos = strs[i].Split(' ');
                //将读取的包用字典存储 用于之后和本地的对比文件进行对比
                ABInfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
            }   
    }


    public async void DownLoadABFile(UnityAction<bool> overCallBack, UnityAction<string> updatePro)
    {

        //1.遍历字典的键 根据文件名 去下载AB包到本地
        //foreach (string name in remoteABInfo.Keys)
        //{
        //    //直接放入 待下载的列表中
        //    downLoadList.Add(name);
        //}

        //本地存储的路径 由于多线程不能访问UNity相关的内容 所以声明在外部
      
        string localPath = Application.persistentDataPath + "/";
        //是否下载成功的标识
        bool isOver = false;
        //下载成功的列表
        List<string> tempList = new List<string>();
        //最大重新下载次数
        int redownLoadMaxNum = 5;
        //下载成功的资源数
        int downLoadOverNum = 0;
        //这一次下载需要下载多少个资源
        int downLoadMaxNum = downLoadList.Count;
        //while循环的目的是 进行n次重新下载 避免网络异常
        while (downLoadList.Count > 0 && redownLoadMaxNum > 0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {

                await Task.Run(() => {
                    isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]);
                });
                if (isOver)
                {
                    updatePro(++downLoadOverNum + "/" + downLoadMaxNum);
                  
                    tempList.Add(downLoadList[i]);
                }

            }
            //把下载成功的文件名 从待下载列表中移除
            for (int i = 0; i < tempList.Count; i++)
            {
                downLoadList.Remove(tempList[i]);
            }
            --redownLoadMaxNum;
        }
        //所有内容都下载完了 告诉外部下载完成
       
        overCallBack(downLoadList.Count == 0);

        //粗暴的 认为 网络有问题 直接去进行网络检测 等等内容
        

        //2.要知道现在下载了多少 结束与否




    }

    private bool DownLoadFile(string fileName,string localPath)
    {
        try
        {
            string pInfo =
#if UNITY_IOS
"IOS";
#elif UNITY_ANDROID
"ANDROID";
#else
"PC";
#endif
            //1.创建FTP连接 用于下载
            FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP +  "/AB/" + pInfo +"/" + fileName)) as FtpWebRequest;
            //2.设置一个通信凭证 这样才能下载(如果有匿名账号 可以不设置凭证)
            //NetworkCredential n = new NetworkCredential("Sun", "szy200284123");
            //req.Credentials = n;

            //3.其他设置
            //  设置代理为null
            req.Proxy = null;
            //  请求完毕后 是否关闭控制连接的参数
            req.KeepAlive = false;
            //  操作命令-上传
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            //  指定传输类型 2进制
            req.UseBinary = true;
            //4.下载文件
            //  ftp流对象
            

            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            using (FileStream file = File.Create(localPath))
            {
                //一点一点的下载内容
                byte[] bytes = new byte[2048];
                //返回值代表下载了多少个字节
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                //循环下载文件中的数据
                while (contentLength != 0)
                {
                    //写入到本地文件流中
                    file.Write(bytes, 0, contentLength);
                    //写完再读
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                //循环完毕后 证明下载结束
                file.Close();
                downLoadStream.Close();
                Debug.Log(fileName + "下载成功");
                return true;
            }
        }
        catch (Exception ex)
        {

            Debug.Log(fileName + "下载失败" + ex.Message);
            return false;
        }

    }

    public void GetLocalABCompareFileInfo(UnityAction<bool> overCallBack)
    {
        //Application.persistentDataPath;
        //如果可读可写文件夹中 存在对比文件 说明之前已经下载过了
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetLocalABCompareFileInfo("file:///" + Application.persistentDataPath + "/ABCompareInfo.txt", overCallBack));
        }
        //只有当可读可写中没有对比文件 才回来加载默认资源(第一次进游戏才会发生)
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
        {

            string path =
#if UNITY_ANDROID
Application.streamingAssetsPath;
#else
"file:///" + Application.streamingAssetsPath;
#endif
            StartCoroutine(GetLocalABCompareFileInfo(path + "/ABCompareInfo.txt", overCallBack));

        }
        else
            overCallBack(true);
        //如果两个都不进 说明第一次并且没有默认资源


    }

    private IEnumerator GetLocalABCompareFileInfo(string filePath,UnityAction<bool> overCallBack)
    {
        //通过UnityWebRequest 去加载本地文件
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();
        //获取文件成功 继续往下执行
        if(req.result == UnityWebRequest.Result.Success)
        {
            GetABCompareFileInfo(req.downloadHandler.text, localABInfo);
            overCallBack(true);
        }

    }

    /// <summary>
    /// 用于检测资源热更新的函数
    /// TODO：用UniTask重写
    /// </summary>
    /// <param name="overCallBack"></param>
    /// <param name="updateInfoCallBack"></param>
    public void CheckUpdate(UnityAction<bool> overCallBack, UnityAction<string> updateInfoCallBack )
    {
        //为了避免上一次报错 而残留信息
        downLoadList.Clear();
        remoteABInfo.Clear();
        localABInfo.Clear();

        //1.加载远端资源对比文件
        DownLoadABCompareFile((isOver)=> {
            updateInfoCallBack("开始更新资源");
            if (isOver)
            {
                updateInfoCallBack("对比文件下载结束");
                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                updateInfoCallBack("解析远端对比文件");
                GetABCompareFileInfo(remoteInfo, remoteABInfo);
                updateInfoCallBack("解析远端对比文件完成");

                //2.加载本地资源对比文件
                GetLocalABCompareFileInfo((isOver) =>{
                    if (isOver)
                    {
                        updateInfoCallBack("解析本地对比文件完成");
                        //3.对比他们 进行AB包加载
                        updateInfoCallBack("开始对比");
                        
                        //3.判断 哪些资源是需要删除的
                        foreach (string abName in remoteABInfo.Keys)
                        {
                            //1.判断哪些资源是新的 然后记录 之后去下载
                            if (!localABInfo.ContainsKey(abName))
                            {
                                //在本地没找到 说明要更新
                                downLoadList.Add(abName);
                            }
                            //发现本地有同名AB包
                            else
                            {
                                //2.判断 哪些资源是需要更新的 记录 下载
                                //对比MD5码 判断是否要更新
                                if (localABInfo[abName].md5 != remoteABInfo[abName].md5)
                                {
                                    downLoadList.Add(abName);
                                }
                                //如果相等 就不需要更新
                                //每次检测完同一名字的AB包 就把该AB包从本地移除
                                //那么本地剩下的AB包 就是可以删除了
                                localABInfo.Remove(abName);
                            }
                        }
                        updateInfoCallBack("对比完成");
                        updateInfoCallBack("开始删除无用的AB包文件");
                        //删除无用的AB包
                        foreach (string abName in localABInfo.Keys)
                        {
                            //如果可读写文件中有该文件 就删
                            //只读文件中 删不了
                            if(File.Exists(Application.persistentDataPath + "/" + abName))
                            {
                                File.Delete(Application.persistentDataPath + "/" + abName);
                            }
                        }
                        //下载待更新的所有AB包
                        updateInfoCallBack("下载和更新AB包文件");
                        DownLoadABFile((isOver) =>
                        {
                            if (isOver)
                            {
                                //下载完所有AB包文件后
                                //把本地的AB包对比文件更新
                                //把之前读取出来的 远端对比文件存储到本地
                                File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt",remoteInfo);
                            }
                            overCallBack(isOver);
                        }, updateInfoCallBack);

                    }
                    else
                        overCallBack(false);
                });
            }
            else
            {
                overCallBack(false);
            }
        });
    }



    public class ABInfo
    {
        public string name;
        public long size;
        public string md5;

        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }

        public static bool operator ==(ABInfo a, ABInfo b)
        {
            if (a.md5 == b.md5)
                return true;
            else
                return false;
        }

        public static bool operator !=(ABInfo a, ABInfo b)
        {
            if (a.md5 == b.md5)
                return false;
            else
                return true;
        }

    }
}


