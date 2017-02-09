using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

	const float rotate_speed = 180.0f;
	public bool first_person_camera_mode;

	// Use this for initialization
	void Start () {
		transform.parent = null;
		transform.position = Vector3.up * SceneManager.cage_size / 3;
		transform.rotation = Quaternion.Euler (0, 0, 0);
		first_person_camera_mode = true;
	}

	// 固定視点変更関数
	public void FirstPersonCamera(){

		first_person_camera_mode = true;
		transform.parent = null;
		transform.position = Vector3.up * SceneManager.cage_size / 3;
		transform.rotation = Quaternion.Euler (0, 0, 0);
		Debug.Log ("カメラ初期位置");

	}

	//追従視点変更関数
	public void TargetChaseCamera(GameObject target){

		first_person_camera_mode = false;
		transform.parent = target.transform;
		transform.localPosition = new Vector3 (0, 20, -15);
		transform.localRotation = Quaternion.Euler (45, 0, 0);
		Debug.Log ("カメラ追従:" + transform.parent.name);

	}

	//追従対象破壊時の視点
	public void DeathCamera(){
		transform.parent = null;
		Debug.Log ("追従対象が破壊された");
	}


	public void CameraRotate(){
		
		float x_rotate, y_rotate;

		x_rotate = -Input.GetAxis ("Vertical") * rotate_speed * Time.deltaTime;

		//上下回転は無制限に許可すると回転がおかしくなるため上下85°までに制限する
		//Unityのx軸回転は無回転が0°　そこから下に回転で+1°
		//0°から上に回転すると即座に360°になってしまう仕様である
		//そのままでは範囲が不連続で扱いづらいため 以下のif文の左辺の計算をすると
		//95° < (現在のx軸回転角度 + 180) % 360 < 265°の連続した範囲が得られて
		//条件式として使用できる
		if ((transform.rotation.eulerAngles.x + x_rotate + 180f) % 360 <= 95f ||
		   	(transform.rotation.eulerAngles.x + x_rotate + 180f) % 360 >= 265f) {
			x_rotate = 0;
		}

		y_rotate = Input.GetAxis ("Horizontal") * rotate_speed * Time.deltaTime;

		//固定視点時は自由回転
		if (first_person_camera_mode) {

			//横回転と縦回転をローカル座標とワールド座標に分けることで
			//上下回転後の左右回転や左右回転後の上下回転の動作が直感的なものになる
			transform.Rotate (Vector3.right * x_rotate, Space.Self);
			transform.Rotate (Vector3.up * y_rotate, Space.World);

		}
			
	}

}
