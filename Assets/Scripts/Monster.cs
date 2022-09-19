using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster : MonoBehaviourPun
{
    //constant value
    [SerializeField] float sightOfRange = 15f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float attackPower = 10f;
    [SerializeField] float attackInterval = 3f;
    [SerializeField] float moveSpeed = 4f;

    [Header("[¿Ã∆Â∆Æ]")]
    [SerializeField] GameObject effObj = null;
    [SerializeField] Transform effTransform = null;

    CharacterController monsterCtrl = null;
    Animator monsterAnimotor = null;
    AnimationEventReceiver animationEventReceiver = null;
    PC target;

    private void Awake()
    {
        monsterCtrl = GetComponent<CharacterController>();
        monsterAnimotor = GetComponentInChildren<Animator>();
        animationEventReceiver = GetComponentInChildren<AnimationEventReceiver>();
        animationEventReceiver.callbackAttackEvent = onAttackEvent;
    }

    private void Start()
    {
        StartCoroutine(processAI());   
    }

    IEnumerator processAI()
    {
        while (true)
        {
            yield return null;
            if (PhotonNetwork.IsMasterClient == false) continue;

            if (target == null)
            {
                float curDis = 9999999f;
                float shortestDis = 999999f;

                PC[] listPC = FindObjectsOfType<PC>();

                for (int i = 0; i < listPC.Length; i++)
                {
                    curDis = Vector3.Distance(this.transform.position, listPC[i].transform.position);
                    if (curDis < shortestDis)
                    {
                        target = listPC[i];
                        shortestDis = curDis;
                    }
                }

            }

            if (target != null)
            {
                float curDis = Vector3.Distance(this.transform.position, target.transform.position);
                
                if (curDis < attackRange)
                {
                    monsterAnimotor.SetBool("move", false);
                    monsterAnimotor.SetTrigger("attack");

                    yield return new WaitForSeconds(attackInterval);
                }
                else if (curDis < sightOfRange)
                {
                    monsterAnimotor.SetBool("move", true);

                    Vector3 moveDir = (target.transform.position - this.transform.position).normalized * moveSpeed * Time.deltaTime;

                    if (monsterCtrl == null) Debug.Log("error");

                    monsterCtrl.Move(moveDir);

                    this.transform.rotation = Quaternion.LookRotation(moveDir);
                }
                else 
                {
                    monsterAnimotor.SetBool("move", false);
                    target = null;
                }
            }
        }
    }

    void onAttackEvent()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        if (target != null)
        {
            target.SendMessage("TransferDamage", attackPower, SendMessageOptions.DontRequireReceiver);
        }
    }
    void TransferDamage(float damageValue)
    {
        photonView.RPC("RPC_TransferDamage", RpcTarget.All, damageValue);
    }
    void RPC_TransferDamage(float damageValue)
    {
        monsterAnimotor.SetTrigger("hit");
    }
}
