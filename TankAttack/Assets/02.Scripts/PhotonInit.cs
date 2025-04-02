using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public Text logText; //��Ʈ��ũ ���� ǥ�� �ؽ�Ʈ
    public InputField userId; //����� �̸��� �Է¹��� UI ���� ����
    public InputField roomName; // �� �̸��� �Է¹��� UI ���� ����

    public GameObject roomItem; // �� ��ϸ�ŭ ������ ������
    public GameObject scrollContents;  //RoomItem�� ���ϵ�� ������ Parent ��ü

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private void Awake()
    {
        PhotonNetwork.GameVersion = "v1.0";
        if (!PhotonNetwork.IsConnected)//���� ������ �ȵǾ��ִٸ� 
        {
            PhotonNetwork.ConnectUsingSettings(); //���� Ŭ���忡 ����
        }
        userId.text = GetUserId();

        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");

    }
    public override void OnConnectedToMaster()//������ ������ �� �Ǿ��ٸ� ȣ��Ǵ� �ݹ��Լ�
    {
        PhotonNetwork.JoinLobby(); //�κ� ������ �õ�               
    }
    public override void OnJoinedLobby() //�κ� ������ �Ǹ� ȣ��Ǵ� �ݹ��Լ�
    {
        Debug.Log("Entered Lobby");
        userId.text = GetUserId();
        // PhotonNetwork.JoinRandomRoom(); // ������ �� ���� �õ�
    }
    string GetUserId() //���ÿ� ����� �÷��̾� �̸��� ��ȯ �Ǵ� �����ϴ� �Լ�
    {
        string userId = PlayerPrefs.GetString("USER_ID");
        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }
        return userId;
    }
    //������ ������ �õ��� �������� ��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms!!");
        //�� �����
        PhotonNetwork.CreateRoom("My Room", new RoomOptions { MaxPlayers = 20 });
    }
    public override void OnJoinedRoom() //�����ӿ� �����ߴٸ�..
    {
        Debug.Log("Enter Room");
        //  CreateTank(); //��Ʈ��ũ ��ũ ����
        //���� ������ �̵��ϴ� �ڷ�ƾ �Լ� ����
        StartCoroutine(this.LoadBattleField());

    }
    IEnumerator LoadBattleField()
    {
        //���� �̵��ϴ� ���� ����Ŭ���� �����κ��� ��Ʈ��ũ �޽��� ���� �ߴ�
        PhotonNetwork.IsMessageQueueRunning = false;
        //��׶���� �� �ε�
        AsyncOperation ao = Application.LoadLevelAsync("scBattleField");
        yield return ao;
    }
    /*
    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20, pos), Quaternion.identity);
    }*/
    public void OnClickJoinRandomRoom() //Join Random Room ��ư ���� �Լ�
    {
        PhotonNetwork.NickName = userId.text; //�����÷��̾� �̸� ����
        PlayerPrefs.SetString("USER_ID", userId.text); //�÷��̾� �̸��� ����
        PhotonNetwork.JoinRandomRoom();//������ �� ����
    }
    public void OnClickCreateRoom() //Make Room ��ư ���� �Լ�
    {
        string _roomName = roomName.text; //����ڰ� �Է� �������� ����
        if (string.IsNullOrEmpty(roomName.text)) // ����ڰ� �������� �Է� ���ߴٸ�
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000"); //�����ϰ� ������ ����
        }
        PhotonNetwork.NickName = userId.text; //���� �÷��̾� �̸� ����
        PlayerPrefs.SetString("USER_ID", userId.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnCreateRoomFailed(short returnCode, string message) //�游��� ���� ���� �� ȣ��Ǵ� �ݹ��Լ�
    {
        Debug.Log("�� ����� ����: " + message);
    }
    //������ �� ����� ����Ǿ��� �� ȣ��Ǵ� �ݹ��Լ�
    //1. �κ� ���ӽ�
    //2. ���ο� ���� ����� �� �� 
    //3. ���� �����Ǵ� ���
    //4. ���� ������ �ٲ���� ��
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ������ RoomItem �������� ������ �ӽú���
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true) // ���� ������ ��� (�Ӽ�����)
            {  
                // ��ųʸ����� �� �̸����� �˻��� ����� RoomItem �����ո� ����
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom); // RoomItem ������ ����        
                rooms.Remove(roomInfo.Name); // ��ųʸ����� �ش� �� �̸��� �����͸� ���� (������ ���� ���� ���ֱ�!)
                                             //GridLayoutGroup�� constraintCount ���� RoomItem ������ŭ ����
                scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                scrollContents.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 20);
            }
            else // �� ������ ����� ���
            {
                // �� �̸��� ��ųʸ��� ���� ��� ���� �߰�
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject room = (GameObject)Instantiate(roomItem); //RoomItem ������ ���� ����
                    room.transform.SetParent(scrollContents.transform, false); // RoomItem�� scrollContents�� �ڽ����� ����

                    RoomData roomData = room.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name; //������
                    roomData.connectPlayer = roomInfo.PlayerCount; //�����ο���
                    roomData.maxPlayer = roomInfo.MaxPlayers; //�ִ��ο���
                    roomData.DispRoomData();//�ؽ�Ʈ ���� ǥ��

                    roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
                    {
                        OnClickRoomItem(roomData.roomName);
                    });

                    // ��ųʸ� �ڷ����� ������ �߰�
                    rooms.Add(roomInfo.Name, room);
                    //GridLayoutGroup�� constraintCount ���� RoomItem ������ŭ ����
                    scrollContents.GetComponent<GridLayoutGroup>().constraintCount = rooms.Count;
                    //��ũ�� ������ ���̸� ����
                    scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
                }
                else // �� �̸��� ��ųʸ��� ���� ��쿡 �� ������ ����
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom); //��˻��� RoomItem�� tempRoom�� ����

                    RoomData roomData = tempRoom.GetComponent<RoomData>();
                    roomData.roomName = roomInfo.Name; //������
                    roomData.connectPlayer = roomInfo.PlayerCount; //�����ο���
                    roomData.maxPlayer = roomInfo.MaxPlayers; //�ִ��ο���
                    roomData.DispRoomData();//�ؽ�Ʈ ���� ǥ��
                }
            }
        }
    }

    //RoomItem�� Ŭ���ϸ� ȣ��Ǵ� �Լ�
    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.NickName = userId.text; //�� ������ �÷��̾� �̸� ����
        PlayerPrefs.SetString("USER_ID", userId.text); // �÷��̾� �̸� ����
        PhotonNetwork.JoinRoom(roomName); // �� �������� �� ����
    }
    // Update is called once per frame
    void Update()
    {
        logText.text = PhotonNetwork.NetworkClientState.ToString();
    }
}
