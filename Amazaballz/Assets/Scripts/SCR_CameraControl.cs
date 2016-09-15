using UnityEngine;

public class SCR_CameraControl : MonoBehaviour
{
	public float m_DampTime = 0.2f;                 	// Approximate time for the camera to refocus.
	public float m_ScreenEdgeBuffer = 6.0f;         	// Space between the top/bottom most target and the screen edge.
	public float m_MinSize = 12.0f;                 	// The smallest orthographic size the camera can be.
	public float m_MaxSize = 30.0f;						// The largest orthographic size the camera can be.
	public float m_ZoomOutSize = 30.0f;					// Max camera zoom out value for CameraPan.
	/*[HideInInspector]*/ public Transform[] m_Targets; // All the targets the camera needs to encompass.
	public BoxCollider2D m_WorldBounds;					//Public variable to pass in the boxCollider used for the world Bounds
	public static float cameraHeight = 0.0f;			// The camera height.
	public static float cameraWidth = 0.0f;				// The camera width.

	private Camera m_Camera;                       		// Used for referencing the camera.
	private float m_ZoomSpeed;                      	// Reference speed for the smooth damping of the orthographic size.
	private Vector3 m_MoveVelocity;                 	// Reference velocity for the smooth damping of the position.
	private Vector3 m_DesiredPosition;              	// The position the camera is moving towards.
	private Vector3 _min, _max;							//private class varible for the coordinates of the worldBound
	private int m_CameraNormal, m_CameraZoom, m_CameraPan;
	private float m_CameraPanSize;
	private float m_CameraPanPositionX, m_CameraPanPositionY;
	private float m_CameraNormalHeight;

	
	
	private void Awake ()
	{
		m_Camera = GetComponentInChildren<Camera> ();
		// Setting the min and max to the worldBounds coordinates 
		_min = m_WorldBounds.bounds.min;
		_max = m_WorldBounds.bounds.max;

		if((PlayerPrefs.GetInt ("player" + "Respawn") != 0)
		   || (PlayerPrefs.GetInt ("player2" + "Respawn") != 0))
	   	{
	   		print ("Respawn Camera");
	   	
			//SCR_PlayerOne playerOne = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<SCR_PlayerOne>();
	   	
			//transform.position = new Vector3(playerOne.transform.position.x, playerOne.transform.position.y, transform.position.z);
			//Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - playerOne.transform.position.x, 0.0f, Camera.main.transform.position.z);
			//Camera.main.transform.position = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<SCR_PlayerOne>().transform.position;
		}

	}

	private void Start ()
	{
		m_CameraNormal = 1;
		m_CameraZoom = 0;
		m_CameraPan = 0;
	}
	
	
	private void Update ()
	{
		m_CameraNormal = PlayerPrefs.GetInt ("CameraNormal");
		m_CameraZoom = PlayerPrefs.GetInt ("CameraZoom");
		m_CameraPan = PlayerPrefs.GetInt ("CameraPan");
		m_CameraPanSize = PlayerPrefs.GetFloat ("CameraPanSize");
		m_CameraPanPositionX = PlayerPrefs.GetFloat("CameraPanPositionX");
		m_CameraPanPositionY = PlayerPrefs.GetFloat("CameraPanPositionY");

		if(m_CameraNormal == 0 && m_CameraZoom == 0 && m_CameraPan == 0)
		{
			m_CameraNormal = 1;
		}

//		Debug.Log (m_CameraNormal);
//		Debug.Log (m_CameraZoom);
//		Debug.Log (m_CameraPan);
	
		// Move the camera towards a desired position.
		Move ();
		
		// Change the size of the camera based.
		Zoom ();

	}

	private void Move ()
	{

		if(m_CameraPan == 1)
		{
			FindPosition ();
			//FindAveragePositionOfTwoPlayer ();
		}

		// Find the average position of Player One.
		if (m_CameraZoom == 1 && m_CameraNormal == 0) 
		{
			//Debug.Log ("Zoom Activate");
			FindAveragePositionOfOnePlayer ();

			
			// Smoothly transition to that position.
			transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
		}
		// Find the average position both Players.

		if (m_CameraNormal == 1 && m_CameraZoom == 0) 
		{
			//Debug.Log ("Normal Activate");
			FindAveragePositionOfTwoPlayer ();

			
			// Smoothly transition to that position.
			transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
		}


		// Find the position of bounds to stop empty space being shown
		FindBoundsPosition ();

	}

	private void FindBoundsPosition ()
	{
		// setting the camera position to the player position
		float x = transform.position.x;
		float y = transform.position.y;
		
		//calculating the camera width using the current camera height
		float cameraHalfWidth = m_Camera.orthographicSize* ((float)Screen.width / Screen.height);
		cameraWidth = m_Camera.orthographicSize * 2.0f;
		cameraHeight = cameraWidth * m_Camera.aspect;

		//calculates from the middle of the camera to account for the half width and height of the camera to the WorldBounds
		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + m_Camera.orthographicSize, _max.y - m_Camera.orthographicSize);
		
