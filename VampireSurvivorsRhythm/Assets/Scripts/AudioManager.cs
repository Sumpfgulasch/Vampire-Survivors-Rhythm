using FMOD.Studio;
using UnityEngine;

public class FmodParameter {
    public const string GAME_LOST = "GameLost";
}

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public EventInstance Play3DAudio(FMOD.GUID audioEvent, Transform target) {
        var instance = FMODUnity.RuntimeManager.CreateInstance(audioEvent);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(target));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, target.gameObject);
        instance.start();
        return instance;
    }

    public EventInstance Play2DAudio(FMOD.GUID audioEvent) {
        var instance = FMODUnity.RuntimeManager.CreateInstance(audioEvent);
        instance.start();
        return instance;
    }

    public EventInstance PlaySnapshot(FMOD.GUID snapshot) {
        var instance = FMODUnity.RuntimeManager.CreateInstance(snapshot);
        instance.start();
        return instance;
    }
    
    public void StopSnapshot(EventInstance snapshot) {
        snapshot.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAudio(EventInstance instance) {
        if (instance.isValid()) {
            instance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void SetGlobalParameter(string fmodParameter, float value) {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(fmodParameter, value);
    }

    public void SetLocalParameter(EventInstance fmodEvent, string fmodParameter, float value) {
        if (!fmodEvent.isValid()) {
            return;
        }

        fmodEvent.setParameterByName(fmodParameter, value);
    }
}