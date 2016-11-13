using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();

                if (_instance == null) Debug.LogError("Missing SoundManager");
                else
                {
                    DontDestroyOnLoad(_instance);
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    // General
    public AudioMixer mixer;
    public bool dev_mute_music = true;
    public bool dev_mute_all = true;

    // Volumes (linear)
    public VolumeProduct MasterVolume { get; private set; }
    public VolumeProduct MusicVolume { get; private set; }
    public VolumeProduct WorldVolume { get; private set; }
    public VolumeProduct UIVolume { get; private set; }

    // UI sounds
    public AudioSource btn_click;
    public AudioSource btn_select;



    // PUBLIC ACCESSORS


    // PUBLIC MODIFIERS

    public static void PlayClickSound()
    {
        if (Instance.btn_click != null) Instance.btn_click.Play();
    }
    public static void PlaySelectSound()
    {
        if (Instance.btn_select != null) Instance.btn_select.Play();
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            Initialize();
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void Initialize()
    {
        MasterVolume = new VolumeProduct(mixer, "MasterVolume");
        MusicVolume = new VolumeProduct(mixer, "MusicVolume");
        WorldVolume = new VolumeProduct(mixer, "WorldVolume");
        UIVolume = new VolumeProduct(mixer, "UIVolume");

        if (dev_mute_all) MasterVolume.SetFactor(0, new UID());
        else if (dev_mute_music) MusicVolume.SetFactor(0, new UID());
    }
    private void Start()
    {
        // Force mixer to update volumes (does not work in init if init is called too early?)
        MasterVolume.ForceUpdateMixer();
        MusicVolume.ForceUpdateMixer();
        WorldVolume.ForceUpdateMixer();
        UIVolume.ForceUpdateMixer();
    }


    // HELPERS

    public static void ClearVolMultOnSceneLoad(VolumeProduct vol, UID factor_id)
    {
        GameManager.on_scene_load += () => vol.SetFactor(1, factor_id);
    }
    public static float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

}

public class VolumeProduct : FloatProduct
{
    private System.Action update_func;

    public VolumeProduct(AudioMixer mixer, string exposed_volume_name) : base()
    {
        update_func = () => mixer.SetFloat(exposed_volume_name, SoundManager.LinearToDecibel(Value));
        SetFactor(1);
    }
    public void ForceUpdateMixer()
    {
        update_func();
    }
    protected override void RecalculateValue()
    {
        base.RecalculateValue();
        update_func();
    }
}
