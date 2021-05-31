using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Obsolete]
public class TankShooting : NetworkBehaviour
{
    public int playerNumber = 1;
    public GameObject shellRb;
    public Transform fireTransform;
    public Slider aimSlider;
    public AudioSource shootingAudio;
    public AudioClip chargingClip;
    public AudioClip fireClip;
    public float minLaunchForce = 15f;
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;

    private string _fireButton;
    public float _currentLaunchForce;
    private float _chargeSpeed;
    private bool _fired;

    

    private void OnEnable()
    {
        _currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
    }

    private void Start()
    {
        _fireButton = "Fire" + playerNumber;
        _chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        aimSlider.value = minLaunchForce;


        //MAX
        if (_currentLaunchForce >= maxLaunchForce && !_fired)
        {
            _currentLaunchForce = maxLaunchForce;

            if (isServer)
                RpcFire(_currentLaunchForce);
            else
                CmdFire(_currentLaunchForce);

            _fired = true;
        }
        //Begining
        else if (Input.GetButtonDown(_fireButton))
        {
            _fired = false;
            _currentLaunchForce = minLaunchForce;

            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        //Stay
        else if (Input.GetButton(_fireButton) && !_fired)
        {
            _currentLaunchForce += _chargeSpeed * Time.deltaTime;
            aimSlider.value = _currentLaunchForce;

            if (isServer)
                RpcUpdateForce(_currentLaunchForce);
            else
                CmdUpdateForce(_currentLaunchForce);
        }
        //Release
        else if (Input.GetButtonUp(_fireButton) && !_fired)
        {
            if (isServer)
                RpcFire(_currentLaunchForce);
            else
                CmdFire(_currentLaunchForce);

            _fired = true;
        }
    }

    [Command]
    void CmdUpdateForce(float force)
    {
        aimSlider.value = force;
    }

    [ClientRpc]

    void RpcUpdateForce(float force)
    {
        aimSlider.value = force;
    }

    [Command]// ส่งให้ตัว Server รู้ด้วย
    private void CmdFire(float force)
    {
        

        GameObject shellInstance = (GameObject)Instantiate(shellRb, fireTransform.position, fireTransform.rotation);
        shellInstance.GetComponent<Rigidbody>().velocity = force * fireTransform.forward;
        print(force);

        NetworkServer.Spawn(shellInstance);

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        aimSlider.value = minLaunchForce; 
        //_currentLaunchForce = minLaunchForce;
    }

    [ClientRpc]
    private void RpcFire(float force)
    {
        CmdFire(force);
        aimSlider.value = minLaunchForce;
    }
}