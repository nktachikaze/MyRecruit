﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//視界に入った動体オブジェクトを追跡して破壊する
//ミサイルを生成するクラス
public class HomingMissileFactory : EntityFactory {

	//単体生成メソッドのオーバーライド
	public override GameObject MakeEntity(	string name,
											Func<Vector3> position_func,
											Func<Quaternion> rotation_func,
											Func<Vector3> scale_func)
	{

		GameObject temp_obj, child_obj;

		temp_obj = MonoBehaviour.Instantiate (	Resources.Load ("MyWork/Entity/Missile") as GameObject,
												position_func(),
												rotation_func()) as GameObject;

		temp_obj.name = name;
		temp_obj.tag = "Bullet";
		temp_obj.transform.localScale = scale_func();

		SetChild (	temp_obj, "Tail",
					Resources.Load ("MyWork/Entity/Parts/MissileTail") as GameObject,
					() => Vector3.zero,
					() => Quaternion.identity,
					() => 0.1f * Vector3.one);
		
		child_obj = SetChild (	temp_obj, "Sight",
								Resources.Load ("MyWork/Entity/Parts/Sight") as GameObject,
								() => new Vector3 (0f, 0f, 0.8f),
								() => Quaternion.identity,
								() => new Vector3 (80f, 80f, 70f));
	
		child_obj.AddComponent<HomingAI>();

		return temp_obj;

	}
}
