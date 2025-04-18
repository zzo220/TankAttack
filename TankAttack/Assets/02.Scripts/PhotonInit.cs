using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public Text logText; //네트워크 상태 표시 텍스트
    public InputField userId; //사용자 이름을 입력받을 UI 연결 변수
    public InputField roomName; // 룸 이름을 입력받을 UI 연결 변수

    public GameObject roomItem; // 룸 목록만큼 생성될 프리팹
    public GameObject scrollContents;  //RoomItem이 차일드로 생성될 Parent 객체

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private void Awake()
    {
        PhotonNetwork.GameVersion = "v1.0";
        if (!PhotonNetwork.IsConnected)//포톤 연결이 안되어있다면 
        {
            PhotonNetwork.ConnectUsingSettings(); //포톤 클라우드에 접속
        }
        userId.text = GetUserId();

        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");

    }
    public override void OnConnectedToMaster()//서버에 접속이 잘 되었다면 호출되는 콜백함수
    {
        PhotonNetwork.JoinLobby(); //로비에 접속을 시도               
    }
    public override void OnJoinedLobby() //로비에 접속이 되면 호출되는 콜백함수
    {
        Debug.Log("Entered Lobby");
        userId.text = GetUserId();
        // PhotonNetwork.JoinRandomRoom(); // 무작위 방 접속 시도
    }
    string GetUserId() //로컬에 저장된 플레이어 이름을 반환 또는 생성하는 함수
    {
        string userId = PlayerPrefs.GetString("USER_ID");
        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }
        return userId;
    }
    //무작위 방접속 시도에 실패했을 때
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms!!");
        //방 만들기
        PhotonNetwork.CreateRoom("My Room", new RoomOptions { MaxPlayers = 20 });
    }
    public override void OnJoinedRoom() //방접속에 성공했다면..
    {
        Debug.Log("Enter Room");
        //  CreateTank(); //네트워크 탱크 생성
        //게임 씬으로 이동하는 코루틴 함수 실행
        StartCoroutine(this.LoadBattleField());

    }
    IEnumerator LoadBattleField()
    {
        //씬을 이동하는 동안 포톨클라우드 서버로부터 네트워크 메시지 수신 중단
        PhotonNetwork.IsMessageQueueRunning = false;
        //백그라운드로 씬 로딩
        AsyncOperation ao = Application.LoadLevelAsync("scBattleField");
        yield return ao;
    }
    /*
    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20, pos), Quaternion.identity);
    }*/
    public void OnClickJoinRandomRoom() //Join Random Room 버튼 연결 함수
    {
        PhotonNetwork.NickName = userId.text; //로컬플레이어 이름 설정
        PlayerPrefs.SetString("USER_ID", userId.text); //플레이어 이름을 저장
        PhotonNetwork.JoinRandomRoom();//무작위 방 입장
    }
    public void OnClickCreateRoom() //Make Room 버튼 연결 함수
    {
        string _roomName = roomName.text; //사용자가 입력 방제목을 얻어옴
        if (string.IsNullOrEmpty(roomName.text)) // 사용자가 방제목을 입력 안했다면
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000"); //랜덤하게 방제목 설정
        }
        PhotonNetwork.NickName = userId.text; //로컬 플레이어 이름 설정
        PlayerPrefs.SetString("USER_ID", userId.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnCreateRoomFailed(short returnCode, string message) //방만들기 실패 했을 때 호출되는 콜백함수
    {
        Debug.Log("방 만들기 실패: " + message);
    }
    //생성된 룸 목록이 변경되었을 때 호출되는 콜백함수
    //1. 로비 접속시
    //2. 새로운 룸이 만들어 질 때 
    //3. 룸이 삭제되는 경우
    //4. 룸의 정보가 바뀌었을 때
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 삭제된 RoomItem 프리팹을 저장할 임시변수
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true) // 룸이 삭제된 경우 (속성지원)
            {  
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹를 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom); // RoomItem 프리팹 삭제        
                rooms.Remove(roomInfo.Name); // 딕셔너리에서 해당 룸 이름의 데이터를 삭제 (데이터 값도 삭제 해주기!)
                                             //GridLayoutGroup의 constraintCount 값을 RoomItem 갯수만큼 증가
                scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                scrollContents.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 20);
            }
            else // 룸 정보가 변경된 경우
            {
                // 룸 이름이 딕셔너리에 없는 경우 새로 추가
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject room = (GameObject)Instantiate(roomItem); //RoomItem 프리팹 동적 생성
                    room.transform.SetParent(scrollContents.transform, false); // RoomItem을 scrollContents의 자식으로 설정

                    RoomData roomData = room.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name; //방제목
                    roomData.connectPlayer = roomInfo.PlayerCount; //현재인원수
                    roomData.maxPlayer = roomInfo.MaxPlayers; //최대인원수
                    roomData.DispRoomData();//텍스트 정보 표시

                    roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
                    {
                        OnClickRoomItem(roomData.roomName);
                    });

                    // 딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, room);
                    //GridLayoutGroup의 constraintCount 값을 RoomItem 갯수만큼 증가
                    scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                    //스크롤 영역의 높이를 증가
                    scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
                }
                else // 룸 이름이 딕셔너리에 없는 경우에 룸 정보를 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom); //룸검색해 RoomItem을 tempRoom에 저장

                    RoomData roomData = tempRoom.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name; //방제목
                    roomData.connectPlayer = roomInfo.PlayerCount; //현재인원수
                    roomData.maxPlayer = roomInfo.MaxPlayers; //최대인원수
                    roomData.DispRoomData();//텍스트 정보 표시
                }
            }
        }
    }

    //RoomItem을 클릭하면 호출되는 함수
    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.NickName = userId.text; //방 접속한 플레이어 이름 설정
        PlayerPrefs.SetString("USER_ID", userId.text); // 플레이어 이름 저장
        PhotonNetwork.JoinRoom(roomName); // 방 제목으로 방 입장
    }
    // Update is called once per frame
    void Update()
    {
        logText.text = PhotonNetwork.NetworkClientState.ToString();
    }
}
