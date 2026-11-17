using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public class GameInitializer : MonoBehaviour
{
    private async void Awake()
    {   
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Unity Services Initializing");
            await UnityServices.InitializeAsync();
        }
        
        EndUserConsent.SetConsentState(new ConsentState {
            AnalyticsIntent = ConsentStatus.Granted,
            AdsIntent = ConsentStatus.Denied
        });
    }
}
