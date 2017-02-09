using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//視界内の物体を3次元機動で避けるAIクラス
public class HomingAI : MonoBehaviour {

	private Vector3 target_turn_direction = Vector3.zero;	//"こっちに曲がれば追跡対象に接近できる"であろう方向

	private Vector3 turn_axis = Vector3.zero;				//target_turn_directionに向けての回転軸

	public float max_turn_speed = 180;						//最高回転角度
	private float turn_speed = 0.0f;						//回転速度
	private float turn_calc_weight = 0.0f;					//回転加減速用補正係数

	private float move_speed = 0;							//移動速度
															//最高速度はAIを付けた視界オブジェクトの
															//transform.localscale.z の3倍とする

	private float move_calc_weight = 1.0f;					//移動速度計算の補正係数

	private float go_ahead_time = 0;						//発射後直進する時間

	private GameObject lock_on_target = null;				//追跡対象

	private CharacterController character_controller;		//親オブジェクトのコントローラー

	//初期化処理
	void Start(){
		target_turn_direction = transform.parent.transform.forward;
		character_controller = transform.parent.GetComponent<CharacterController> ();
		go_ahead_time = Time.time + 1.0f;
	}

	void Update(){

		if (move_calc_weight > 0.0f)	move_calc_weight -= 0.5f * Time.deltaTime;
		if (move_calc_weight <= 0.0f)	move_calc_weight = 0.0f;

		if (Time.time > go_ahead_time) {

			//追跡対象が見つかるまでは真っ直ぐ飛ぶ
			//見つけたら追跡対象に近づけるように曲がる
			if (lock_on_target != null) {

				if (turn_calc_weight < 1f)
					turn_calc_weight += 0.5f * Time.deltaTime;
				if (turn_calc_weight >= 1f)
					turn_calc_weight = 1f;

				target_turn_direction = lock_on_target.transform.position - transform.parent.transform.position;
				turn_axis = Vector3.Cross (transform.parent.transform.forward, target_turn_direction);
				turn_speed = (Vector3.Angle (transform.parent.transform.forward, target_turn_direction) >= 0.0f ? 1 : -1) * max_turn_speed * turn_calc_weight * Time.deltaTime;
				transform.parent.transform.RotateAround (transform.parent.transform.position, turn_axis, turn_speed);
		
			}

		}

		move_speed = 90f * (1.0f - move_calc_weight);

		character_controller.Move (transform.parent.transform.TransformVector(Vector3.forward) * move_speed * Time.deltaTime);

	}

	void OnTriggerEnter(Collider other) {

		//他の視界トリガー等に反応しないようにしておく
		//何かしらの動体オブジェクトを見つけたら追跡開始
		if (!other.isTrigger && other.tag == "Dynamics" && lock_on_target == null) {

			lock_on_target = other.gameObject;
			Debug.Log ("lock on:" + lock_on_target.name);

		}
	}

}
