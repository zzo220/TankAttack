using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    Transform tr; 
    public float rotSpeed = 100.0f;
    PhotonView pv = null;
    Quaternion currRot = Quaternion.identity;
    // Start is called before the first frame update
    void Awake()
    {
        tr=GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this; //����ȭ ������Ʈ�� TurretCtrl ��ũ��Ʈ�� ����
        // ViewSynchronization.ReliableDeltaCompressed : TCP ���
        // ViewSynchronization.Unreliable : UDP ���
        // ViewSynchronization.UnreliableOnChange : UDP ��� + ��ȭ�� ���� ����
        pv.Synchronization = ViewSynchronization.UnreliableOnChange; //����ȭ �Ӽ� ����
        currRot = tr.localRotation; //�ʱ� ȸ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            //���콺 ��ũ�� �Է°��� �޾ƿ� ĳ�� x���� ȸ��
            float angle = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * rotSpeed;
            tr.Rotate(angle, 0, 0);
        }
        else {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime*3.0f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //�۽�
        {
            stream.SendNext(tr.localRotation); //�ͷ��� ȸ������ �۽�
        }
        else // ����
        {
            //���� ��ũ�� �ͷ� ȸ���� ����
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
