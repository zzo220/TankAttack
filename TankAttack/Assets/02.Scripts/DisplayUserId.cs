using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserId : MonoBehaviour
{
    public Text userId;
    PhotonView pv = null;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>(); //����� ������Ʈ ������ ����
        userId.text = pv.Owner.NickName; //userId �ؽ�Ʈ�� ���� �г��� �Է�
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
