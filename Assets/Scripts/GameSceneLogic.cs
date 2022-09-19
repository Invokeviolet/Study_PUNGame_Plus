using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameSceneLogic : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate("PC", Vector3.zero, Quaternion.identity);

            Vector3 spawnPosition = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
            PhotonNetwork.Instantiate("mon_skeleton", spawnPosition, Quaternion.identity);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("»õ·Î¿î Master : " + newMasterClient.ToString());
    }
}
