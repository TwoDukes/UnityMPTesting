using UnityEngine.Networking;
using UnityEngine;

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
        RegisterPlayer(); //Registers player
    }

    void RegisterPlayer()
    {
        string _ID = "Player" + GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
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
    }

}
