using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DestroyTrail();
    }

    public void DestroyTrail()
    {
        Destroy(this.gameObject, 2f);
    }
}
