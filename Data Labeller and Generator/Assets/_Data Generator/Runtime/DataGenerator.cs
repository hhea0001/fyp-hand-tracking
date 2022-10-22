using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataGenerator : Randomiser
{
	[SerializeField] private BackgroundRandomiser _backgroundRandomiser;
	[SerializeField] private HandRandomiser _handRandomiser;
	[SerializeField] private Hand _hand;
    private Camera _camera;
    private Camera Camera
	{
        get
		{
            if (_camera == null)
			{
                _camera = Camera.main;
			}
            return _camera;
		}
	}

	public override void Randomise()
	{
		_backgroundRandomiser.Randomise();
		_handRandomiser.Randomise();
	}

	[Header("Dataset Settings")]
    [SerializeField] private int _datasetSize = 128;
    [SerializeField] private int _randomiseHandEvery = 4;
    [SerializeField] private Vector2Int _resolution = new(320, 240);

    private ImageData[] _data;
    private RenderTexture _rt;
    private Texture2D _tex;

    public void GenerateData(string directory)
    {
        _data = new ImageData[_datasetSize];
        _rt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        _tex = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGB24, false);

        for (int i = 0; i < _data.Length; i++)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Generating data...", $"Up to {i + 1}/{_datasetSize}.", (i + 1) / (float)_datasetSize);
#endif
            _backgroundRandomiser.Randomise();
            if (i % _randomiseHandEvery == 0) _handRandomiser.Randomise();

            Vector3 position = _hand.Position;
            HandAction action = _hand.Action;

            string filename = directory + $"/{i}.png";
            SaveScreenshot(filename);

            _data[i] = new ImageData()
            {
                filename = i + ".png",

                action = action,
                x = position.x,
                y = 1 - position.y,
                z = position.z
            };
        }

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif

        Debug.Log($"Finished exporting data to {directory}");

        string jsonText = JsonConvert.SerializeObject(_data, Formatting.Indented);
        File.WriteAllText(directory + "/data.json", jsonText);
    }

    private void SaveScreenshot(string filename)
    {
        var temp = Camera.targetTexture;
        Camera.targetTexture = _rt;
        Camera.Render();
        Camera.targetTexture = temp;

        RenderTexture.active = _rt;
        _tex.ReadPixels(new Rect(0, 0, _resolution.x, _resolution.y), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = _tex.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }
}
