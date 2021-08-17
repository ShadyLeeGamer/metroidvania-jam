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
    public static bool customCursor;

    public static void Default() {
    	masterVolume = 1;
    	musicVolume = 1;
    	sfxVolume = 0.75f;
    	uiScale = 0.5f;
    	fullscreen = true;
    	customCursor = true;
    }
    public static bool unread = true;
    public static void Read() {
    	unread = false;
    }
    public static void Write() {

    }
}
