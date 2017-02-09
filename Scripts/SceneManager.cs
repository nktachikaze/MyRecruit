using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーンの必要情報を管理するクラス
public class SceneManager : MonoBehaviour {

	public CameraBehaviour camera_behavior;
	public GameObject main_camera;
	public const int something_max = 150;
	public const float cage_size = 300;		//入れ物の直径　もしくは一辺の長さ

	//入れ物内部の飛行物体に関する変数
	private List<GameObject> Somethings = new List<GameObject>();

	//入れ物そのものに関する変数
	private GameObject Cage = null;
	private List<GameObject> CageCollider = new List<GameObject>() ;

	private int some_num = 0;
	private int tail_num = 0;
	private int next_target_index = 0;
//	bool left_hand_shot;
//	float next_fire_bullet = 0.0f;
	private float next_fire_missile = 0.0f;

	// Use this for initialization
	void Start () {

		CageInitCube ();

		//せっかくだから使いたかったがあたり判定のめり込み等が多く断念
		//CageInitSphere();

		switch(Random.Range(0,3)){
		default:
			Somethings = new AvoidStarFactory ().MakeEntityList (
				something_max, "Something", 
				() => new Vector3 (0, cage_size / 2, 0) + (cage_size - 40) * MyRandom.RandomVector3,
			  //() => new Vector3 (0, cage_size / 2, 0) + (cage_size / 2 - 40) * Random.insideUnitSphere,
				() => Random.rotation,
				() => Vector3.one * 2);

			some_num = 0;
			tail_num = 0;

			break;

		case 1:
			Somethings = new AirplaneFactory ().MakeEntityList (
				something_max, "Something", 
				() => new Vector3 (0, cage_size / 2, 0) + (cage_size - 40) * MyRandom.RandomVector3,
				//() => new Vector3 (0, cage_size / 2, 0) + (cage_size / 2 - 40) * Random.insideUnitSphere,
				() => Random.rotation,
				() => Vector3.one * 2);

			some_num = 1;
			tail_num = 1;
			
			break;

		case 2:
			Somethings = new BirdFactory ().MakeEntityList (
				something_max, "Something", 
				() => new Vector3 (0, cage_size / 2, 0) + (cage_size - 40) * MyRandom.RandomVector3,
				//() => new Vector3 (0, cage_size / 2, 0) + (cage_size / 2 - 40) * Random.insideUnitSphere,
				() => Random.rotation,
				() => Vector3.one * 2);

			some_num = 2;
			tail_num = 2;

			break;

		}

		main_camera = GameObject.Find ("Main Camera");
		camera_behavior = main_camera.GetComponent<CameraBehaviour> ();

		Resources.UnloadUnusedAssets();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit ();
		}

		//以下は一人称視点時のみの操作
		if (camera_behavior.first_person_camera_mode) {

			//全ての物体の親オブジェクトのみ変更する
			if (Input.GetKeyDown (KeyCode.X)) {

				List<GameObject> temp_list = new List<GameObject>();

				foreach(GameObject target in Somethings){
					temp_list.Add(SomethingChange (target, some_num));
					Destroy (target);
				}

				Somethings = temp_list;

				some_num = (some_num + 1) % 3;

			}

			//全ての物体をランダムに変更する
			if (Input.GetKeyDown (KeyCode.C))
				RandomChange ();

			//誘導ミサイル発射
			if (Input.GetKey (KeyCode.Space) && Time.time > next_fire_missile) {

				new HomingMissileFactory ().MakeEntity (
					"HomingMissile", 
					() => main_camera.transform.position - new Vector3 (0, 10, 0) + (main_camera.transform.forward * 5),
					() => main_camera.transform.rotation,
					() => Vector3.one);

				next_fire_missile = Time.time + 1f;

			}
		}

		//作ってはみたが誘導なしでは撃っても当たらないためコメントアウト
//		if(Input.GetKey (KeyCode.Z) && Time.time > next_fire_bullet){
//
//			new BulletFactory ().MakeEntity (
//				"Bullet", 
//				() => main_camera.transform.position - Vector3.up + ((left_hand_shot ? -2 : 2) * main_camera.transform.right) + (main_camera.transform.forward * 2),
//				() => main_camera.transform.rotation,
//				() => Vector3.one);
//
//			left_hand_shot = !left_hand_shot;
//			next_fire_bullet = Time.time + 0.05f;
//
//		}

