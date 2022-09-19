using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PC : MonoBehaviourPun, IPunObservable
{
    //components
	CharacterController playerCtrl = null;
    Animator playerAnimator = null;
    HpBarInfo playerHpInfo = null;
    AnimationEventReceiver playerAnimationEventReceiver = null;

    //constant value
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 150f;
    [SerializeField] float attackRange = 5f;

    //variable value
    float xAxis;
    float zAxis;

    int itemCount = 0;
    float exp = 0f;

    private void Awake()
    {

        //get components
        playerCtrl = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        playerHpInfo = GetComponentInChildren<HpBarInfo>();
        playerAnimationEventReceiver = GetComponentInChildren<AnimationEventReceiver>();
        playerAnimationEventReceiver.callbackAttackEvent = onAttackEvent;

        //initializing var value
        xAxis = 0f;
        zAxis = 0f;
    }

    private void Start()
    {
        //mouse cursor lock
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)//check view is mine
        {
            FollowCamera playerCamera = FindObjectOfType<FollowCamera>();
            playerCamera.UpdateTarget(this.transform);
        }
        playerHpInfo.SetName(photonView.Controller.NickName + " " + itemCount);
    }


    private void Update()
    {
        if (!photonView.IsMine) return;

        StartCoroutine(OnCtrlUsed());

        playerAnimator.SetBool("Move", (xAxis != 0f || zAxis != 0f));
        playerAnimator.SetFloat("xAxis", xAxis);
        playerAnimator.SetFloat("zAxis", zAxis);

        //move
        Vector3 moveDir = (transform.forward * zAxis + transform.right * xAxis )* Time.deltaTime * moveSpeed;
        playerCtrl.Move(moveDir);

        //rotation
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(0f, mouseX * rotationSpeed * Time.deltaTime , 0f);

        if (Input.GetButtonDown("Fire1") && xAxis == 0f && zAxis == 0f )
        {
            playerAnimator.SetTrigger("Attack");
            itemCount++;
        }

    }

    IEnumerator OnCtrlUsed()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            xAxis = Input.GetAxis("Horizontal");
            zAxis = Input.GetAxis("Vertical");

            if (xAxis != 0f)
            {
                if (xAxis > 0f)
                {
                    xAxis = Mathf.Lerp(xAxis, 0.3f, Time.deltaTime);
                }
                else
                {
                    xAxis = Mathf.Lerp(xAxis, -0.3f, Time.deltaTime);
                }
                xAxis = Input.GetAxis("Horizontal") * 0.3f;
            }
            if (zAxis != 0f)
            {
                if (zAxis > 0f)
                {
                    zAxis = Mathf.Lerp(zAxis, 0.3f, Time.deltaTime);
                }
                else
                {
                    zAxis = Mathf.Lerp(zAxis, -0.3f, Time.deltaTime);
                }
                zAxis = Input.GetAxis("Vertical") * 0.3f;
            }
        }
        else
        {
            xAxis = Input.GetAxis("Horizontal");
            zAxis = Input.GetAxis("Vertical");
        }

        yield return null;
    }

    //call when syncronized
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//other client user send data to me
        {
            stream.SendNext(itemCount);
            stream.SendNext(exp);
        }
        else//receive data
        {
            itemCount = (int)stream.ReceiveNext();
            exp = (int)stream.ReceiveNext();

        }
    }

    void onAttackEvent()
    {
        PC[] listPc = FindObjectsOfType<PC>();

        for (int i = 0; i < listPc.Length; i++)
        {
            if (listPc[i].photonView.IsMine) continue;

            if (Vector3.Distance(listPc[i].transform.position, this.transform.position) <= attackRange)
            {
                listPc[i].photonView.RPC("TransferDamage", RpcTarget.All, 10);    
            }
        }

        Monster[] listMonster = FindObjectsOfType<Monster>();
        for (int i = 0; i < listMonster.Length; i++)
        {
            if (Vector3.Distance(listMonster[i].transform.position, this.transform.position) <= attackRange)
            {
                listMonster[i].SendMessage("TransferDamage", 10f, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    float totalDamage;

    [PunRPC]
    void TransferDamage(float damageValue)
    {
        photonView.RPC("RPC_TransferDamage", RpcTarget.All, damageValue);
    }
    void RPC_TransferDamage(float damageValue)
    {
        totalDamage += damageValue;

        playerHpInfo.SetName(photonView.Controller.NickName + " " + totalDamage.ToString());
    }
}
