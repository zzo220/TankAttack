using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    Transform tr;
    RaycastHit hit; //ray�� ���鿡 ���� ��ġ�� ������ ����
    public float rotSpeed = 5.0f; //�ͷ��� ȸ�� �ӵ�
    PhotonView pv = null;
    Quaternion currRot = Quaternion.identity;
    // Start is called before the first frame update
    void Awake()
    {
        tr = GetComponent<Transform>(); 
        pv = GetComponent<PhotonView>(); //����� ������Ʈ �Ҵ�
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
        if (pv.IsMine) //������ ��
        {
            //����ī�޶󿡼� ���콺 Ŀ���� ��ġ�� ĳ���õǴ� Ray�� ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //������ Ray�� Scene�信�� ��� �������� ǥ��
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);

            //8��° ���̾ �ش��ϴ� ������Ʈ�� Ray�� �ε����ٸ�
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
            {
                //Ray�� ���� ��ġ�� ������ǥ�� ��ȯ
                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //��ź��Ʈ �Լ��� Atan2�� �� �� ���� ������ ���
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                //rotSpeed ������ ������ �ӵ��� ȸ��
                tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
            }
        }
        else //����Ʈ�� ��
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime * 3.0f);
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
