using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Obsolete]
public class SetUpLocalPlayer : NetworkBehaviour
{
    [SyncVar] //ให้เครื่องอื่นเห็น
    public string pname = "player";

    [SyncVar]
    public Color playerColor = Color.white;

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            pname = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), pname);
            if (GUI.Button(new Rect(130, Screen.height - 40, 80, 30), "Change"))
            {
                CmdChangeName(pname);
            }
        }
    }

    [Command]
    public void CmdChangeName(string name)
    {
        pname = name;
        this.GetComponentInChildren<TextMesh>().text = pname;
    }
    void Start()
    {
        if (!isLocalPlayer)  
        {
            return;
        }
        else if (isLocalPlayer)
        {
            GetComponent<TankMovement>().enabled = true;
            GetComponent<TankShooting>().enabled = true;
            GetComponents<AudioBehaviour>()[0].enabled = true;
            GetComponents<AudioBehaviour>()[1].enabled = true;

        }


        GameObject cam = GameObject.Find("CameraRig");
        cam.GetComponent<CameraControl>().targets.Add(this.transform);

        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
            r.material.color = playerColor;

        this.transform.position = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));


    }

    private void Update()
    {
        if (isLocalPlayer)
        this.GetComponentInChildren<TextMesh>().text = pname;
       
    }
}
