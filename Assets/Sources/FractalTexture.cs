using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class FractalTexture : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    public bool gray;
    public int width;
    public int height;
    public float lacunarity;
    public float h;
    public float octaves;
    public float offset;
    public float scale;
    public float offsetPos;
    private Texture2D texture;
    private Perlin perlin;
    private FractalNoise fractal;
    public virtual void Start()
    {
        this.texture = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
        this.GetComponent<Renderer>().material.mainTexture = this.texture;
    }

    public virtual void Update()
    {
        this.Calculate();
    }

    public virtual void Calculate()
    {
        if (this.perlin == null)
        {
            this.perlin = new Perlin();
        }
        this.fractal = new FractalNoise(this.h, this.lacunarity, this.octaves, this.perlin);
        int y = 0;
        while (y < this.height)
        {
            int x = 0;
            while (x < this.width)
            {
                if (this.gray)
                {
                    float value = this.fractal.HybridMultifractal((x * this.scale) + Time.time, (y * this.scale) + Time.time, this.offset);
                    this.texture.SetPixel(x, y, new Color(value, value, value, value));
                }
                else
                {
                    this.offsetPos = Time.time;
                    float valuex = this.fractal.HybridMultifractal((x * this.scale) + (this.offsetPos * 0.6f), (y * this.scale) + (this.offsetPos * 0.6f), this.offset);
                    float valuey = this.fractal.HybridMultifractal(((x * this.scale) + 161.7f) + (this.offsetPos * 0.2f), ((y * this.scale) + 161.7f) + (this.offsetPos * 0.3f), this.offset);
                    float valuez = this.fractal.HybridMultifractal(((x * this.scale) + 591.1f) + this.offsetPos, ((y * this.scale) + 591.1f) + (this.offsetPos * 0.1f), this.offset);
                    this.texture.SetPixel(x, y, new Color(valuex, valuey, valuez, 1));
                }
                x++;
            }
            y++;
        }
        this.texture.Apply();
    }

    public FractalTexture()
    {
        this.gray = true;
        this.width = 128;
        this.height = 128;
        this.lacunarity = 6.18f;
        this.h = 0.69f;
        this.octaves = 8.379f;
        this.offset = 0.75f;
        this.scale = 0.09f;
    }

}