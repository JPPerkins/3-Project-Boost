using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 5f;
	[SerializeField] float levelLoadDelay = 2f;

	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip success;
	[SerializeField] AudioClip death;

	[SerializeField] ParticleSystem EngineParticles;
	[SerializeField] ParticleSystem SuccessParticles;
	[SerializeField] ParticleSystem DeathParticles;
	   
	enum State { Alive, Dead, Finished};
	State state = State.Alive;

	bool collisionsDisabled = false;

	Rigidbody rigidBody;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (state == State.Alive)
		{
			RespondToThrustInput();
			RespondToRotateInput();
		}
		if (Debug.isDebugBuild)
		{
			RespondToDebugKeys();
		}
	}

	private void RespondToDebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			LoadNextLevel();
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			collisionsDisabled = !collisionsDisabled;	
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (state != State.Alive || collisionsDisabled) { return; }

		switch (collision.gameObject.tag)
		{
			case "Friendly":
				break;
			case "Finish":
				StartDeathSequence();
				break;
			default:
				StartSuccessSequence();
				break;
		}
	}

	private void StartSuccessSequence()
	{
		state = State.Dead;
		audioSource.Stop();
		Invoke("LoadFirstLevel", levelLoadDelay);
		audioSource.PlayOneShot(death);
		SuccessParticles.Play();
	}

	private void StartDeathSequence()
	{
		state = State.Finished;
		audioSource.Stop();
		Invoke("LoadNextLevel", levelLoadDelay);
		audioSource.PlayOneShot(success);
		DeathParticles.Play();
	}

	private void LoadNextLevel()
	{
		SceneManager.LoadScene(1);
	}

	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	private void RespondToThrustInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust();
		}
		else
		{
			audioSource.Stop();
			EngineParticles.Stop();
		}
	}

	private void ApplyThrust()
	{
		rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(mainEngine);
		}
		EngineParticles.Play();
	}

	private void RespondToRotateInput()
	{
		rigidBody.freezeRotation = true; // take manual control of rotation

		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.forward * rotationThisFrame);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		rigidBody.freezeRotation = false;
	}
}
