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


    //�洢Զ����Դ�Ա��ļ����ݵ��ֵ�
    private Dictionary<string, ABInfo> remoteABInfo = new Dictionary<string, ABInfo>();

    //�洢������Դ�Ա��ļ����ݵ��ֵ�
    private Dictionary<string, ABInfo> localABInfo = new Dictionary<string, ABInfo>();


    //����Ǵ����ص�AB���б��ļ� �洢AB��������
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
        //1.����Դ������������Դ�Ա��ļ�
        // www UnityWebRequest  Ftp���API
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
        //�����ⲿ�ɹ����
        overCallBack?.Invoke(isOver);
        
        

    }

    /// <summary>
    /// ��ȡ����������AB���е���Ϣ
    /// </summary>
    public void GetABCompareFileInfo(string info, Dictionary<string, ABInfo> ABInfo)
    {
       
            //2.��ȥ��Դ�Ա��ļ��е� �ַ�����Ϣ ���в��
            //string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
            //����ȡ�� ֱ�������洫����
            string[] strs = info.Split("|"); //ͨ��|����ַ��� ��һ����AB����ֳ���
            string[] infos = null;

            for (int i = 0; i < strs.Length; i++)
            {
                //���÷ָ���" "��������Ϣ��ֳ���
                infos = strs[i].Split(' ');
                //����ȡ�İ����ֵ�洢 ����֮��ͱ��صĶԱ��ļ����жԱ�
                ABInfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
            }   
    }


    public async void DownLoadABFile(UnityAction<bool> overCallBack, UnityAction<string> updatePro)
    {

        //1.�����ֵ�ļ� �����ļ��� ȥ����AB��������
        //foreach (string name in remoteABInfo.Keys)
        //{
        //    //ֱ�ӷ��� �����ص��б���
        //    downLoadList.Add(name);
        //}

        //���ش洢��·�� ���ڶ��̲߳��ܷ���UNity��ص����� �����������ⲿ
      
        string localPath = Application.persistentDataPath + "/";
        //�Ƿ����سɹ��ı�ʶ
        bool isOver = false;
        //���سɹ����б�
        List<string> tempList = new List<string>();
        //����������ش���
        int redownLoadMaxNum = 5;
        //���سɹ�����Դ��
        int downLoadOverNum = 0;
        //��һ��������Ҫ���ض��ٸ���Դ
        int downLoadMaxNum = downLoadList.Count;
        //whileѭ����Ŀ���� ����n���������� ���������쳣
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
            //�����سɹ����ļ��� �Ӵ������б����Ƴ�
            for (int i = 0; i < tempList.Count; i++)
            {
                downLoadList.Remove(tempList[i]);
            }
            --redownLoadMaxNum;
        }
        //�������ݶ��������� �����ⲿ�������
       
        overCallBack(downLoadList.Count == 0);

        //�ֱ��� ��Ϊ ���������� ֱ��ȥ���������� �ȵ�����
        

        //2.Ҫ֪�����������˶��� �������




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
            //1.����FTP���� ��������
            FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP +  "/AB/" + pInfo +"/" + fileName)) as FtpWebRequest;
            //2.����һ��ͨ��ƾ֤ ������������(����������˺� ���Բ�����ƾ֤)
            //NetworkCredential n = new NetworkCredential("Sun", "szy200284123");
            //req.Credentials = n;

            //3.��������
            //  ���ô���Ϊnull
            req.Proxy = null;
            //  ������Ϻ� �Ƿ�رտ������ӵĲ���
            req.KeepAlive = false;
            //  ��������-�ϴ�
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            //  ָ���������� 2����
            req.UseBinary = true;
            //4.�����ļ�
            //  ftp������
            

            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            using (FileStream file = File.Create(localPath))
            {
                //һ��һ�����������
                byte[] bytes = new byte[2048];
                //����ֵ���������˶��ٸ��ֽ�
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                //ѭ�������ļ��е�����
                while (contentLength != 0)
                {
                    //д�뵽�����ļ�����
                    file.Write(bytes, 0, contentLength);
                    //д���ٶ�
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }
                //ѭ����Ϻ� ֤�����ؽ���
                file.Close();
                downLoadStream.Close();
                Debug.Log(fileName + "���سɹ�");
                return true;
            }
        }
        catch (Exception ex)
        {

            Debug.Log(fileName + "����ʧ��" + ex.Message);
            return false;
        }

    }

    public void GetLocalABCompareFileInfo(UnityAction<bool> overCallBack)
    {
        //Application.persistentDataPath;
        //����ɶ���д�ļ����� ���ڶԱ��ļ� ˵��֮ǰ�Ѿ����ع���
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetLocalABCompareFileInfo("file:///" + Application.persistentDataPath + "/ABCompareInfo.txt", overCallBack));
        }
        //ֻ�е��ɶ���д��û�жԱ��ļ� �Ż�������Ĭ����Դ(��һ�ν���Ϸ�Żᷢ��)
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
        //������������� ˵����һ�β���û��Ĭ����Դ


    }

    private IEnumerator GetLocalABCompareFileInfo(string filePath,UnityAction<bool> overCallBack)
    {
        //ͨ��UnityWebRequest ȥ���ر����ļ�
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();
        //��ȡ�ļ��ɹ� ��������ִ��
        if(req.result == UnityWebRequest.Result.Success)
        {
            GetABCompareFileInfo(req.downloadHandler.text, localABInfo);
            overCallBack(true);
        }

    }

    /// <summary>
    /// ���ڼ����Դ�ȸ��µĺ���
    /// TODO����UniTask��д
    /// </summary>
    /// <param name="overCallBack"></param>
    /// <param name="updateInfoCallBack"></param>
    public void CheckUpdate(UnityAction<bool> overCallBack, UnityAction<string> updateInfoCallBack )
    {
        //Ϊ�˱�����һ�α��� ��������Ϣ
        downLoadList.Clear();
        remoteABInfo.Clear();
        localABInfo.Clear();

        //1.����Զ����Դ�Ա��ļ�
        DownLoadABCompareFile((isOver)=> {
            updateInfoCallBack("��ʼ������Դ");
            if (isOver)
            {
                updateInfoCallBack("�Ա��ļ����ؽ���");
                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                updateInfoCallBack("����Զ�˶Ա��ļ�");
                GetABCompareFileInfo(remoteInfo, remoteABInfo);
                updateInfoCallBack("����Զ�˶Ա��ļ����");

                //2.���ر�����Դ�Ա��ļ�
                GetLocalABCompareFileInfo((isOver) =>{
                    if (isOver)
                    {
                        updateInfoCallBack("�������ضԱ��ļ����");
                        //3.�Ա����� ����AB������
                        updateInfoCallBack("��ʼ�Ա�");
                        
                        //3.�ж� ��Щ��Դ����Ҫɾ����
                        foreach (string abName in remoteABInfo.Keys)
                        {
                            //1.�ж���Щ��Դ���µ� Ȼ���¼ ֮��ȥ����
                            if (!localABInfo.ContainsKey(abName))
                            {
                                //�ڱ���û�ҵ� ˵��Ҫ����
                                downLoadList.Add(abName);
                            }
                            //���ֱ�����ͬ��AB��
                            else
                            {
                                //2.�ж� ��Щ��Դ����Ҫ���µ� ��¼ ����
                                //�Ա�MD5�� �ж��Ƿ�Ҫ����
                                if (localABInfo[abName].md5 != remoteABInfo[abName].md5)
                                {
                                    downLoadList.Add(abName);
                                }
                                //������ �Ͳ���Ҫ����
                                //ÿ�μ����ͬһ���ֵ�AB�� �ͰѸ�AB���ӱ����Ƴ�
                                //��ô����ʣ�µ�AB�� ���ǿ���ɾ����
                                localABInfo.Remove(abName);
                            }
                        }
                        updateInfoCallBack("�Ա����");
                        updateInfoCallBack("��ʼɾ�����õ�AB���ļ�");
                        //ɾ�����õ�AB��
                        foreach (string abName in localABInfo.Keys)
                        {
                            //����ɶ�д�ļ����и��ļ� ��ɾ
                            //ֻ���ļ��� ɾ����
                            if(File.Exists(Application.persistentDataPath + "/" + abName))
                            {
                                File.Delete(Application.persistentDataPath + "/" + abName);
                            }
                        }
                        //���ش����µ�����AB��
                        updateInfoCallBack("���غ͸���AB���ļ�");
                        DownLoadABFile((isOver) =>
                        {
                            if (isOver)
                            {
                                //����������AB���ļ���
                                //�ѱ��ص�AB���Ա��ļ�����
                                //��֮ǰ��ȡ������ Զ�˶Ա��ļ��洢������
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


