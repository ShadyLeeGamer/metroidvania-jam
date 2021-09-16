using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Teleporter : Activated
{
    public Teleporter nextTeleporter;
    public Vector2 offsetPos;
    List<Rigidbody2D> objects = new List<Rigidbody2D>();
    void TeleportAll() {
        Vector3 pos = nextTeleporter.transform.position + (Vector3)nextTeleporter.offsetPos;
        for (int i = 0; i < objects.Count; i++) {
            if (objects[i] == null) {
                objects.RemoveAt(i);
                i--;
                continue;
            }
            
            Vector3 delta = objects[i].transform.position - transform.position - (Vector3)offsetPos;
            objects[i].transform.position = pos + delta;
            if (objects[i].gameObject.name == "Player")
                Camera.main.transform.position = pos + delta + Camera.main.GetComponent<CameraController>().offset;
        }
    }

// todo: an animation before start teleport
    /*void TeleportAnimation(float progress) {

    }*/

    public float teleportTime = 2f;
    float TPtimer = -1;
    public override void Update() {
    	base.Update();

        if (TPtimer < 0) {
            if (active && objects.Count > 0)
                TPtimer = 0;
        }
        else if (TPtimer < teleportTime) {
            TPtimer += Time.deltaTime;
        }
        else {
            TPtimer = -1;
            TeleportAll();
        }
    }

    // Store references to the objects
// todo: this is nearly a duplicate of conveyor, make a RB gatherer script?
    void OnTriggerEnter2D(Collider2D info) {
		GameObject other = info.gameObject;
		Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
		if (rb == null) {
			rb = other.GetComponentInParent<Rigidbody2D>();
			if (rb == null) return;
		}
        if (!objects.Contains(rb)) objects.Add(rb);
	}
    void OnTriggerExit2D(Collider2D info) {
        GameObject other = info.gameObject;
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb == null) return;
        }
        if (objects.Contains(rb)) objects.Remove(rb);
    }



}
