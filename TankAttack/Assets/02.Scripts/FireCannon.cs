using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireCannon : MonoBehaviourPun
{
    public GameObject cannon = null;
    public Transform firePos;
    AudioClip fireSfx = null;
    AudioSource sfx = null;
    PhotonView pv = null;
    private void Awake()
    {
        cannon = (GameObject)Resources.Load("Cannon");
        fireSfx = Resources.Load<AudioClip>("CannonFire");
        sfx = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //마우스커서가 Canvas UI 항목위에 있으면 Update() 종료
        if (MouseHover.instance.isUIHover) return;

        if (pv.IsMine && Input.GetMouseButtonDown(0)) //왼쪽 마우스 버튼을 눌렀다면
        {
            Fire(); //Fire() 호출
            //원격 네트워크 탱크에 RPC로 원격 Fire() 호출
            pv.RPC("Fire", RpcTarget.Others, null);
        }
    }
    [PunRPC]
    void Fire()
    {
        sfx.PlayOneShot(fireSfx, 1.0f); //발사 사운드 실행
        //cannon 프리팹 생성
        Instantiate(cannon, firePos.position, firePos.rotation);
    }
}
