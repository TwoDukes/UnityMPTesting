using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

    public Text usernameText;

    private void Start()
    {
        if(UserAccountManager.IsLoggedIn)
            usernameText.text = "Logged in As: " + UserAccountManager.PlayerUsername;
    }

    public void LogOutButtonPressed()
    {
        if(UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
