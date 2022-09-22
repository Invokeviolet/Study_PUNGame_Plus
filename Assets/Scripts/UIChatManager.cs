using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Chat.Demo;
using Photon.Realtime;


public static class AppSettingsExtensions
{
    public static ChatAppSettings GetChatSettings(this AppSettings appSettings)
    {
        return new ChatAppSettings
        {
            AppIdChat = appSettings.AppIdChat,
            AppVersion = appSettings.AppVersion,
            FixedRegion = appSettings.IsBestRegion ? null : appSettings.FixedRegion,
            NetworkLogging = appSettings.NetworkLogging,
            Protocol = appSettings.Protocol,
            EnableProtocolFallback = appSettings.EnableProtocolFallback,
            Server = appSettings.IsDefaultNameServer ? null : appSettings.Server,
            Port = (ushort)appSettings.Port,
            //ProxyServer = appSettings.ProxyServer
            // values not copied from AppSettings class: AuthMode
            // values not needed from AppSettings class: EnableLobbyStatistics 
        };
    }
}



public class UIChatManager : MonoBehaviour, IChatClientListener
{
    [Header("[��ȭ ����]")]
    [SerializeField] ScrollRect srChat = null; // ��ȭ ���
    [SerializeField] TextMeshProUGUI txtChat = null; // ��ȭ ����    
    [SerializeField] TMP_InputField myChat = null; // ��ȭ �Է�â    


    ChatClient myChatClient = null;



    void Start()
    {
        if (myChatClient == null)
        {
            myChatClient = new ChatClient(this); // ������ �����Ѱ� �ֱ� ������ this ����� �������� ��
            myChatClient.UseBackgroundWorkerForSending = true; // ��׶��带 ���������� ���Һκ�
            //myChatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);
            myChatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        }
    }

    public void OnDestroy()
    {
        if (myChatClient != null) // ä��Ŭ���̾�Ʈ�� ������ ������Ⱑ �����ϵ��� ��
        {
            myChatClient.Disconnect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myChatClient != null)
        {
            myChatClient.Service();
        }
       
    }

    // ä��â �Է�
    public void OnEndEdit(string inStr)
    {
        // ä�� �Է��� �ƹ��͵� ������ ���� 
        if (inStr.Length <= 0)
        {
            return;
        }

        // ����ä������ ���� �Է��� ������ ������
        myChatClient.PublishMessage("public", inStr);
        myChat.text = "";

        srChat.verticalNormalizedPosition = 0;


        // inStr �� ä�ø��â�� �߰��ؾ� �Ѵ�.
        addChatLine(PhotonNetwork.NickName, inStr);
    }

    void addChatLine(string userName, string chatLine)
    {
        txtChat.text += $"[{userName}] : {chatLine}\n"; // [���̸�] : ��ȭ ����

    }


    //----------------------------------------------------------------------------------------------------------
    // ����ä�� �������̽� ���� �Լ�
    #region ����ä�� �������̽� ���� �Լ�
    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        //throw new System.NotImplementedException();
        addChatLine("[�ý���]", "OnConnected");

        // ä�� ���� �ʿ�
        myChatClient.Subscribe("public", 0); // ����� ���ԵǾ����� �ش� �Լ� ȣ��

        //addChatLine("[�ý���]", "OnConnected : "+ state); // ���������� ȣ��Ǹ� �ý����� ����� ��ó�� ���̰� �ϱ� ���ؼ�
    }


    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
        addChatLine("[�ý���]", "OnConnected");
    }

    // ������ ���� ä�ø޼����� �޾ƿ��� �Լ�
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        for (int i = 0; i < messages.Length; i++)
        {
            addChatLine(senders[i], messages[i].ToString());
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status,bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
        // �˾ƺ��� ���� �����
        addChatLine("[�ý���]", string.Format("OnConnected ({0})<{1}>",string.Join(",",channels), string.Join(",", channels)));
    }

    public void OnUnsubscribed(string[] channels) 
    {
        //throw new System.NotImplementedException();
        addChatLine("[�ý���]", string.Format("OnConnected ({0})", string.Join(",", channels)));
    }

    public void OnUserSubscribed(string channels,string user) 
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channels, string user)
    {
        //throw new System.NotImplementedException();
    }

    #endregion
    //----------------------------------------------------------------------------------------------------------
}
