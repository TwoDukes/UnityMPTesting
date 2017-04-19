using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    Player player;

    [SerializeField]
    private float syncTimeDelay = 20f;

    private void Start () {
        player = GetComponent<Player>();
        StartCoroutine(syncScoreLoop());
	}

    private void OnDestroy()
    {
        if(player != null)
            SyncNow();
    }

    IEnumerator syncScoreLoop()
    {
        yield return new WaitForSeconds(syncTimeDelay); // syncs to server every synctimeDelay seconds

        SyncNow();
            
    }

    private void SyncNow()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataRecieved);
        }
    }
	
    private void OnDataRecieved(string data)
    {

        if (player.kills == 0 && player.deaths == 0)
            return;

        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = player.kills + kills;
        int newDeaths = player.deaths + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        Debug.Log("Syncing: " + newData);

        UserAccountManager.instance.SendData(newData);

        player.kills = 0;
        player.deaths = 0;
    }
	
}