		transform.position = new Vector3 (x, y, transform.position.z);
	}

	private void FindPosition()
	{
		Vector3 m_NewPosition = new Vector3 (m_CameraPanPositionX, m_CameraPanPositionY, transform.position.z);
		//float newX = Mathf.Lerp(transform.position.x, newPosition.x, 1.0f);
		//float newY = Mathf.Lerp(transform.position.y, newPosition.y, 1.0f);
		//transform.position = new Vector3 (newX, newY, transform.position.z);

		
		// Smoothly transition to that position.
		transform.position = Vector3.SmoothDamp(transform.position, m_NewPosition, ref m_MoveVelocity, m_DampTime);
	}

	private void FindAveragePositionOfOnePlayer ()
	{
		Vector3 averagePos = new Vector3 ();
		int numTargets = 0;
		
		// Go through all the targets and add their positions together.
		for (int i = 0; i < 1; i++)
		{
			// If the target isn't active, go on to the next one.
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			// Add to the average and increment the number of targets in the average.
			averagePos += m_Targets[i].position;
			numTargets++;
		}
		
		// If there are targets divide the sum of the positions by the number of them to find the average.
		if (numTargets > 0)
			averagePos /= numTargets;
		
		// Keep the same y value.
		averagePos.z = transform.position.z;
		
		// The desired position is the average position;
		m_DesiredPosition = averagePos;
	}

	private void FindAveragePositionOfTwoPlayer ()
	{
		Vector3 averagePos = new Vector3 ();
		int numTargets = 0;
		
		// Go through all the targets and add their positions together.
		for (int i = 0; i < m_Targets.Length; i++)
		{
			// If the target isn't active, go on to the next one.
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			// Add to the average and increment the number of targets in the average.
			averagePos += m_Targets[i].position;
			numTargets++;
		}
		
		// If there are targets divide the sum of the positions by the number of them to find the average.
		if (numTargets > 0)
			averagePos /= numTargets;
		
		// Keep the same z value.
		averagePos.z = transform.position.z;
		
		// The desired position is the average position;
		m_DesiredPosition = averagePos;
	}
	
	
	private void Zoom ()
	{
		// Find the required size based on the desired position and smoothly transition to that size.
		if (m_CameraNormal == 1) 
		{
			float requiredSize = FindRequiredSize ();
			m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
			m_Camera.transform.position = new Vector3(transform.position.x, transform.position.y, m_Camera.transform.position.z);
		}

		if (m_CameraZoom == 1) 
		{
			//ZoomIn();
			float requiredSize = ZoomIn();
			m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
		}

		if (m_CameraPan == 1)
		{
			//ZoomOut();
			float requiredSize = ZoomOut();
			m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
		}
	}
	
	
	private float FindRequiredSize ()
	{
		// Find the position the camera rig is moving towards in its local space.
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
		
		// Start the camera's size calculation at zero.
		float size = 0f;
		
		// Go through all the targets...
		for (int i = 0; i < m_Targets.Length; i++)
		{
			// ... and if they aren't active continue on to the next target.
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			// Otherwise, find the position of the target in the camera's local space.
			Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
			
			// Find the position of the target from the desired position of the camera's local space.
			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
			
			// Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
			
			// Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
		}
		
		// Add the edge buffer to the size.
		size += m_ScreenEdgeBuffer;
		
		// Make sure the camera's size isn't below the minimum.
		size = Mathf.Max (size, m_MinSize);
		// Make sure the camera's size isn't above the maximum.
		size = Mathf.Min (m_MaxSize, size);
		
		return size;
	}

	private float ZoomIn ()
	{
		// Find the position the camera rig is moving towards in its local space.
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
		
		// Start the camera's size calculation at zero.
		float size = 8.0f;
		
		return size;
	}

	private float ZoomOut ()
	{
		float size = m_CameraPanSize;

		return size;
	}

	public float LerpCamera (float oldValue, float newValue)
	{
		// If the old value is different from the new value.
		if (oldValue != newValue) 
		{
			// Return the lerp value inbetween the values.
			return Mathf.MoveTowards (oldValue, newValue, m_ZoomSpeed * Time.deltaTime);
		} 
		// Otherwise, they are the same.
		else
		{
			// Return nothing.
			return 0.0f;
		}
	}
	

	/*public void SetStartPositionAndSize ()
	{
		// Find the desired position.
		FindAveragePosition ();
		
		// Set the camera's position to the desired position without damping.
		transform.position = m_DesiredPosition;
		
		// Find and set the required size of the camera.
		m_Camera.orthographicSize = FindRequiredSize ();
	}*/
}
