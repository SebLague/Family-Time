using System;
using UnityEngine;

public class PostProc : MonoBehaviour
{
    public Shader shader;
    Material mat;
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!mat) mat = new Material(shader);
        
        Graphics.Blit(source, destination, mat);
    }
}
