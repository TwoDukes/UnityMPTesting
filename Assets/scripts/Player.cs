using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    #region Variables
    [SyncVar]
    private bool isDead = false; //cannot mark props as syncVar
    public bool _isDead          //This is why we have a var and a prop
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }



    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameobjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    #endregion
    //private void Update()
    //{
    //        if (!isLocalPlayer)
    //            return;

    //        if (Input.GetKeyDown(KeyCode.K))
    //        {
    //            RpcTakeDamage(99999);
    //        }
    //}

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
        }

        deaths++;

        //disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false; //sets disabled all behaviours in disableOnDeath
        }

        for (int i = 0; i < disableGameobjectsOnDeath.Length; i++)
        {
            disableGameobjectsOnDeath[i].SetActive(false); //sets disabled all GameObjects in disableGameobjectsOnDeath
        }

        //disable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //spawn death effect
        GameObject _gfxIns = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        //switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }


        //CALL RESPAWN METHOD
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.2f); //Allows player to move before spawn effects

        SetupPlayer();

        Debug.Log(transform.name + "Respawned");
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }
    
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled; //stores which behaviours are enabled on start
            }
            firstSetup = false;
        }
        

        SetDefaults();
    }

    //////////////////////////////////////////////////////////////////////PUBLIC CLASS METHODS
    public void SetupPlayer()
    {  
        if (isLocalPlayer)
        {
            //switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBroadcastNewPlayerSetup();
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i]; //sets enabled all behaviours based on previously stored array (wasEnabled)
        }

        for (int i = 0; i < disableGameobjectsOnDeath.Length; i++)
        {
            disableGameobjectsOnDeath[i].SetActive(true); //sets enabled all GameObjects in disableGameobjectsOnDeath
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true; //Colliders need to be set enabled/disabled seperately as they are not derived from the "Behavior" class
        }

        //spawn "spawn" effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }

    [ClientRpc] //Can be invoked by server
    public void RpcTakeDamage(int _amount, string _sourceID)
    {
        if(isDead)
        {
            return;
        }

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health");

        if(currentHealth <= 0)
        {
            Die(_sourceID);
        }
    }

}
