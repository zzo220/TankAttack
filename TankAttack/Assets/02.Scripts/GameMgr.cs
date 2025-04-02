using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviourPunCallbacks
{
    public Text txtConnect; //접속인원수 표시 텍스트
    public Text txtLogMsg; //접속 로그 표시 텍스트
    private PhotonView pv; //RPC함수 호출을 위한 포톤뷰 컴포넌트
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
        Room currRoom = PhotonNetwork.CurrentRoom; //현재 입장한 룸의 정보를 얻어옴
        //현재인원수/최대인원수 표시
        txtConnect.text = currRoom.PlayerCount.ToString() + "/"+currRoom.MaxPlayers.ToString(); ; 
    }
    //새로운 플레이어가 방에 들어왔을때 자동으로 호출되는 콜백함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GetConnectPlayerCount();//현재 접속인원을 갱신
    }
    // 플레이어가 방을 나갔을 때 자동으로 호출되는 콜백함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();//현재 접속인원을 갱신
    }
    //EXIT버튼을 누르면 호출되는 함수
    public void OnClickExitRoom()
    {

        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "] Disconnected" + "</color>";
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

        PhotonNetwork.LeaveRoom();
    }
    //방 나가기가 성공하면 호출되는 콜백함수
    public override void OnLeftRoom()
    {
        Application.LoadLevel("scLobby");
    }
    [PunRPC]
    void LogMsg(string msg) //로그 메시지 Text UI에 텍스트를 누적시켜 표시
    {
        txtLogMsg.text = txtLogMsg.text + msg;
    }
}
