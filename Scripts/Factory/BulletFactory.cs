using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : EntityFactory {

	//単体生成メソッドのオーバーライド
	public override GameObject MakeEntity(	string name,
											Func<Vector3> position_func,
											Func<Quaternion> rotation_func,
											Func<Vector3> scale_func)
	{

		GameObject temp_obj;

		temp_obj = MonoBehaviour.Instantiate (	Resources.Load ("MyWork/Entity/Bullet") as GameObject,
												position_func(),
												rotation_func()) as GameObject;

		temp_obj.name = name;
		temp_obj.tag = "Bullet";
		temp_obj.transform.localScale = scale_func();
		temp_obj.AddComponent<GoAheadAI> ();

		return temp_obj;

	}
}
