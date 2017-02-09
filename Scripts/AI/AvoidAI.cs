using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//視界内の物体を3次元機動で避けるAIクラス
public class AvoidAI : MonoBehaviour {

	private Vector3 target_turn_direction = Vector3.zero;	//"こっちに曲がれば目の前の物体を回避できる"であろう方向
															//平常時は進行方向ベクトルで初期化
															//物体発見時は物体上の最も近い点での法線ベクトルを
															//加算して方向修正を図る

	private Vector3 turn_axis = Vector3.zero;				//target_turn_directionに向けての回転軸

	private const float max_turn_speed = 270;				//最高回転角度
	private float turn_speed = 0.0f;						//回転速度
	private float turn_calc_weight = 0.0f;					//回転加減速用補正係数

	private const float max_move_speed = 30;				//最高移動速度
	private float move_speed = 0.0f;						//移動速度
	private float move_calc_weight = 1.0f;					//移動速度計算の補正係数
															//目の前に物体がある場合は減速する

	private bool now_collide = false;						//視界内に物体を検知した場合 true

	private Ray ray;										//レイキャスト用
	private RaycastHit hit;									//同上

	private CharacterController character_controller;		//親オブジェクトのコントローラー

	//初期化処理
	void Start(){
		target_turn_direction = transform.parent.transform.forward;
		character_controller = transform.parent.GetComponent<CharacterController> ();
	}

	void Update(){

		//一気にオブジェクトを回転させると見た目に違和感があるため
		//係数の加減算を利用して慣性が働いている感じを演出する
		//更にブレーキを掛けることで回転が終わるまでに衝突することを防ぐ
		if (now_collide) {
			if (turn_calc_weight < 1f)	turn_calc_weight += 0.5f * Time.deltaTime;
			if (turn_calc_weight >= 1f)	turn_calc_weight = 1f;
			if (move_calc_weight < 0.6f)	move_calc_weight += 1.0f * Time.deltaTime;
			if (move_calc_weight >= 0.6f)	move_calc_weight = 0.6f;

		} else {
			if (turn_calc_weight > 0.0f)	turn_calc_weight -= 1.0f * Time.deltaTime;
			if (turn_calc_weight <= 0.0f)	turn_calc_weight = 0.0f;
			if (move_calc_weight > 0.0f)	move_calc_weight -= 0.5f * Time.deltaTime;
			if (move_calc_weight <= 0.0f)	move_calc_weight = 0.0f;
		}

		//
		turn_speed = (Vector3.Angle (transform.parent.transform.forward, target_turn_direction) >= 0.0f ? 1 : -1) * max_turn_speed * turn_calc_weight * Time.deltaTime;
		transform.parent.transform.RotateAround (transform.parent.transform.position, turn_axis, turn_speed);

		move_speed = max_move_speed * (1.0f - move_calc_weight);

		character_controller.Move (transform.parent.transform.TransformVector(Vector3.forward) * move_speed * Time.deltaTime);

	}

	void OnTriggerStay(Collider other) {

		//他の視界トリガー等に反応しないようにしておく
		if (!other.isTrigger) {

			now_collide = true;

			//OnTriggerではCollision型のデータが入ってこないため
			//法線ベクトルを得ようとするとレイキャストが必要になる
			//OnCollisionでは思うような動作にならなかったため泣く泣くこの形に
			ray = new Ray(	transform.parent.transform.position,
							other.ClosestPointOnBounds(transform.parent.transform.position) - transform.parent.transform.position);

			//OnTriggerで検知したなら対象へのレイキャストを失敗することはないと思われるが
			//万が一を考えてif文で囲う
			//Raycastの第三引数はスケールでよいかと思いきや10倍しないと視界トリガーの端まで届かない模様
			if (ray.direction != Vector3.zero && other.Raycast (ray, out hit, transform.localScale.z * 10.0f)) {

				target_turn_direction += hit.normal;

				//回避対象と一対一の状態かつ回避対象の面に完璧に直角になってしまった場合など
				//全法線ベクトル加算後にゼロベクトルになるときは回転軸の計算が出来ないため進行方向以外のベクトルを足す
				if (target_turn_direction == Vector3.zero)	target_turn_direction += transform.parent.transform.up;

				target_turn_direction = Vector3.Normalize (target_turn_direction);

				if(target_turn_direction.magnitude > 0.1f)
					turn_axis = Vector3.Cross (transform.parent.transform.forward, target_turn_direction);

			}
		}
	}

	void OnTriggerExit(Collider other) {

		//視界範囲内にぶつかる対象がなければ回避方向と物体検知中フラグを初期化
		target_turn_direction = transform.parent.transform.forward;
		now_collide = false;

	}


}
