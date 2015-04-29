using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraRenderLines : MonoBehaviour {

	private GameController _gameController = null;

	void Start() {
		_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		if (_gameController == null)
			throw new System.NullReferenceException("CameraRenderLines could not find GameController object");
	}

	void OnPostRender() {
		if (_gameController == null || !_gameController.RenderSpringsInGame) 
			return;

		foreach (CustomParticleSystem system in _gameController.ParticleSystems) {
			foreach (CustomSpring spring in system.Springs) {
				Vector3 pos1 = spring.Particle1.transform.position;
				Vector3 pos2 = spring.Particle2.transform.position;

				GL.PushMatrix();
				GL.Begin(GL.LINES);

				//GL.Color(Color.yellow);

				GL.Vertex(pos1);				
				GL.Vertex(pos2);

				GL.End();
				GL.PopMatrix();
			}

			foreach (CustomAttraction attraction in system.Attractions) {
				Vector3 pos1 = attraction.Particle1.transform.position;
				Vector3 pos2 = attraction.Particle2.transform.position;

				GL.PushMatrix();
				GL.Begin(GL.LINES);

				//GL.Color(Color.red);

				GL.Vertex(pos1);
				GL.Vertex(pos2);

				GL.End();
				GL.PopMatrix();
			}
		}
	}
}
