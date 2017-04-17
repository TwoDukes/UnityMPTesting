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
    private GameObject playerUIInstance;


    Camera sceneCamera;

    private void Start()
    {
        if (!isLocalPlayer) //if not local player
        {
            DisableComponenets();
            AssignRemoteLayer();
        }
        else //if local player
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false); // turn off lobby camera
            }
            //Disable player graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayer));

            //Create player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //configure player UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("no PlayerUI component on PlayerUI prefab");
            ui.SetPlayerConstroller(GetComponent<PlayerController>());
        }

        GetComponent<Player>().Setup(); //Sets up players health and stores active components to be disabled on death
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

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true); //turns on lobby camera
        }

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
