using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRandomiser : Randomiser
{
	[SerializeField] private Background _background;
	public Cubemap[] backgroundCubemaps = new Cubemap[0];
	public Texture2D[] backgroundTextures = new Texture2D[0];
	public override void Randomise()
	{
		Shader.SetGlobalFloat("_BackgroundRandom", Random.value);

		if (backgroundTextures.Length == 0 && backgroundCubemaps.Length > 0) RandomCubemap();
		else if (backgroundTextures.Length > 0 && backgroundCubemaps.Length == 0) RandomTexture();
		else if (backgroundTextures.Length == 0 && backgroundCubemaps.Length == 0) return;
		else
		{
			if (Random.value > 0.5f)
			{
				RandomCubemap();
			}
			else
			{
				RandomTexture();
			}
		}
	}

	public void RandomCubemap()
	{
		int randomIndex = Random.Range(0, backgroundCubemaps.Length);
		Cubemap background = backgroundCubemaps[randomIndex];
		_background.SetCubemap(background);

	}

	public void RandomTexture()
	{
		int randomIndex = Random.Range(0, backgroundTextures.Length);
		Texture2D background = backgroundTextures[randomIndex];
		_background.SetTexture2D(background);
	}
}
