using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
	public void SetCubemap(Cubemap cubemap)
	{
		Shader.EnableKeyword("_BackgroundIsCubemap");
		Shader.SetGlobalTexture("_BackgroundCubemap", cubemap);
	}

	public void SetTexture2D(Texture2D texture)
	{
		Shader.DisableKeyword("_BackgroundIsCubemap");
		Shader.SetGlobalTexture("_BackgroundTexture2D", texture);
	}
}
