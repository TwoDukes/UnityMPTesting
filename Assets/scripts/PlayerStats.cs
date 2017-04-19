using UnityEngine.UI;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public Text killCount;
    public Text deathCount;

    private void Start()
    {
        if(UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.GetData(OnRecievedData);//sending OnRecievedData as a callback
    }

    private void OnRecievedData(string data)
    {
        killCount.text = "Total Kills: " + DataTranslator.DataToKills(data).ToString();
        deathCount.text = "Total Deaths: " + DataTranslator.DataToDeaths(data).ToString();
    }

}
