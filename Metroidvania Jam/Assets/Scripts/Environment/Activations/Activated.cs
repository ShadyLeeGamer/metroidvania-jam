using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activated : MonoBehaviour
{

	public List<Activator> activators;
	bool AllActive() {
		bool a = true;
		for (int i = 0; i < activators.Count; i++)
			a &= activators[i].active;
		return a;
	}


    public virtual void Update() {
    	bool allActive = AllActive();
        if (allActive) {
        	if (!active) {
        		active = true;
        	}
        }
        else {
        	if (active) {
        		active = false;
        	}
        }
    }
    public bool active = false; // used by inherited scripts



}
