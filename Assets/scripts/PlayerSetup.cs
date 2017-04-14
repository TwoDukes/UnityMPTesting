using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

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
