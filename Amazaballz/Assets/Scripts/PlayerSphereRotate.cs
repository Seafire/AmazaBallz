using UnityEngine;
using System.Collections;

public class PlayerSphereRotate : MonoBehaviour 
{
	public int speed;
	public float friction;
	public float lerpSpeed;
	private float xDeg;
	private float yDeg;
	//private Quaternion fromRotation;
	private Rigidbody m_Rigidbody;              	// Reference used to move the tank.
	//private Quaternion toRotation;

	private void Awake ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
	}

	private void Update ()
	{
		xDeg -= Input.GetAxis("LeftAngHorizontal1") * speed * friction * Time.deltaTime;
		yDeg += Input.GetAxis("LeftAngVertical1") * speed * friction * Time.deltaTime;

		Debug.Log (xDeg);
		//fromRotation = transform.rotation;
		//toRotation = Quaternion.Euler(yDeg, xDeg, 0);
		//transform.rotation = Quaternion.Lerp(fromRotation,toRotation, Time.deltaTime * lerpSpeed);

		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (xDeg, yDeg, 0f);
		
		// Apply this rotation to the rigidbody's rotation.
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
	}

}
