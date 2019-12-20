using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Heightmap : MonoBehaviour
{
    [SerializeField]
    ComputeShader heightmapKernel = null;

    [SerializeField, Range(-256.0f, 256.0f)]
    float offsetX = 0.0f;
    [SerializeField, Range(-256.0f, 256.0f)]
    float offsetY = 0.0f;

    [SerializeField]
    bool drawAxis = true;

    int kernelIndex;

    RenderTexture result;

    // Start is called before the first frame update
    void Start()
    {
        kernelIndex = heightmapKernel.FindKernel("HeightmapKernel");

        result = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32) { enableRandomWrite = true };
        result.Create();

        heightmapKernel.SetTexture(kernelIndex, "result", result);

        GetComponent<Renderer>().sharedMaterial.SetTexture("_BaseMap", result);
    }

    // Update is called once per frame
    void Update()
    {
        // レンダーテクスチャのクリア
        Graphics.SetRenderTarget(result);
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
        Graphics.SetRenderTarget(null);

        heightmapKernel.SetFloat("offsetX", offsetX);
        heightmapKernel.SetFloat("offsetY", offsetY);
        heightmapKernel.SetBool("drawAxis", drawAxis);
        heightmapKernel.Dispatch(kernelIndex, 1, 512, 1);
    }
}