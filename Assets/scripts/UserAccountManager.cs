using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    //These store the username and password of the player when they have logged in
    public static string PlayerUsername { get; protected set; }
    private static string PlayerPassword = "";
    public static string LoggedInData { get; protected set; }

    public static bool IsLoggedIn {get; protected set;}

    public string loggedInSceneName = "Lobby";
    public string loggedOutSceneName = "LoginMenu";


    public void LogOut()
    {
        PlayerUsername = "";
        PlayerPassword = "";

        IsLoggedIn = false;

        Debug.Log(PlayerUsername + " logged out");

        SceneManager.LoadScene(loggedOutSceneName);
    }

    public void LogIn(string username, string password)
    {
        PlayerUsername = username;
        PlayerPassword = password;

        IsLoggedIn = true;

        Debug.Log("Logged in as: " + PlayerUsername);

        SceneManager.LoadScene(loggedInSceneName);
    }


    IEnumerator GetData()
    {
        IEnumerator e = DCF.GetUserData(PlayerUsername, PlayerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.

            instance.LogOut();
        }
        else
        {
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            LoggedInData = response;
        }
    }
    IEnumerator SetData(string data)
    {
        IEnumerator e = DCF.SetUserData(PlayerUsername, PlayerPassword, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //The data string was set correctly. Goes back to LoggedIn UI

        }
        else
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
            instance.LogOut();
        }
    }

    public void SendData(string data)
    {
        if(IsLoggedIn)
        {
            StartCoroutine(SetData(data));
        }
    }
    public void RetrieveData()
    {

        if(IsLoggedIn)
        {
            //Called when the player hits 'Get Data' to retrieve the data string on their account. Switches UI to 'Loading...' and starts coroutine to get the players data string from the server
            StartCoroutine(GetData());
        }
        
    }
}
