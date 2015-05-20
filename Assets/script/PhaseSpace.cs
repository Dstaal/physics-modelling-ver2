using UnityEngine;
using System.Collections;

public class PhaseSpace : MonoBehaviour {

	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float x_v { get; set; }
	public float y_v { get; set; }
	public float z_v { get; set; }
	
	public PhaseSpace() {
		this.x = 0f;
		this.y = 0f;
		this.z = 0f;
		
		this.x_v = 0f;
		this.y_v = 0f;
		this.z_v = 0f;
	}

	public override string ToString () {
		return "(" + this.x + ", " + this.y + ", " + this.z + "), (" + this.x_v + ", " + this.y_v + ", " + this.z_v + ")";
	}
}
