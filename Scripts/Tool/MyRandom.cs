using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRandom {

	//-0.5f0～0.5fまででランダム化されたx, y, zを持つVector3を返却するstatic変数
	//意味合い的には(0,0,0)を中心点とした一辺の長さ 1の立方体を想像して
	//その立方体内のランダムな点を取るのと同じ
	public static Vector3 RandomVector3{

		get{
			return new Vector3 (Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
		}

	}

}
