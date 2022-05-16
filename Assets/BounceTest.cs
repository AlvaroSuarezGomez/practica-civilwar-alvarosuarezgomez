using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BounceTest : NetworkBehaviour
{
    private Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
        }
    }
}
