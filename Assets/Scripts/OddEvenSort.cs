using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddEvenSort : MonoBehaviour
{
    [SerializeField]
    ComputeShader sorterKernel = null;

    [SerializeField]
    Texture2D src = null;

    int kernelIndex;

    RenderTexture[] buffers;

    Material mat;

    int pixelSize;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        kernelIndex = sorterKernel.FindKernel("OddEvenKernel");

        buffers = new RenderTexture[2];
        for(var i = 0; i < buffers.Length; i++)
        {
            buffers[i] = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            buffers[i].enableRandomWrite = true;
            buffers[i].Create();
        }

        mat = GetComponent<Renderer>().material;
        pixelSize = src.width;
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;

        Graphics.CopyTexture(src, buffers[0]);

        for(int i = 0; i < pixelSize; i++)
        {
            sorterKernel.SetFloat("iteration", i);
            sorterKernel.SetTexture(kernelIndex, "src", buffers[count % 2]);
            sorterKernel.SetTexture(kernelIndex, "dest", buffers[(count + 1) % 2]);
            sorterKernel.Dispatch(kernelIndex, 1, 1024, 1);

            count++;
        }

        mat.SetTexture( "_BaseMap", buffers[count % 2] );
    }
}
