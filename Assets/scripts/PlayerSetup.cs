using UnityEngine.Networking;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDiable;

    Camera sceneCamera;

    private void Start()
    {
        if(!isLocalPlayer)
        {
            foreach(Behaviour item in componentsToDiable)
            {
                item.enabled = false;
            }
        }
        else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
            
    }

    private void OnDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

}
