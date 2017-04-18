using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    private uint roomSize = 2;

    private string roomName;

    private string roomPassword = "";

    private NetworkManager networkManager;

    [SerializeField]
    private Slider roomSizeInputSlider;

    [SerializeField]
    private Text roomSizeText;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void SetRoomSize()
    {
        roomSize = (uint)roomSizeInputSlider.value;

        roomSizeText.text = "Maximum number of players: " + roomSize;

    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating room: " + roomName + " with room for " + roomSize + " Players");
            //Create room
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, roomPassword, "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }

}
