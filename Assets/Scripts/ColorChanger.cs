using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {
    public Color cNear;
    public Color cMiddle;
    public Color cFar;
    public float zNear;
    public float zFar;

    MeshRenderer mRenderer;

	void Start () {
	     mRenderer = GetComponent<MeshRenderer>();
         mRenderer.material.color = Color.white;
	}
	
	void Update () {
        if (transform.position.z > zFar) {
            mRenderer.material.color = cFar;
        } else if (transform.position.z < zNear) {
            mRenderer.material.color = cNear;
        } else {
            mRenderer.material.color = cMiddle;
        }
	}

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(0f, 0f, zNear), new Vector3(5f, 5f, 0.1f));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(0f, 0f, zFar), new Vector3(5f, 5f, 0.1f));
    }
}
