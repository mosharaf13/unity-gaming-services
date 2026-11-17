using System;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public PlayerDataServiceBindings  m_Bindings;
    public LoginManager   m_LoginManager;
    public string PlayerName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_LoginManager.PlayerSignedIn += InitializePlayer;
        m_Bindings = new PlayerDataServiceBindings(CloudCodeService.Instance);
    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await m_Bindings.SayHello(PlayerName);
            Debug.Log(resultFromCloud);
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }

    }

    private void OnDisable()
    {
        m_LoginManager.PlayerSignedIn -= InitializePlayer;
    }

    public async void SaveNewPlayerName()
    {
        if (!IsPlayerNameValid(PlayerName))
        {
            Debug.LogWarning("PlayerName is not valid");
            return; 
        }
        try
        {
            Debug.Log(PlayerName);
            PlayerName = await m_Bindings.HandleNewPlayerNameEntry(PlayerName); 
            InitializePlayer();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    private bool IsPlayerNameValid(string playerName)
    {
        if (playerName.Length < 4 || playerName.Length > 16)
        {
            return false;
        }

        return true;
    }
}
