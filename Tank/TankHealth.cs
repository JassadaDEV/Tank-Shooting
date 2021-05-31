using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Obsolete]
public class TankHealth : NetworkBehaviour
{
    [SerializeField]
    public float startingHealth;
    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public GameObject explosionPrefab;

    private AudioSource _explosionAudio;
    private ParticleSystem _explosionParticles;

    [SyncVar (hook = "UpdateHelth")]
    public float _currentHealth;
    public bool _isDead;

    private void Start()
    {
        startingHealth = 100.0f;
    }

    private void Awake()
    {
        _explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

        _explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _currentHealth = startingHealth;
        _isDead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        _currentHealth -= amount;

        SetHealthUI();

        if (_currentHealth <= 0f)
        {
            _currentHealth = startingHealth;

            //CmdRespawn();
            //RpcRespawn();
            RpcOnDeath();


        }
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        slider.value = _currentHealth;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, _currentHealth / startingHealth);
    }

    [Command]

    public void CmdRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = Vector3.zero;
            _currentHealth = startingHealth;
            UpdateHelth(startingHealth);
        }
    }

    [ClientRpc]
    private void RpcOnDeath()
    {

        _isDead = true;

        _explosionParticles.transform.position = this.transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();

        _explosionAudio.Play();

        this.gameObject.SetActive(false);
        GetComponent<TankMovement>().enabled = false;
        GetComponents<AudioBehaviour>()[0].enabled = false;
        GetComponents<AudioBehaviour>()[1].enabled = false;

    }


    /*void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }*/

    void UpdateHelth(float hp)
    {
        _currentHealth = hp;
        SetHealthUI();
    }
}