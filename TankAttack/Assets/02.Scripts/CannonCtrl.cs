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
        pv.ObservedComponents[0] = this; //동기화 컴포넌트를 TurretCtrl 스크립트로 지정
        // ViewSynchronization.ReliableDeltaCompressed : TCP 방식
        // ViewSynchronization.Unreliable : UDP 방식
        // ViewSynchronization.UnreliableOnChange : UDP 방식 + 변화가 있을 때만
        pv.Synchronization = ViewSynchronization.UnreliableOnChange; //동기화 속성 설정
        currRot = tr.localRotation; //초기 회전값 설정
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            //마우스 스크롤 입력값을 받아와 캐논 x값을 회전
            float angle = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * rotSpeed;
            tr.Rotate(angle, 0, 0);
        }
        else {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime*3.0f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //송신
        {
            stream.SendNext(tr.localRotation); //터렛의 회전정보 송신
        }
        else // 수신
        {
            //원격 탱크의 터렛 회전값 수신
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
