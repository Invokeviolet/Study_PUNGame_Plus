using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI textConnectionInfo = null;
    [SerializeField] Button connectButton = null;

    void Start()
    {
        connectButton.interactable = false;
        //require connetion to master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        textConnectionInfo.text = "�����Ϳ� ����Ǿ����ϴ�.";
        connectButton.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        textConnectionInfo.text = "�����Ϳ� ������ ������ϴ�.";
        //connectButton.interactable = false;
    }

    public void OnValueChanged(string inStr)
    {
        PhotonNetwork.NickName = inStr;
    }

    public void OnClick_Connect()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.NickName)) return;

        PhotonNetwork.JoinOrCreateRoom("myroom", new RoomOptions { MaxPlayers = 100 }, null);
    }

    //failure connecting
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        textConnectionInfo.text = $"���ӽ���... <{returnCode}>{message}";
    }
    //success connecting
    public override void OnJoinedRoom()
    {
        textConnectionInfo.text = "���� ����";

        PhotonNetwork.LoadLevel("GameScene");
    }
}