		//軌跡のエフェクトを変更する
		if (Input.GetKeyDown (KeyCode.Z))
			TailChange ();

		//Rキーを押すと固定視点に切り替わる
		if (!camera_behavior.first_person_camera_mode && Input.GetKeyDown (KeyCode.R))
			camera_behavior.FirstPersonCamera();

		//TABキーを押すと追従視点に切り替わる
		if (Input.GetKeyDown (KeyCode.Tab)) {

			while (Somethings [next_target_index] == null) {
				next_target_index++;
			}

			camera_behavior.TargetChaseCamera (Somethings[next_target_index]);

			next_target_index = (next_target_index + 1) % something_max;

		}

		camera_behavior.CameraRotate ();


	}

	private void CageInitCube(){

		GameObject source;

		//檻の纏めオブジェクト
		Cage = new GameObject("Cage");
		Cage.transform.position = Vector3.zero;

		//壁を配置
		source = GameObject.CreatePrimitive (PrimitiveType.Plane);
		source.GetComponent<Renderer> ().material = Resources.Load ("MyWork/Materials/Grass") as Material;
		source.GetComponent<MeshCollider> ().convex = true;
		source.AddComponent<Rigidbody> ();
		source.GetComponent<Rigidbody> ().useGravity = false;
		source.GetComponent<Rigidbody> ().isKinematic = true;
		source.tag = "Statics";
		source.transform.localScale = (cage_size / 10) * Vector3.one;

		CageCollider.Add (Instantiate (source, Vector3.zero, Quaternion.identity) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider1";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		CageCollider.Add (Instantiate (	source,
										new Vector3 (0, cage_size, 0),
										Quaternion.Euler (new Vector3 (180, 0, 0))) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider2";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		CageCollider.Add (Instantiate (	source,
										new Vector3 (0, cage_size / 2, cage_size / 2),
										Quaternion.Euler (new Vector3 (-90, 0, 0))) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider3";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		CageCollider.Add (Instantiate (	source,
										new Vector3 (0, cage_size / 2, -cage_size / 2),
										Quaternion.Euler (new Vector3 (90, 0, 0))) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider4";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		CageCollider.Add (Instantiate (	source,
										new Vector3 (cage_size / 2, cage_size / 2, 0), 
										Quaternion.Euler (new Vector3 (0, 0, 90))) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider5";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		CageCollider.Add (Instantiate (	source,
										new Vector3 (-cage_size / 2, cage_size / 2, 0),
										Quaternion.Euler (new Vector3 (0, 0, -90))) as GameObject);
		CageCollider[CageCollider.Count-1].name = "CageCollider6";
		CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;

		Destroy (source);


//		//障害物を配置
//		//視界オブジェクトがほぼ常に反応し続けるため大変重くなる
//
//		GameObject temp_obj;
//
//		source = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		source.GetComponent<Renderer> ().material = Resources.Load ("MyWork/Materials/Metallic") as Material;
//		source.AddComponent<Rigidbody> ();
//		source.GetComponent<Rigidbody> ().useGravity = false;
//		source.GetComponent<Rigidbody> ().isKinematic = true;
//		source.tag = "Statics";
//
//
//		for (int i = 0; i < (int)(cage_size / 20);i++) {
//			temp_obj = Instantiate (source,
//									new Vector3 (0, cage_size / 2, 0) + (cage_size - 40) * MyRandom.RandomVector3,
//									Random.rotation) as GameObject;
//									temp_obj.transform.localScale = Vector3.one * Random.Range(30.0f, 50.0f);
//		}
//
//		Destroy (source);

	}

