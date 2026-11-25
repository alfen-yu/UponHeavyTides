using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

[DefaultExecutionOrder(-100)]
public class UGS_Analytics : MonoBehaviour
{
    private static UGS_Analytics instance;

    public static UGS_Analytics Instance
    {
        get
        {
            if (instance == null)
            {
                var existing = FindFirstObjectByType<UGS_Analytics>();
                if (existing != null)
                {
                    instance = existing;
                }
                else
                {
                    var go = new GameObject("UGS_Analytics");
                    instance = go.AddComponent<UGS_Analytics>();
                }
            }
            return instance;
        }
    }

    private static bool isInitialized = false;

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeSDK();
    }

    public static async System.Threading.Tasks.Task InitializeSDK()
    {
        if (isInitialized) return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            // Grant analytics consent via the modern Unity Developer Data framework
            try
            {
                var consentState = new ConsentState { AnalyticsIntent = ConsentStatus.Granted };
                EndUserConsent.SetConsentState(consentState);
            }
            catch (Exception consentEx)
            {
                Debug.LogWarning($"[UGS Analytics] Consent setup notice: {consentEx.Message}");
            }

#pragma warning disable CS0618
            try
            {
                AnalyticsService.Instance.StartDataCollection();
            }
            catch
            {
                // StartDataCollection fallback if needed
            }
#pragma warning restore CS0618

            isInitialized = true;
            Debug.Log("<color=#00FFCC>[UGS Analytics]</color> Initialized successfully and data collection started.");
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError($"<color=#FF5555>[UGS Analytics]</color> Failed to initialize: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=#FF5555>[UGS Analytics]</color> Error during initialization: {e.Message}");
        }
    }

    private static async System.Threading.Tasks.Task SendEventWhenReady(string eventName, CustomEvent customEvent, string debugInfo)
    {
        if (!isInitialized)
        {
            await InitializeSDK();
        }

        if (isInitialized)
        {
            try
            {
                AnalyticsService.Instance.RecordEvent(customEvent);
                AnalyticsService.Instance.Flush();
                Debug.Log($"<color=#00FFCC>[UGS Analytics Event]</color> <b>{eventName}</b> recorded! ({debugInfo})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[UGS Analytics] Could not record '{eventName}': {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"[UGS Analytics] Failed to record '{eventName}': Analytics not initialized.");
        }
    }

    /// <summary>
    /// Custom Event 1: Records when a player is caught by a predator in the Kelp Maze.
    /// Event Name: PlayerCaught
    /// Parameter: CatchNumber (int)
    /// </summary>
    public static void RecordPlayerCaught(int catchNumber)
    {
        var customEvent = new CustomEvent("PlayerCaught")
        {
            { "CatchNumber", catchNumber }
        };

        if (isInitialized)
        {
            try
            {
                AnalyticsService.Instance.RecordEvent(customEvent);
                AnalyticsService.Instance.Flush();
                Debug.Log($"<color=#00FFCC>[UGS Analytics Event]</color> <b>PlayerCaught</b> recorded! (CatchNumber = {catchNumber})");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[UGS Analytics] Could not record 'PlayerCaught': {ex.Message}");
            }
        }
        else
        {
            _ = SendEventWhenReady("PlayerCaught", customEvent, $"CatchNumber = {catchNumber}");
        }
    }

    /// <summary>
    /// Custom Event 2: Records time taken to complete Level 1 when returning the shell to the Hermit Crab.
    /// Event Name: TimeToCompletion
    /// Parameter: DurationSeconds (float)
    /// </summary>
    public static void RecordTimeToCompletion(float durationSeconds)
    {
        var customEvent = new CustomEvent("TimeToCompletion")
        {
            { "DurationSeconds", durationSeconds }
        };

        if (isInitialized)
        {
            try
            {
                AnalyticsService.Instance.RecordEvent(customEvent);
                AnalyticsService.Instance.Flush();
                Debug.Log($"<color=#00FFCC>[UGS Analytics Event]</color> <b>TimeToCompletion</b> recorded! (DurationSeconds = {durationSeconds:F1}s)");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[UGS Analytics] Could not record 'TimeToCompletion': {ex.Message}");
            }
        }
        else
        {
            _ = SendEventWhenReady("TimeToCompletion", customEvent, $"DurationSeconds = {durationSeconds:F1}s");
        }
    }
}
