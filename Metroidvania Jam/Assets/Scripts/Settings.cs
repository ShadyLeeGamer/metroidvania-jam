using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{

	public static float masterVolume;
    public static float musicVolume;
    public static float sfxVolume;
    public static float uiScale;
    public static bool fullscreen;

    public static void Default() {
    	masterVolume = 0.5f;
    	musicVolume = 1;
    	sfxVolume = 0.75f;
    	uiScale = 0.5f;
    	fullscreen = true;
    }

    // unimplemented rw from .txt
    public static bool unread = true;
    public static void Read() {
    	unread = false;
    }
    public static void Write() {

    }
}
