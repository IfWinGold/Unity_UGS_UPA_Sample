using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;

public class CloudSaveSample : MonoBehaviour
{
   private const string CLOUD_SAVE_LEVEL_KEY = "level";
    private const string CLOUD_SAVE_PET_NAME_KEY = "pet_name";

    // Start is called before the first frame update
    void Start()
    {
        //SetupAndSignIn();
    }

    // This part of the code should be done at the beginning of your game flow (i.e. Main Menu)
    async void SetupAndSignIn() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            SaveDataWithErrorHandling();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            LoadDataWithErrorHandling();
        }
    }

    // SaveData method as in the tutorial
    async void SaveData() {
        var data = new Dictionary<string, object>{ 
            {"level", 1}, 
            {"pet_name", "Good Boy"} 
        };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        Debug.Log("Attempted to save data");
    }

    // LoadData method as in the tutorial
    async void LoadData() {
        var keysToLoad = new HashSet<string> {
            "level",
            "pet_name"
        };
        var loadedData = await CloudSaveService.Instance.Data.LoadAsync(keysToLoad);
        var loadedLevel = loadedData["level"];
        var loadedPetName = loadedData["pet_name"];
        Debug.Log("Loaded data. Level: " + loadedLevel + ", Pet name: " + loadedPetName);
    }

    async void SaveDataWithErrorHandling() {
        var data = new Dictionary<string, object>{ 
            {CLOUD_SAVE_LEVEL_KEY, 1}, 
            {CLOUD_SAVE_PET_NAME_KEY, "Good Boy"} 
        };
        try {
            Debug.Log("Attempting to save data...");
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Save data success!");
        } catch (ServicesInitializationException e) {
            // service not initialized
            Debug.LogError(e);
        } catch (CloudSaveValidationException e) {
            // validation error
            Debug.LogError(e);
        } catch (CloudSaveRateLimitedException e) {
            // rate limited
            Debug.LogError(e);
        } catch (CloudSaveException e) {
            Debug.LogError(e);
        }

    }

    async void LoadDataWithErrorHandling() {
        var keysToLoad = new HashSet<string> {
            CLOUD_SAVE_LEVEL_KEY,
            CLOUD_SAVE_PET_NAME_KEY
        };
        try {
            var loadedData = await CloudSaveService.Instance.Data.LoadAsync(keysToLoad);

            if (loadedData.TryGetValue(CLOUD_SAVE_LEVEL_KEY, out var loadedLevel)) {
                Debug.Log("Loaded saved level: " + loadedLevel);
            } else {
                Debug.Log("Level not found in cloud save");
            }

            if (loadedData.TryGetValue(CLOUD_SAVE_PET_NAME_KEY, out var loadedPetName)) {
                Debug.Log("Loaded saved pet name: " + loadedPetName);
            } else {
                Debug.Log("Pet name not found in cloud save");
            }
        } catch (ServicesInitializationException e) {
            // service not initialized
            Debug.LogError(e);
        } catch (CloudSaveValidationException e) {
            // validation error
            Debug.LogError(e);
        } catch (CloudSaveRateLimitedException e) {
            // rate limited
            Debug.LogError(e);
        } catch (CloudSaveException e) {
            Debug.LogError(e);
        }
    }
}