using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ひたすら前に進むだけのAI  ...AI?
public class GoAheadAI : MonoBehaviour {

	private float move_speed = 60;							//移動速度

	private CharacterController character_controller;		//親オブジェクトのコントローラー

	//初期化処理
	void Start(){
		character_controller = transform.GetComponent<CharacterController> ();
	}

	void Update(){
		character_controller.Move (transform.TransformVector(Vector3.forward) * move_speed * Time.deltaTime);
	}
}
