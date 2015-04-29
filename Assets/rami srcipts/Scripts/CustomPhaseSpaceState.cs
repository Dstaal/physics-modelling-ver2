using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomPhaseSpaceState {

	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float xd { get; set; }
	public float yd { get; set; }
	public float zd { get; set; }

	public CustomPhaseSpaceState() {
		this.x = 0f;
		this.y = 0f;
		this.z = 0f;

		this.xd = 0f;
		this.yd = 0f;
		this.zd = 0f;
	}

	public override string ToString () {
		return "(" + this.x + ", " + this.y + ", " + this.z + "), (" + this.xd + ", " + this.yd + ", " + this.zd + ")";
	}
}
