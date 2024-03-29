using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwapper : MonoBehaviour
{

	public List<GameObject> visible = new List<GameObject>();
	public List<GameObject> invisible = new List<GameObject>();

	public void Swap() {
		for (int i = 0; i < visible.Count; i++) {
			visible[i].SetActive(true);
		}
		for (int i = 0; i < invisible.Count; i++) {
			invisible[i].SetActive(false);
		}
	}
}
