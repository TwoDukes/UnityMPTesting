using UnityEngine.Networking;//The Unity High Level API (HLAPI)
using UnityEngine;

public class PlayerSetup : NetworkBehaviour {



    //List of components to disable to avoid duplicates in networking.
    //All components in Unity derive from Behavior.
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    private void Start()
    {
        if(!isLocalPlayer) //if not local player
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else //if local player
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false); // turn off lobby camera
            }
        }
    }

    private void OnDisable() //basically a destructor
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true); //turns on lobby camera
        }
    }
    private void DisableComponents() //disable all player control scripts
    {
        foreach (Behaviour item in componentsToDisable) 
        {
            item.enabled = false;
        }
    }
    //Takes the layer attribute of the Game Object and replaces it with the new layerName
    private void AssignRemoteLayer() 
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

}
