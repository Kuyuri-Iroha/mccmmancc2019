using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSort : MonoBehaviour
{
    [SerializeField]
    ComputeShader sorterKernel = null;

    [SerializeField]
    Texture2D src = null;

    [SerializeField, Range(0.0f, 1.0f)]
    float brightnessThreshold = 0.0f;

    int kernelIndexHorizontal;
    int kernelIndexVertical;

    RenderTexture[] buffers;
    Material mat;

    int pixelSize;

    // Start is called before the first frame update
    void Start()
    {
        kernelIndexHorizontal = sorterKernel.FindKernel("PixelSortKernelHorizontal");
        kernelIndexVertical = sorterKernel.FindKernel("PixelSortKernelVertical");

        buffers = new RenderTexture[2];
        for(var i = 0; i < buffers.Length; i++)
        {
            buffers[i] = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            buffers[i].enableRandomWrite = true;
            buffers[i].Create();
        }

        mat = GetComponent<Renderer>().sharedMaterial;
        pixelSize = src.height;
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.CopyTexture(src, buffers[0]);

        sorterKernel.SetFloat("brightnessThreshold", brightnessThreshold);

        int count = 0;
        for(int i = 0; i < pixelSize; i++)
        {
            sorterKernel.SetFloat("iteration", i);
            sorterKernel.SetTexture(kernelIndexHorizontal, "src", buffers[count % 2]);
            sorterKernel.SetTexture(kernelIndexHorizontal, "dest", buffers[(count + 1) % 2]);
            sorterKernel.Dispatch(kernelIndexHorizontal, 1, pixelSize, 1);

            count++;
        }

        Graphics.CopyTexture(src, buffers[count % 2]);
        count = 0;
        for(int i = 0; i < pixelSize; i++)
        {
            sorterKernel.SetFloat("iteration", i);
            sorterKernel.SetTexture(kernelIndexVertical, "src", buffers[count % 2]);
            sorterKernel.SetTexture(kernelIndexVertical, "dest", buffers[(count + 1) % 2]);
            sorterKernel.Dispatch(kernelIndexVertical, pixelSize, 1, 1);

            count++;
        }

        mat.SetTexture("_BaseMap", buffers[count % 2]);
    }
}
