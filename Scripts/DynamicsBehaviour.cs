using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicsBehaviour : MonoBehaviour {

	string prev_colhit_name = "";

	void OnControllerColliderHit(ControllerColliderHit colhit){

		//ミサイル等との接触時にこちらの衝突判定が優先されると困るため分岐
		//前回呼び出し時と同じオブジェクトと衝突した場合も処理しない (壁こすり等)
		if (colhit.transform.tag != "Bullet" && colhit.transform.name != prev_colhit_name) {

			//Debug.Log ("Collision!:" + transform.name + "->" + colhit.transform.name);
			//Debug.Log ("start prev_colhit_name:" + prev_colhit_name);

			prev_colhit_name = colhit.transform.name;

			//衝突したらその地点での一人称視点に変更
			if (transform.FindChild ("Main Camera") != null)
				transform.FindChild ("Main Camera").GetComponent<CameraBehaviour> ().DeathCamera ();

			GameObject temp_obj = Instantiate (Resources.Load ("MyWork/Effects/DynamicsExplode") as GameObject,
				                     transform.position,
				                     transform.rotation) as GameObject;

			//爆発エフェクトのマテリアルを軌跡のエフェクトと同一にする
			temp_obj.GetComponent<ParticleSystemRenderer> ().material = 
			transform.FindChild ("Tail").GetComponent<Renderer> ().material;

			if (colhit.transform.tag != "Statics") {

				if (colhit.transform.FindChild ("Main Camera") != null)
					colhit.transform.FindChild ("Main Camera").GetComponent<CameraBehaviour> ().DeathCamera ();
			
			}
				
			//Debug.Log ("end prev_colhit_name:" + prev_colhit_name);


		}
	}

}
