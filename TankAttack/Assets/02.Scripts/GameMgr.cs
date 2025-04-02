using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviourPunCallbacks
{
    public Text txtConnect; //�����ο��� ǥ�� �ؽ�Ʈ
    public Text txtLogMsg; //���� �α� ǥ�� �ؽ�Ʈ
    private PhotonView pv; //RPC�Լ� ȣ���� ���� ����� ������Ʈ
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        CreateTank();
        PhotonNetwork.IsMessageQueueRunning = true;
        GetConnectPlayerCount();
    }
    private void Start()
    {
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "] Connected" + "</color>";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20, pos), Quaternion.identity);
    }
    void GetConnectPlayerCount()
    {
        Room currRoom = PhotonNetwork.CurrentRoom; //���� ������ ���� ������ ����
        //�����ο���/�ִ��ο��� ǥ��
        txtConnect.text = currRoom.PlayerCount.ToString() + "/"+currRoom.MaxPlayers.ToString(); ; 
    }
    //���ο� �÷��̾ �濡 �������� �ڵ����� ȣ��Ǵ� �ݹ��Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GetConnectPlayerCount();//���� �����ο��� ����
    }
    // �÷��̾ ���� ������ �� �ڵ����� ȣ��Ǵ� �ݹ��Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();//���� �����ο��� ����
    }
    //EXIT��ư�� ������ ȣ��Ǵ� �Լ�
    public void OnClickExitRoom()
    {

        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "] Disconnected" + "</color>";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

        PhotonNetwork.LeaveRoom();
    }
    //�� �����Ⱑ �����ϸ� ȣ��Ǵ� �ݹ��Լ�
    public override void OnLeftRoom()
    {
        Application.LoadLevel("scLobby");
    }
    [PunRPC]
    void LogMsg(string msg) //�α� �޽��� Text UI�� �ؽ�Ʈ�� �������� ǥ��
    {
        txtLogMsg.text = txtLogMsg.text + msg;
    }
}
