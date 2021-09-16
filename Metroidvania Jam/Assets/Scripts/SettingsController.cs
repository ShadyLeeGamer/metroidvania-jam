using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{

	// Attach to canvas. Deals with reading / writing settings,
	// // displaying them on UI, etc.

	public Slider masterVolume;
	public Slider musicVolume;
	public Slider sfxVolume;
	public Slider uiScale;
	public Toggle fullscreen;

	CanvasScaler cs;
    void Start() {
    	cs = GetComponent<CanvasScaler>();
        Camera.main.GetComponent<CameraController>().sc = this;

    	FindAudio();
        // unimplemented rw from .txt
        // if (Settings.unread) Settings.Read();
    	Revert();
        Save();
    }
    List<AudioSource> music = new List<AudioSource>();
    List<AudioSource> sfx = new List<AudioSource>();
    void FindAudio() {
    	music.Clear();
    	sfx.Clear();
    	GameObject[] sources = GameObject.FindGameObjectsWithTag("Music");
    	for (int i = 0; i < sources.Length; i++)
    		music.Add(sources[i].GetComponent<AudioSource>());
    	sources = GameObject.FindGameObjectsWithTag("SFX");
    	for (int i = 0; i < sources.Length; i++)
    		sfx.Add(sources[i].GetComponent<AudioSource>());
    }
    
    // General purpose
    public void Save() {
        WriteUI();
        ApplyAll();
    }
    public void Revert() {
        Settings.Default();
        ReadSettings();
    }

    // Sync between Settings and UI
    public void WriteUI() { // call on Back / Save
    	Settings.masterVolume = masterVolume.value;
    	Settings.musicVolume = musicVolume.value;
    	Settings.sfxVolume = sfxVolume.value;
    	Settings.uiScale = uiScale.value;
    	Settings.fullscreen = fullscreen.isOn;
    	//Settings.Write();
    }
    public void ReadSettings() { // call on Start / Revert
    	masterVolume.value = Settings.masterVolume;
    	musicVolume.value = Settings.musicVolume;
    	sfxVolume.value = Settings.sfxVolume;
    	uiScale.value = Settings.uiScale;
    	fullscreen.isOn = Settings.fullscreen;
    }

    // Apply the settings
    public void ApplyAll() {
    	ApplyMasterVolume(Settings.masterVolume);
    	ApplyMusicVolume(Settings.musicVolume);
    	ApplySFXVolume(Settings.sfxVolume);
    	ApplyUIscale(Settings.uiScale);
    	ApplyFullscreen(Settings.fullscreen);
    }
    public void ApplyMasterVolume(float val) {
    	Settings.masterVolume = val;
    	for (int i = 0; i < music.Count; i++)
    		music[i].volume = Settings.musicVolume * Settings.masterVolume;
    	for (int i = 0; i < sfx.Count; i++)
    		sfx[i].volume = Settings.sfxVolume * Settings.masterVolume;
    }
    public void ApplyMusicVolume(float val) { // music slider OnValueChanged
    	Settings.musicVolume = val;
    	for (int i = 0; i < music.Count; i++)
    		music[i].volume = Settings.musicVolume * Settings.masterVolume;
    }
    public void ApplySFXVolume(float val) { // sfx slider OnValueChanged
    	Settings.sfxVolume = val;
    	for (int i = 0; i < sfx.Count; i++)
    		sfx[i].volume = Settings.sfxVolume * Settings.masterVolume;
    }
    public Vector2 minRefRes = new Vector2(800, 600);
    public Vector2 maxRefRes = new Vector2(1920, 1080);
    public void ApplyUIscale(float val) {
    	Settings.uiScale = 1-val;
    	cs.referenceResolution = minRefRes + Settings.uiScale * (maxRefRes-minRefRes);
    }
    public void ApplyFullscreen(bool val) {
    	Settings.fullscreen = val;
    	Screen.SetResolution(Screen.width, Screen.height, Settings.fullscreen);
    }



    // General functions
    public void PlayAudio(string name) {
    	for (int i = 0; i < music.Count; i++)
    		if (music[i].gameObject.name == name) {
    			music[i].Play();
    			return;
    		}
    	for (int i = 0; i < sfx.Count; i++)
    		if (sfx[i].gameObject.name == name) {
    			sfx[i].Play();
    			return;
    		}
    }
    public void StopAudio(string name) {
    	for (int i = 0; i < music.Count; i++)
    		if (music[i].gameObject.name == name) {
    			music[i].Stop();
    			return;
    		}
    	for (int i = 0; i < sfx.Count; i++)
    		if (sfx[i].gameObject.name == name) {
    			sfx[i].Stop();
    			return;
    		}
    }
    [HideInInspector] public bool paused = false;
    public void Pause() {
        Time.timeScale = 0;
        paused = true;
    }
    public void UnPause() {
        Time.timeScale = 1;
        paused = false;
    }
    public void QuitGame() {
        Application.Quit();
    }
    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    // HP / Energy
    public Slider health;
    public Slider energy;
    public TMP_Text robot;
    public void SetHealth(float val) {
        health.value = val;
    }
    public void SetEnergy(float val) {
        energy.value = val;
    }
    public void SetName(string val) {
        robot.text = val;
    }

    // Win UI
    public UIAnimations winFade;
    public void Win() {
        winFade.gameObject.SetActive(true);
        winFade.StartSingle();
    }

}
