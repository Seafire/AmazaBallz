using UnityEngine;
using System.Collections;

public class SCR_Camer2DContoller : MonoBehaviour 
{
	//Allows the player position to be passed in
	public Transform Player;

	//Public vector for the Margin(distance x and y will be in front)
	//and Smoothing(to stop the camera jumping between frames)
	public Vector2 Margin, Smoothing;

	//Public variable to pass in the boxCollider used for the world Bounds
	public BoxCollider2D worldBounds;
	
	//private class varible for the coordinates of the worldBound
	private Vector3 _min, _max;

	public bool IsFollowing { get; set; }

	// Functions.
	//////////////////////////////////////////////////
	//                    Start                 	//
	//==============================================//
	// This function will be called for             //
	// initialisation.								//
	//////////////////////////////////////////////////
	public void Start()
	{
		// Setting the min and max to the worldBounds coordinates 
		_min = worldBounds.bounds.min;
		_max = worldBounds.bounds.max;
		IsFollowing = true;
	}
	
	//////////////////////////////////////////////////
	//             		Update              		//
	//==============================================//
	// Called every frame, we will check to see if  //
	// we are moving the object, if we are,			//
	// activate the base object activated flag for  //
	// the standard response.						//
	//////////////////////////////////////////////////
	public void Update()
	{
		// setting the camera position to the player position
		float x = transform.position.x;
		float y = transform.position.y;

		//checks to see if the camera is following the player
		if (IsFollowing) 
		{
			// updates the camera position to the player checking the marginal diference 
			if (Mathf.Abs(x - Player.position.x) > Margin.x)
				x = Mathf.Lerp(x, Player.position.x, Smoothing.x * Time.deltaTime);
			if (Mathf.Abs(y - Player.position.x) > Margin.y)
				y = Mathf.Lerp(y, Player.position.y, Smoothing.y * Time.deltaTime);
		}
		//Debug.Log ("hello");
		//calculating the camera height using the current camera height
		float cameraHalfWidth = Camera.main.orthographicSize* ((float)Screen.width / Screen.height);

		//Debug.Log ("Hello2");
		//calculates from the middle of the camera to account for the half width and height of the camera to the WorldBounds
		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

		//Updates the camera position(z is not affected) 
		transform.position = new Vector3 (x, y, transform.position.z);
	}

	public float LerpCamera (float oldValue, float newValue)
	{
		// If the old value is different from the new value.
		if (oldValue != newValue) 
		{
			// Return the lerp value inbetween the values.
			return Mathf.MoveTowards (oldValue, newValue, Smoothing.x * Time.deltaTime);
		} 
		// Otherwise, they are the same.
		else
		{
			// Return nothing.
			return 0.0f;
		}
	}

}
