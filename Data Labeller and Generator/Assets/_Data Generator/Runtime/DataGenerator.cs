using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGenerator : Randomiser
{
	[SerializeField] BackgroundRandomiser _backgroundRandomiser;
	[SerializeField] HandRandomiser _handRandomiser;
	[SerializeField] Hand _hand;

	public override void Randomise()
	{
		_backgroundRandomiser.Randomise();
		_handRandomiser.Randomise();
	}
}
