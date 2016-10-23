// Group Namespaces together 
using UnityEngine;
using System.Collections;

// Class info here
public class template : MonoBehaviour {

/* Class description here:
 * 
 * This template demonstrates some formatting properties to become common amongst scripts. 
 * The purpose of including this metadata is to allow future users (maybe even you) to pick 
 * up the script and use it as simply as possible so be as explicit and consistent as possible. 
 * 
 * The naming conventions used when creating custom type is <HelloWorld> and 
 * for other things including methods it is <helloWorld>
 * 
 * These scripts will be managed by FreeForm3d. Suggestions and changes welcome.
*/

// Variables grouped by public/ private and labelled accordingly
	public enum AxisType{
		XAxis,
		ZAxis
	}
	public Color color;
	public float thickness = 0.002f;    
	public AxisType facingAxis = AxisType.XAxis;
	public float length = 100f;
	public bool showCursor = true;

	private GameObject holder;
	private GameObject pointer;
	private GameObject cursor;
	private Vector3 cursorScale = new Vector3(0.05f, 0.05f, 0.05f);
	private float contactDistance = 0f;
	private Transform contactTarget = null;

// Method Example (argv1, argv2)
// Comment: Example - This is to initialize the scene

	void SetPointerTransform(float setLength, float setThicknes)
	{
		// Set internal variables
		float beamPosition = setLength / (2 + 0.00001f);
		// follow with program
		if (facingAxis == AxisType.XAxis)
		{
			pointer.transform.localScale = new Vector3(setLength, setThicknes, setThicknes);
			pointer.transform.localPosition = new Vector3(beamPosition, 0f, 0f);
			if (showCursor)
			{
				cursor.transform.localPosition = new Vector3(setLength - cursor.transform.localScale.x, 0f, 0f);
			}
		} else
		{
			pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
			pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);

			if (showCursor)
			{
				cursor.transform.localPosition = new Vector3(0f, 0f, setLength - cursor.transform.localScale.z);
			}
		}
	}

// Method 2 example
// Comment: Example - This is to initialize the scene

	void Start () {
		
		Material newMaterial = new Material(Shader.Find("Unlit/Color"));
		newMaterial.SetColor("_Color", color);

		holder = new GameObject();
		holder.transform.parent = this.transform;
		holder.transform.localPosition = Vector3.zero;

		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pointer.transform.parent = holder.transform;
		pointer.GetComponent<MeshRenderer>().material = newMaterial;

		pointer.GetComponent<BoxCollider>().isTrigger = true;
		pointer.AddComponent<Rigidbody>().isKinematic = true;
		pointer.layer = 2;

		if (showCursor)
		{
			cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			cursor.transform.parent = holder.transform;
			cursor.GetComponent<MeshRenderer>().material = newMaterial;
			cursor.transform.localScale = cursorScale;

			cursor.GetComponent<SphereCollider>().isTrigger = true;
			cursor.AddComponent<Rigidbody>().isKinematic = true;
			cursor.layer = 2;
		}

		SetPointerTransform(length, thickness);        
	}
	// Example Comment of what is going on here
	float GetBeamLength(bool bHit, RaycastHit hit)
	{
		float actualLength = length;

		// Reset if beam not hitting or hitting new target
		if (!bHit || (contactTarget && contactTarget != hit.transform))
		{
			contactDistance = 0f;
			contactTarget = null;
		}

		// Check if beam has hit a new target
		if (bHit)
		{
			if (hit.distance <= 0)
			{
				Debug.Log ("watch it");
			}
			contactDistance = hit.distance;
			contactTarget = hit.transform;
		}

		// Adjust beam length if something is blocking it
		if (bHit && contactDistance < length)
		{
			actualLength = contactDistance;
		}

		if (actualLength <= 0)
		{
			actualLength = length;
		}

		return actualLength; ;
	}

// Method 3
// Comment: Example - This is to adjust beam length.

	void Update () {
		Ray raycast = new Ray(transform.position, transform.forward);

		RaycastHit hitObject;
		bool rayHit = Physics.Raycast(raycast, out hitObject);

		float beamLength = GetBeamLength(rayHit, hitObject);
		SetPointerTransform(beamLength, thickness);
	}
}