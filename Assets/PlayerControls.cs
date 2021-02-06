using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
