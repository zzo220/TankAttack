using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using Photon.Pun;

public class TankMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 20.0f; //이동 속도
    public float rotSpeed = 50.0f; // 회전 속도

    Rigidbody rbody; //리지드바디 컴포넌트
    Transform tr; // 트랜스폼 컴포넌트
    float h, v; // 키보드 입력 값 변수
    PhotonView pv = null; //포톤뷰 컴포넌트
    public Transform camPivot; // 카메라가 추적할 CamPivot

    Vector3 currPos = Vector3.zero; //동기화할 위치 정보
    Quaternion currRot = Quaternion.identity; //동기화할 회전 정보

    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        rbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f); //무게 중심을 낮게 설정
        pv = GetComponent<PhotonView>();
        //데이터 전송 타입을 설정
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        //PhotonView Obserced Components 속성에 TankMove 스크립트를 연결
        pv.ObservedComponents[0] = this;

        if (pv.IsMine) //내꺼인지 아닌지? local or remote
        {
            Camera.main.GetComponent<SmoothFollow>().target = camPivot;
            rbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f);
        }
        else //원격(리모트)
        {
            rbody.isKinematic = true; //원격 탱크의 물리력을 이용하지 않음
        }
        //원격 탱크의 위치 및 회전 값을 처리할 변수 설정
        currPos = tr.position;
        currRot = tr.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!pv.IsMine) return; //로컬이 아니라면 함수 종료
        if (pv.IsMine)
        {
            h = Input.GetAxis("Horizontal"); //좌우 (회전)
            v = Input.GetAxis("Vertical"); //위아래 (이동)
                                           //회전과 이동 처리
            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        }
        else
        {
            //원격 탱크의 위치와 회전을 부드럽게 처리함
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 3.0f);
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 3.0f);
        }
            
       
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 송신 
        {
            stream.SendNext(tr.position); // 위치 정보 송신
            stream.SendNext(tr.rotation); // 회전 정보 송신
        }
        else //수신 
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
