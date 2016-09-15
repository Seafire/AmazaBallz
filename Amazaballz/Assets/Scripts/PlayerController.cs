using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
	public float m_Speed = 500.0f;                 // How fast the tank moves forward and back.
	public float m_MaxSpeed = 10.0f;				// Limits the speed of the player(Ball)
	public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
	//public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
	//public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
	//public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
	public float m_PitchRange = 0.2f; 			// The amount by which the pitch of the engine noises can vary.
	public CollisionInfo collisions;
	
	
	private string m_MovementAxisVerName;           // The name of the input axis for moving forward and back.
	private string m_MovementAxisHorName;			// The name of the input axis for moving left and Rght. 
	private string m_TurnAxisHorName;               // The name of the input axis for turning.
	private Rigidbody m_Rigidbody;              	// Reference used to move the tank.
	private SphereCollider m_SphereCollider;
	private float m_MovementInputVerValue;         	// The current value of the movement input.
	private float m_MovementInputHorValue;
	private float m_TurnInputValue;            		// The current value of the turn input.
	private float m_OriginalPitch;            		// The pitch of the audio source at the start of the scene.
	private bool isJumping;
	private int jumpCount = 0;
	
	
	private void Awake ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_SphereCollider = GetComponent<SphereCollider> ();
	}
	
	
	private void OnEnable ()
	{
		// When the tank is turned on, make sure it's not kinematic.
		m_Rigidbody.isKinematic = false;
		
		// Also reset the input values.
		m_MovementInputVerValue = 0f;
		m_TurnInputValue = 0f;
	}
	
	
	private void OnDisable ()
	{
		// When the tank is turned off, set it to kinematic so it stops moving.
		m_Rigidbody.isKinematic = true;
	}
	
	
	private void Start ()
	{
		// The axes names are based on player number.
		m_MovementAxisVerName = "Vertical" + m_PlayerNumber;
		m_MovementAxisHorName = "Horizontal" + m_PlayerNumber;
		//m_TurnAxisHorName = "RightAngHorizontal-1" + m_PlayerNumber;
		isJumping = false;
		
		// Store the original pitch of the audio source.
		//m_OriginalPitch = m_MovementAudio.pitch;
	}
	
	
	private void Update ()
	{
		// Store the value of both input axes.
		m_MovementInputVerValue = Input.GetAxis (m_MovementAxisVerName);
		m_MovementInputHorValue = Input.GetAxis (m_MovementAxisHorName);
		//m_TurnInputValue = Input.GetAxis (m_TurnAxisHorName);
		
		//EngineAudio ();
	}
	
	
/*	private void EngineAudio ()
	{
		// If there is no input (the tank is stationary)...
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
		{
			// ... and if the audio source is currently playing the driving clip...
			if (m_MovementAudio.clip == m_EngineDriving)
			{
				// ... change the clip to idling and play it.
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}
		else
		{
			// Otherwise if the tank is moving and if the idling clip is currently playing...
			if (m_MovementAudio.clip == m_EngineIdling)
			{
				// ... change the clip to driving and play.
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		}
	}
	
	*/
	private void FixedUpdate ()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		//MoveRight ();
		//Turn ();
	}
	
	
	private void Move ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.

		float verMovement = m_MovementInputVerValue * m_Speed * Time.deltaTime;
		float horMovement = m_MovementInputHorValue * m_Speed * Time.deltaTime;
		float jumpMovement = (Physics.gravity.y*100 )* Time.deltaTime;

		if (!isJumping && Input.GetButtonDown ("Jump")) 
		{
			jumpMovement = 1000.0f;
			isJumping = true;
		}
		if (isJumping)
		{
			++jumpCount;
			if (jumpCount >= 30)
			{
				jumpMovement = (Physics.gravity.y*100 )* Time.deltaTime;
				jumpCount = 0;
			}
		}

		Vector3 movement = new Vector3 ((horMovement), jumpMovement, (verMovement));

		if(m_Rigidbody.velocity.magnitude > m_MaxSpeed)
		{
			m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_MaxSpeed;
		}

		// Apply this movement to the rigidbody's position.
		m_Rigidbody.AddForce(movement);
	}

	private void Turn ()
	{
		// Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
		
		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
		
		// Apply this rotation to the rigidbody's rotation.
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
	}

	public void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "ground") 
		{
			Debug.Log ("I made it");
			isJumping = false;
		}
	}

	public struct CollisionInfo 
	{
		public bool above, below;
		public bool left, right;

		public Vector3 velocityOld;
		
		public void Reset() 
		{
			above = below = false;
			left = right = false;

		}
	}
}
