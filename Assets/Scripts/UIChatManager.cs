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
    [Header("[대화 내용]")]
    [SerializeField] ScrollRect srChat = null; // 대화 목록
    [SerializeField] TextMeshProUGUI txtChat = null; // 대화 내용    
    [SerializeField] TMP_InputField myChat = null; // 대화 입력창    


    ChatClient myChatClient = null;



    void Start()
    {
        if (myChatClient == null)
        {
            myChatClient = new ChatClient(this); // 위에서 선언한게 있기 때문에 this 사용이 가능해진 것
            myChatClient.UseBackgroundWorkerForSending = true; // 백그라운드를 설정해줄지 정할부분
            //myChatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);
            myChatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        }
    }

    public void OnDestroy()
    {
        if (myChatClient != null) // 채팅클라이언트가 있을때 연결끊기가 가능하도록 함
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

    // 채팅창 입력
    public void OnEndEdit(string inStr)
    {
        // 채팅 입력이 아무것도 없으면 리턴 
        if (inStr.Length <= 0)
        {
            return;
        }

        // 포톤채팅으로 내가 입력한 내용을 보내기
        myChatClient.PublishMessage("public", inStr);
        myChat.text = "";

        srChat.verticalNormalizedPosition = 0;


        // inStr 을 채팅목록창에 추가해야 한다.
        addChatLine(PhotonNetwork.NickName, inStr);
    }

    void addChatLine(string userName, string chatLine)
    {
        txtChat.text += $"[{userName}] : {chatLine}\n"; // [내이름] : 대화 내용

    }


    //----------------------------------------------------------------------------------------------------------
    // 포톤채팅 인터페이스 구현 함수
    #region 포톤채팅 인터페이스 구현 함수
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
        addChatLine("[시스템]", "OnConnected");

        // 채널 가입 필요
        myChatClient.Subscribe("public", 0); // 제대로 가입되었으면 해당 함수 호출

        //addChatLine("[시스템]", "OnConnected : "+ state); // 정상적으로 호출되면 시스템이 얘기한 것처럼 보이게 하기 위해서
    }


    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
        addChatLine("[시스템]", "OnConnected");
    }

    // 상대방이 보면 채팅메세지를 받아오는 함수
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
        // 알아보기 쉽게 묶어보기
        addChatLine("[시스템]", string.Format("OnConnected ({0})<{1}>",string.Join(",",channels), string.Join(",", channels)));
    }

    public void OnUnsubscribed(string[] channels) 
    {
        //throw new System.NotImplementedException();
        addChatLine("[시스템]", string.Format("OnConnected ({0})", string.Join(",", channels)));
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
