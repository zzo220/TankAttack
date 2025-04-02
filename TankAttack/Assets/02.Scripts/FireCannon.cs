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
        //���콺Ŀ���� Canvas UI �׸����� ������ Update() ����
        if (MouseHover.instance.isUIHover) return;

        if (pv.IsMine && Input.GetMouseButtonDown(0)) //���� ���콺 ��ư�� �����ٸ�
        {
            Fire(); //Fire() ȣ��
            //���� ��Ʈ��ũ ��ũ�� RPC�� ���� Fire() ȣ��
            pv.RPC("Fire", RpcTarget.Others, null);
        }
    }
    [PunRPC]
    void Fire()
    {
        sfx.PlayOneShot(fireSfx, 1.0f); //�߻� ���� ����
        //cannon ������ ����
        Instantiate(cannon, firePos.position, firePos.rotation);
    }
}
