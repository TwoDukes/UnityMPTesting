using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayer = "DontDraw";
    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;


    private void Start()
    {
        if (!isLocalPlayer) //if not local player
        {
            DisableComponenets();
            AssignRemoteLayer();
        }
        else //if local player
        {

            //Disable player graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayer));

            //Create player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //configure player UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("no PlayerUI component on PlayerUI prefab");
            ui.SetPlayerController(GetComponent<PlayerController>());

            GetComponent<Player>().SetupPlayer(); //Sets up local players health and stores active components to be disabled on death
        }

       
    }

    private void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void DisableComponenets()
    {
        foreach (Behaviour item in componentsToDisable) //disable all remote player control scripts
        {
            item.enabled = false;
        }
    }

    private void OnDisable() //basically a destructor
    {
        Destroy(playerUIInstance);

        if(isLocalPlayer)
        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

}
