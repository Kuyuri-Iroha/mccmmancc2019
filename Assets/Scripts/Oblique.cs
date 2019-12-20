using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Oblique : MonoBehaviour
{
    [SerializeField]
    ComputeShader obliqueKernel = null;

    [SerializeField]
    float v0 = 23.67f;

    [SerializeField, Range(0.0f, Mathf.PI/2.0f)]
    float angle = 1.0f;

    int kernelIndex;

    RenderTexture result;

    // Start is called before the first frame update
    void Start()
    {
        kernelIndex = obliqueKernel.FindKernel("ObliqueKernel");

        result = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32) { enableRandomWrite = true };
        result.Create();

        obliqueKernel.SetTexture(kernelIndex, "result", result);

        GetComponent<Renderer>().sharedMaterial.SetTexture("_BaseMap", result);
    }

    // Update is called once per frame
    void Update()
    {
        // レンダーテクスチャのクリア
        Graphics.SetRenderTarget(result);
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
        Graphics.SetRenderTarget(null);

        obliqueKernel.SetFloat("v0", v0);
        obliqueKernel.SetFloat("angle", angle);
        obliqueKernel.Dispatch(kernelIndex, 1, 1, 1);
    }
}
