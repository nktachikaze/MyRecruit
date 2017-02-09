//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
////制作中に思いついたことを試すためのお遊び用クラス
//public class HogeHoge{
//
//	//恐るべきことにこのstatic変数は何の問題もなく動作する
//	//Func<bool>型のデリゲートのようなものか？
//	//値を参照するたびに何かしようとするため
//	//悪意を持って使えばパソコンを破壊することすらできてしまいそうである　怖い
//	public static bool AllGrean{
//
//		get{
//			bool a, b, c;
//
//			a = Fuga.test1 ("ok!");
//			b = Fuga.test2 (1, 2);
//			c = Fuga.test3 (() => Vector3.one, () => Quaternion.identity, 5f);
//
//			return (a && b && c);
//		}
//
//	}
//
//}
//
//public class Fuga{
//
//	public static bool test1(string msg){
//		Debug.Log ("test1:" + msg);
//		return true;
//	}
//
//	public static bool test2(int a, int b){
//		Debug.Log ("test2:" + (a + b));
//		return true;
//	}
//
//	public static bool test3(Func<Vector3> vec, Func<Quaternion> q, float length){
//		Debug.Log (length * (q() * vec()) );
//		return true;
//	}
//
//}
