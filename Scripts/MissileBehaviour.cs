using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour {

	void OnControllerColliderHit(ControllerColliderHit colhit){
		
		Debug.Log ("Collision!:" + transform.name + "->" + colhit.transform.name);

		//衝突したらその地点での一人称視点に変更
		if (transform.FindChild ("Main Camera") != null)
			transform.FindChild ("Main Camera").GetComponent<CameraBehaviour> ().DeathCamera ();

		Instantiate (	Resources.Load ("MyWork/Effects/MissileExplode") as GameObject,
						transform.position,
						transform.rotation);
		

		Destroy(this.gameObject);

		if (colhit.transform.tag != "Untagged" && colhit.transform.tag != "Statics") {

			if (colhit.transform.FindChild ("Main Camera") != null)
				colhit.transform.FindChild ("Main Camera").GetComponent<CameraBehaviour> ().DeathCamera ();

			Debug.Log ("from:" + colhit.transform.position);

			colhit.transform.position = (SceneManager.cage_size / 2) * Vector3.up + (SceneManager.cage_size - 30) * Random.insideUnitSphere;
			colhit.transform.rotation = Random.rotation;

			Debug.Log ("to:" + colhit.transform.position);
		}
	}

}
