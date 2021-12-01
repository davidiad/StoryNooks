// reference: https://stackoverflow.com/questions/44264468/convert-rendertexture-to-texture2d
/* Usage:
   public RenderTexture tex;
   Texture2D myTexture = tex.ToTexture2D();
*/

using UnityEngine;
using StoryNook;

public static class Texture2DExtension
{ 
    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(3000, 2000, TextureFormat.RGB24, false);

        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}

