
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Resizer : MonoBehaviour
{
    private int squareSize = 256;




    public void Resize()
    {
        var path = "FinalPrefab/Building_UnderlyingModels/Blacksmith/Textures";
        var allTextures = Resources.LoadAll(path, typeof(Texture2D)).Cast<Texture2D>().ToList();
        ResizeSquare(allTextures.First());
    }

    private void ResizeSquare(Texture2D texture)
    {
        if (!IsSquare(texture))
        {
            return;
        }
        if (texture.width <= squareSize)
        {
            return;
        }



        Resize(DuplicateTexture(texture), squareSize, squareSize);
    }

    private Texture2D DuplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }


    private void Resize(Texture2D texture, int newWidth, int newHeight)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        RenderTexture.active = tmp;
        Graphics.Blit(texture, tmp);

        texture.Reinitialize(newWidth, newHeight, texture.format, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.ReadPixels(new Rect(Vector2.zero, new Vector2(newWidth, newHeight)), 0, 0);
        texture.Apply();
        RenderTexture.ReleaseTemporary(tmp);
    }

    private static bool IsSquare(Texture2D texture)
    {
        return texture.width == texture.height;
    }
}