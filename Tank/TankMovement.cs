using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float speed = 12f;
    public float turnSpeed = 180f;
    public AudioSource movementAudio;
    public AudioClip engineIdling;
    public AudioClip engineDriving;
    public float pitchRange = 0.2f;

    private string _movementAxis;
    private string _turnAxis;
    private Rigidbody _rb;
    private float _movementInput;
    private float _turnInput;
    private float _originalPitch;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _rb.isKinematic = false;
        _movementInput = 0f;
        _turnInput = 0f;
    }

    private void OnDisable()
    {
        _rb.isKinematic = true;
    }

    private void Start()
    {
        _movementAxis = "Vertical";
        _turnAxis = "Horizontal";

        _originalPitch = movementAudio.pitch;
    }

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        _movementInput = Input.GetAxis(_movementAxis);
        _turnInput = Input.GetAxis(_turnAxis);

        EngineAudio();
    }

    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(_movementInput) < 0.1f && Mathf.Abs(_turnInput) < 0.1f)
        {
            if (movementAudio.clip == engineDriving)
            {
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        else
        {
            if (movementAudio.clip == engineIdling)
            {
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }

    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * _movementInput * speed * Time.deltaTime;
        _rb.MovePosition(_rb.position + movement);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = _turnInput * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        _rb.MoveRotation(_rb.rotation * turnRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Rim01_L_DarkInside")
        {
            speed += 8.0f;
        }

        Destroy(other.gameObject);
    }
}