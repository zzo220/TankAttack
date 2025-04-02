using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using Photon.Pun;

public class TankMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 20.0f; //�̵� �ӵ�
    public float rotSpeed = 50.0f; // ȸ�� �ӵ�

    Rigidbody rbody; //������ٵ� ������Ʈ
    Transform tr; // Ʈ������ ������Ʈ
    float h, v; // Ű���� �Է� �� ����
    PhotonView pv = null; //����� ������Ʈ
    public Transform camPivot; // ī�޶� ������ CamPivot

    Vector3 currPos = Vector3.zero; //����ȭ�� ��ġ ����
    Quaternion currRot = Quaternion.identity; //����ȭ�� ȸ�� ����

    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        rbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f); //���� �߽��� ���� ����
        pv = GetComponent<PhotonView>();
        //������ ���� Ÿ���� ����
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        //PhotonView Obserced Components �Ӽ��� TankMove ��ũ��Ʈ�� ����
        pv.ObservedComponents[0] = this;

        if (pv.IsMine) //�������� �ƴ���? local or remote
        {
            Camera.main.GetComponent<SmoothFollow>().target = camPivot;
            rbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f);
        }
        else //����(����Ʈ)
        {
            rbody.isKinematic = true; //���� ��ũ�� �������� �̿����� ����
        }
        //���� ��ũ�� ��ġ �� ȸ�� ���� ó���� ���� ����
        currPos = tr.position;
        currRot = tr.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!pv.IsMine) return; //������ �ƴ϶�� �Լ� ����
        if (pv.IsMine)
        {
            h = Input.GetAxis("Horizontal"); //�¿� (ȸ��)
            v = Input.GetAxis("Vertical"); //���Ʒ� (�̵�)
                                           //ȸ���� �̵� ó��
            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        }
        else
        {
            //���� ��ũ�� ��ġ�� ȸ���� �ε巴�� ó����
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 3.0f);
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 3.0f);
        }
            
       
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // �۽� 
        {
            stream.SendNext(tr.position); // ��ġ ���� �۽�
            stream.SendNext(tr.rotation); // ȸ�� ���� �۽�
        }
        else //���� 
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
