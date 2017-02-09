using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//実体生成用クラスの雛形
public abstract class EntityFactory{

	//単体生成メソッドを各継承クラスでオーバーライドする
	public abstract GameObject MakeEntity(	string name,						//生成される実体の名前
											Func<Vector3> position_func,		//位置決定関数
											Func<Quaternion> rotation_func,		//回転決定関数
											Func<Vector3> scale_func);			//スケール設定関数



	//複数の実体を生成してリスト化するメソッド
	//デリゲートを渡すことで内部でのMakeEntity呼び出し時に各ループ毎で違った値を引数に与えられる
	//(いろいろな位置・回転・スケールで初期化されたオブジェクトのリストが得られる)
	//position_func以降の引数はラムダ式を使用して与えることでデリゲートによるメソッド渡しと
	//Vector3型やQuaternion型の変数渡しの両方に対応できる
	//関数で設定：() => basic_pos + frame_length * MyRandom.RandomVector3等　対応する型を返す式を渡す
	//変数で設定：() => Vector3.one等　対応する型の変数をそのまま渡す

	public List<GameObject> MakeEntityList(	int list_size,						//実体の最大生成数
											string name,						//生成される実体の名前
											Func<Vector3> position_func,		//位置決定関数
											Func<Quaternion> rotation_func,		//回転決定関数
											Func<Vector3> scale_func)			//スケール設定関数
	{

		List<GameObject> temp_list = new List<GameObject>();

		for (int i = 0; i < list_size; i++) {
			temp_list.Add (MakeEntity(name, position_func, rotation_func, scale_func));
			temp_list [temp_list.Count - 1].name = name + (i + 1);
		}

		return temp_list;

	}


	//単体の実体に子オブジェクトをセット
	public static GameObject SetChild(	GameObject target,						//子オブジェクトを紐づける親オブジェクト
										string child_name,						//子オブジェクトの名前
										GameObject source,						//子オブジェクトのリソースファイル
										Func<Vector3> position_func,			//ローカル位置決定関数
										Func<Quaternion> rotation_func,			//ローカル回転決定関数
										Func<Vector3> scale_func)				//ローカルスケール決定関数
	{

		GameObject temp_obj = null;

		//親オブジェクトの存在確認
		//単体へのSetChild使用時にtargetにnullを指定する人間はまさかいないだろうが
		//第一引数がList<GameObject>の場合から呼び出されるときに
		//List内にnullの要素があってもエラーを出さないように配慮しておく
		if (target != null) {

			temp_obj = MonoBehaviour.Instantiate (source) as GameObject;

			//親オブジェクトを設定して位置等の調整をする
			temp_obj.transform.parent = 		target.transform;
			temp_obj.transform.localPosition = 	position_func();
			temp_obj.transform.localRotation = 	rotation_func();
			temp_obj.transform.localScale = 	scale_func();
			temp_obj.name = child_name;
		
		}

		//子オブジェクトに対して更に処理を行う場合に備えてセットした子オブジェクトの情報を返す
		return temp_obj;

	}


	//複数の実体にまとめて子オブジェクトをセット
	//MakeEntityListと同じ理由でデリゲートを引数とする
	public static void SetChild(	List<GameObject> parent_list,			//子オブジェクトを紐づける親オブジェクトのリスト
									string child_name,						//子オブジェクトの名前
									GameObject source,						//子オブジェクトのリソースファイル
									Func<Vector3> position_func,			//ローカル位置決定関数
									Func<Quaternion> rotation_func,			//ローカル回転決定関数
									Func<Vector3> scale_func)				//ローカルスケール決定関数
	{

		foreach (GameObject target in parent_list) {
			
			SetChild (target, child_name, source, position_func, rotation_func, scale_func);

		}
	}


	//単体の実体の子オブジェクトを取り換え
	//こちらはSetChildと異なり同名の子オブジェクトがあった場合は消去してから追加する
	//位置と回転　スケールは取り換え前のオブジェクトから引き継ぐ
	public static GameObject ChangeChild(	GameObject target,						//子オブジェクトを紐づける親オブジェクト
											string child_name,						//子オブジェクトの名前
											GameObject source)						//子オブジェクトのリソースファイル
	{

		GameObject temp_obj = null;

		//親オブジェクトの存在確認
		//単体へのSetChild使用時にtargetにnullを指定する人間はまさかいないだろうが
		//第一引数がList<GameObject>の場合から呼び出されるときに
		//List内にnullの要素があってもエラーを出さないように配慮しておく
		if (target != null) {

			//同名の子オブジェクトがあった場合は消す
			if (target.transform.FindChild (child_name) != null) {

				GameObject prev_child = target.transform.FindChild (child_name).gameObject;

				temp_obj = MonoBehaviour.Instantiate (source) as GameObject;

				//親オブジェクトを設定して位置等の調整をする
				temp_obj.transform.parent = 		target.transform;
				temp_obj.transform.localPosition = 	prev_child.transform.localPosition;
				temp_obj.transform.localRotation = 	prev_child.transform.localRotation;
				temp_obj.transform.localScale = 	prev_child.transform.localScale;

				MonoBehaviour.Destroy (prev_child);
			}


			temp_obj.name = child_name;

		}

		//子オブジェクトに対して更に処理を行う場合に備えてセットした子オブジェクトの情報を返す
		return temp_obj;

	}


	//複数の実体の子オブジェクトをまとめて取り換え
	public static void ChangeChild(	List<GameObject> parent_list,			//子オブジェクトを紐づける親オブジェクトのリスト
									string child_name,						//子オブジェクトの名前
									GameObject source)						//子オブジェクトのリソースファイル
	{

		foreach (GameObject target in parent_list) {

			ChangeChild (target, child_name, source);

		}
	}





}
