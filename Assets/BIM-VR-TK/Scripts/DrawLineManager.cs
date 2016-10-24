using UnityEngine;
using System.Collections;

public class DrawLineManager : MonoBehaviour {

	//private Color LineColor;
	public SteamVR_TrackedObject TrackedObj; 
	public float meshWidth = .2f;
	private MeshLineRenderer currLine;
	private int numClicks = 0;
	private GameObject meshes;

	void Update () {
		SteamVR_Controller.Device device = SteamVR_Controller.Input ((int)TrackedObj.index);
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) {
			GameObject go = new GameObject (); 
			go.AddComponent<MeshFilter> ();
			go.AddComponent<MeshRenderer> ();
			go.AddComponent<MeshCollider> ();
			currLine = go.AddComponent<MeshLineRenderer> ();
			currLine.MyMat (); 
			currLine.setWidth (meshWidth);
			currLine.tag = "MarkUp";
		} else if (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) { 
			currLine.AddPoint (TrackedObj.transform.position);
			numClicks++;
		} else if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) { 
			numClicks = 0;
			currLine = null;
		} else if (device.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			meshes = GameObject.FindWithTag ("MarkUp");
			Destroy (meshes);
		}
	}
}


