using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRandomiser : Randomiser
{
	[Header("Hand Settings")]
	[SerializeField] private Hand _hand;
	[SerializeField] private Material _handMaterial;
	[SerializeField] private Gradient _skinColour;
	[Header("Hand Position")]
	[SerializeField] private Vector3 _minimumHandPosition;
	[SerializeField] private Vector3 _maximumHandPosition;
	[Header("Hand Rotation")]
	[SerializeField] private Vector3 _minimumHandRotation;
	[SerializeField] private Vector3 _maximumHandRotation;

	[Space]
	[Header("Lighting Settings")]
	[SerializeField] private Light _lighting;	
	[SerializeField] private Gradient _lightColour;
	[SerializeField] private Gradient _ambientColour;
	[SerializeField] private Vector3 _minimumLightRotation;
	[SerializeField] private Vector3 _maximumLightRotation;

	public override void Randomise()
	{
		// Randomise hand pose
		_hand.Randomise();

		int iterations = 100;

		do
		{
			// Randomise hand position
			Vector3 handPosition = new(
				Random.Range(_minimumHandPosition.x, _maximumHandPosition.x),
				Random.Range(_minimumHandPosition.y, _maximumHandPosition.y),
				Random.Range(_minimumHandPosition.z, _maximumHandPosition.z)
			);

			Vector3 handRotation = new(
				Random.Range(_minimumHandRotation.x, _maximumHandRotation.x),
				Random.Range(_minimumHandRotation.y, _maximumHandRotation.y),
				Random.Range(_minimumHandRotation.z, _maximumHandRotation.z)
			);

			_hand.SetPosition(handPosition);
			_hand.SetRotation(handRotation);

		} while (!_hand.IsFullyInView || (--iterations <= 0));

		// Randomise hand appearance
		Color skinColour = _skinColour.Evaluate(Random.value);
		SetSkin(skinColour);

		// Randomise lighting
		Vector3 lightRotation = new(
			Random.Range(_minimumLightRotation.x, _maximumLightRotation.x),
			Random.Range(_minimumLightRotation.y, _maximumLightRotation.y),
			Random.Range(_minimumLightRotation.z, _maximumLightRotation.z)
		);

		Color lightColour = _lightColour.Evaluate(Random.value);
		Color ambientColour = _ambientColour.Evaluate(Random.value);

		SetLighting(lightRotation, lightColour, ambientColour);
		
	}

	public void SetLighting(Vector3 rotation, Color colour, Color ambientColour)
	{
		RenderSettings.ambientLight = ambientColour;
		_lighting.transform.eulerAngles = rotation;
		_lighting.color = colour;
	}

	public void SetSkin(Color skinColour)//, Texture2D noise, Color noiseColour)
	{
		_handMaterial.SetColor("_SkinColour", skinColour);
		//_skinMaterial.SetTexture("_NoiseTexture", noise);
		//_skinMaterial.SetColor("_NoiseColour", noiseColour);
	}
}
