using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SmoothRandomPosition : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    // Moves the object along as far as range units randomly but in a smooth way.
    // This script requires the Noise.cs script.
    public float speed;
    public Vector3 range;
    // private Perlin noise;
    private Vector3 position;
    public virtual void Start()
    {
        this.position = this.transform.position;
    }

    public virtual void Update()
    {
        this.transform.position = this.position + Vector3.Scale(SmoothRandom.GetVector3(this.speed), this.range);
    }

    public SmoothRandomPosition()
    {
        this.speed = 1f;
        this.range = new Vector3(1f, 1f, 1f);
        // this.noise = new Perlin();
    }
}