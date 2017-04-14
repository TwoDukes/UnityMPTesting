using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

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

    [SerializeField]
    Behaviour[] disableOnDeath;
    private bool[] wasEnabled;



    //private void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if(Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(99999);
    //    }
    //}

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false; //sets disabled all behaviours in disableOnDeath
        }

        Debug.Log(transform.name + " is DEAD!");

        //CALL RESPAWN METHOD
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + "Respawned");
    }


    //////////////////////////////////////////////////////////////////////PUBLIC CLASS METHODS
    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled; //stores which behaviours are enabled on start
        }

        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i]; //sets enabled all behaviours based on previously stored array (wasEnabled)
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true; //Colliders need to be set enabled/disabled seperately as they are not derived from the "Behavior" class
        }
    }

    [ClientRpc] //Can be invoked by server
    public void RpcTakeDamage(int _amount)
    {
        if(isDead)
        {
            return;
        }

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

}
