using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    Transform tr;
    RaycastHit hit; //ray가 지면에 맞은 위치를 저장할 변수
    public float rotSpeed = 5.0f; //터렛의 회전 속도
    PhotonView pv = null;
    Quaternion currRot = Quaternion.identity;
    // Start is called before the first frame update
    void Awake()
    {
        tr = GetComponent<Transform>(); 
        pv = GetComponent<PhotonView>(); //포톤뷰 컴포넌트 할당
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
        if (pv.IsMine) //로컬일 때
        {
            //메인카메라에서 마우스 커서의 위치로 캐스팅되는 Ray를 생성
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //생성된 Ray를 Scene뷰에서 녹색 광선으로 표현
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);

            //8번째 레이어에 해당하는 오브젝트에 Ray가 부딪혔다면
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
            {
                //Ray에 맞은 위치를 로컬좌표로 변환
                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //역탄젠트 함수인 Atan2로 두 점 간의 각도를 계산
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                //rotSpeed 변수에 지정된 속도로 회전
                tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
            }
        }
        else //리모트일 때
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime * 3.0f);
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