//	private void CageInitSphere(){
//
//		GameObject source;
//
//		Cage = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//		Cage.name = "Cage";
//		Cage.GetComponent<Renderer> ().material = Resources.Load ("MyWork/Materials/Grass") as Material;
//		Cage.transform.position = new Vector3 (0, cage_size / 2, 0);
//		Cage.transform.localScale = cage_size * Vector3.one;
//		Destroy (Cage.GetComponent<Collider>());
//
//		source = GameObject.CreatePrimitive (PrimitiveType.Plane);
//		source.GetComponent<MeshCollider> ().convex = true;
//		source.AddComponent<Rigidbody> ();
//		source.GetComponent<Rigidbody> ().useGravity = false;
//		source.GetComponent<Rigidbody> ().isKinematic = true;
//		source.tag = "Statics";
//		source.transform.Rotate (Vector3.zero, Space.World);
//		source.transform.localScale = Vector3.one * (cage_size / 25);
//
//		Vector3 v3_cagecollpos = new Vector3 (0, -(cage_size / 2), 0);
//
//		int count = 0;
//
//		for(int i = 0; i < 4; i++){
//
//			for (int j = 0; j < 8; j++) {
//
//				transform.Rotate (new Vector3 (45, 0, 0), Space.Self);
//
//				if (i == 0 || (j != 3 && j != 7)) {
//					CageCollider.Add (Instantiate (source, transform.rotation * v3_cagecollpos, transform.rotation) as GameObject);
//					CageCollider [CageCollider.Count - 1].transform.position -= v3_cagecollpos;
//					CageCollider [CageCollider.Count - 1].transform.parent = Cage.transform;
//					Destroy(CageCollider [CageCollider.Count - 1].GetComponent<MeshRenderer> ());
//					CageCollider [CageCollider.Count - 1].name = "CageCollider" + (count + 1);
//					count++;
//				}
//			}
//
//			transform.Rotate (new Vector3 (0, 45, 0), Space.World);
//
//		}
//
//		Destroy (source);
//
//	}

	private void TailChange(){
		
		switch(tail_num){
		case 0:
			EntityFactory.ChangeChild (	Somethings, "Tail",
										Resources.Load ("MyWork/Entity/Parts/StarTail") as GameObject);

			tail_num = 1;
			break;

		case 1:
			EntityFactory.ChangeChild (	Somethings, "Tail",
										Resources.Load ("MyWork/Entity/Parts/PaperTail") as GameObject);

			tail_num = 2;
			break;

		case 2:
			EntityFactory.ChangeChild (	Somethings, "Tail",
										Resources.Load ("MyWork/Entity/Parts/FeatherTail") as GameObject);

			tail_num = 0;
			break;

		default:
			break;

		}

	}


	private void RandomChange(){

		List<GameObject> temp_list = new List<GameObject> ();

		foreach (GameObject target in Somethings) {

			temp_list.Add (SomethingChange (target, Random.Range (0, 3)));

			switch (Random.Range (0, 3)) {

			case 1:
				EntityFactory.ChangeChild (	temp_list[temp_list.Count - 1], "Tail",
											Resources.Load ("MyWork/Entity/Parts/StarTail") as GameObject);
				break;

			case 2:
				EntityFactory.ChangeChild (	temp_list[temp_list.Count - 1], "Tail",
											Resources.Load ("MyWork/Entity/Parts/PaperTail") as GameObject);
				break;


			default:
				EntityFactory.ChangeChild (	temp_list[temp_list.Count - 1], "Tail",
											Resources.Load ("MyWork/Entity/Parts/FeatherTail") as GameObject);
				break;

			}
				
			Destroy (target);

		}

		Somethings = temp_list;

	}


	GameObject SomethingChange(GameObject target, int idx){

		GameObject temp_obj;

		EntityFactory avoid, air, bird;
		avoid = new AvoidStarFactory ();
		air = new AirplaneFactory ();
		bird = new BirdFactory ();

		switch (idx) {

		case 1:
			temp_obj = avoid.MakeEntity("Something",
										() => target.transform.position,
										() => target.transform.rotation,
										() => target.transform.localScale);
			break;

		case 2:
			temp_obj = air.MakeEntity(	"Something",
										() => target.transform.position,
										() => target.transform.rotation,
										() => target.transform.localScale);
			break;


		default:
			temp_obj = bird.MakeEntity(	"Something",
										() => target.transform.position,
										() => target.transform.rotation,
										() => target.transform.localScale);

			break;

		}

		return temp_obj;


	}

}
