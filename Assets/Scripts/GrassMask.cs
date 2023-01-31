using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrassMask
{
    public RenderTexture maskTexture;
    public ComputeShader grassCutoutComputeShader;

    private int width;
    private int height;

    public GrassMask(int width, int height)
    {
        this.width = width;
        this.height = height;

        maskTexture = new RenderTexture(width, height, 0);
        maskTexture.enableRandomWrite = true;
        grassCutoutComputeShader = Resources.Load<ComputeShader>("GrassComputeShader");

        grassCutoutComputeShader.SetTexture(0, "disableTexture", maskTexture);
        grassCutoutComputeShader.SetTexture(1, "disableTexture", maskTexture);
    }

    public void UpdatePlayerPosition(Vector2 pos)
    {
        int sizeOfThredsGroup = 16;
        int countOfThredsGroup = 1;

        int minusCenter = sizeOfThredsGroup * countOfThredsGroup / 2;
        grassCutoutComputeShader.SetVector("coloringPosition", new Vector4((pos.x * width - minusCenter), (pos.y * height - minusCenter), 0, 0));
        grassCutoutComputeShader.Dispatch(1, countOfThredsGroup, countOfThredsGroup, 1);
    }

}
