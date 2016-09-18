using UnityEngine;
using System.Collections;

public class PlayerController3D : MonoBehaviour 
{
	public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
	public float m_Speed = 500.0f;                 // How fast the tank moves forward and back.
	public float m_MaxSpeed = 10.0f;				// Limits the speed of the player(Ball)
	public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
	//public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
	//public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
	//public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
	public float m_PitchRange = 0.2f; 			// The amount by which the pitch of the engine noises can vary.

	public Rigidbody player_RigidBody;
	
	
	private string m_MovementAxisVerName;           // The name of the input axis for moving forward and back.
	private string m_MovementAxisHorName;			// The name of the input axis for moving left and Rght. 
	private string m_TurnAxisHorName;               // The name of the input axis for turning.
	private Rigidbody m_Rigidbody;              	// Reference used to move the player.
	public Transform m_Parent;
	private float m_MovementInputVerValue;         	// The current value of the movement input.
	private float m_MovementInputHorValue;
	private float m_TurnInputValue;            		// The current value of the turn input.
	private float m_OriginalPitch;            		// The pitch of the audio source at the start of the scene.
	
	
	private void Awake ()
	{
		//player_RigidBody = GetComponent<Rigidbody> ();
		m_Rigidbody = GetComponent<Rigidbody> ();
	}
	
	
	private void OnEnable ()
	{
		// When the tank is turned on, make sure it's not kinematic.
		player_RigidBody.isKinematic = false;
		
		//m_Parent = GetComponentInParent<Transform> ();
		// Also reset the input values.
		m_MovementInputVerValue = 0f;
		m_TurnInputValue = 0f;
	}
	
	
	private void OnDisable ()
	{
		// When the tank is turned off, set it to kinematic so it stops moving.
		player_RigidBody.isKinematic = true;
	}
	
	
	private void Start ()
	{
		// The axes names are based on player number.
		m_MovementAxisVerName = "Vertical" + m_PlayerNumber;
		m_MovementAxisHorName = "Horizontal" + m_PlayerNumber;
		m_TurnAxisHorName = "HorizontalRot" + m_PlayerNumber;
		
		// Store the original pitch of the audio source.
		//m_OriginalPitch = m_MovementAudio.pitch;
	}
	
	
	private void Update ()
	{
		// Store the value of both input axes.
		m_MovementInputVerValue = Input.GetAxis (m_MovementAxisVerName);
		m_MovementInputHorValue = Input.GetAxis (m_MovementAxisHorName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisHorName);
		
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
		MoveUp ();
		MoveRight ();
		Turn ();

		m_Parent.position = transform.position;
	}

	private void MoveUp()
	{
		//float verMovement = m_MovementInputVerValue * m_Speed * Time.deltaTime;
		//float horMovement = m_MovementInputHorValue * m_Speed * Time.deltaTime;
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movementUp =  transform.forward * m_MovementInputVerValue * m_Speed * Time.deltaTime;
		//Debug.Log (movementRight.x);
		//Debug.Log (movementRight.y);
		//Debug.Log (movementRight.z);

		// Apply this movement to the rigidbody's position.
		player_RigidBody.MovePosition(player_RigidBody.position + movementUp);

		transform.position = player_RigidBody.position;
	}

	private void MoveRight ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movementRight = transform.right * m_MovementInputHorValue * m_Speed * Time.deltaTime;
		
		
		// Apply this movement to the rigidbody's position.
		player_RigidBody.MovePosition(player_RigidBody.position + movementRight);

		transform.position = player_RigidBody.position;

	}
	
	private void Turn ()
	{
		// Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		Debug.Log ("Current rotate: " + turn);
		
		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		// Apply this rotation to the rigidbody's rotation.
		player_RigidBody.MoveRotation (player_RigidBody.rotation * turnRotation);
		//transform.rotation.eulerAngles.Set (transform.rotation.x, player_RigidBody.rotation.y, transform.rotation.z);
		Debug.Log (player_RigidBody.rotation.y);
		m_Rigidbody.MoveRotation (player_RigidBody.rotation * turnRotation);
		//transform.rotation = Quaternion.Euler (0f, turn, 0f);
	}


}
