using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class InitialRigidbodyVelocity : MonoBehaviour
{
    public float speed;
    public virtual void Start()
    {
        this.GetComponent<Rigidbody>().velocity = this.transform.TransformDirection(Vector3.forward) * this.speed;
    }

    public InitialRigidbodyVelocity()
    {
        this.speed = 10f;
    }

}