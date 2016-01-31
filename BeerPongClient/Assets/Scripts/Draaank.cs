using UnityEngine;
using System.Collections;

public class Draaank : MonoBehaviour {

    private Material material;

    // Between 0 and 50
    [Range(0.0f, 50.0f)]
    public float Drunk = 10.0f;

    void Awake()
    {
        material = new Material(Shader.Find("Hidden/DrunkShader"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_Drunk", Drunk);
        Graphics.Blit(source, destination, material);
    }

    public void AddDrunk()
    {
        Drunk += 10;
    }
}