using UnityEngine;
using UnityEngine.Networking;

public class UIDirectionControl : NetworkBehaviour
{
    public bool useRelativeRotation = true;

    private Quaternion _relativeRotation;

    private void Start()
    {
        _relativeRotation = transform.parent.localRotation;
    }

    private void Update()
    {
        if (useRelativeRotation)
            transform.rotation = _relativeRotation;
    }
}
