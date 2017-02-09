using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEnder : MonoBehaviour {

	private ParticleSystem par;

	// Use this for initialization
	void Start () {
		par = this.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {

		//パーティクルシステムが取得できていて再生が終了したら消去
		if (par != null && par.particleCount == 0)
			Destroy (this.gameObject);
	}
}
