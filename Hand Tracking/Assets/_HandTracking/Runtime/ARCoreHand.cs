using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCoreHand : MonoBehaviour
{
    [SerializeField] private HandPredictor _handPredictor;
    [SerializeField] private ARCameraManager _cameraManager;
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private RawImage _debugImage;
    [SerializeField] private HandPreview _preview;
    [SerializeField] private Vector2Int _inputResolution = new(160, 120);
    private Texture2D _cameraTexture;

    void OnEnable()
    {
        // Prevent device from sleeping
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Setup camera
        _cameraTexture = new Texture2D(_inputResolution.x, _inputResolution.y, TextureFormat.RGB24, false);
        _debugImage.texture = _cameraTexture;
        _cameraManager.frameReceived += FrameRecieved;
    }

	private void OnDisable()
	{
        _cameraManager.frameReceived -= FrameRecieved;
	}

    void OnHandPrediction(HandAction action, Vector3 position)
	{
        //_debugText.text = $"Action: {action}\nPosition: {position}";
        _debugText.text = $"Action: {action}";
        _preview.Preview(action, position);
    }

    private void FrameRecieved(ARCameraFrameEventArgs args)
    {
        if (_handPredictor.IsWorking) return;

        if (_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            var rawTextureData = _cameraTexture.GetRawTextureData<byte>();

            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(_inputResolution.x, _inputResolution.y),
                outputFormat = TextureFormat.RGB24,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            // See how many bytes you need to store the final image.
            int size = image.GetConvertedDataSize(conversionParams);

            if (rawTextureData.Length == size)
            {
                image.Convert(conversionParams, rawTextureData);
                _cameraTexture.Apply();

                _handPredictor.Predict(_cameraTexture, OnHandPrediction);
            }
            else
            {
                _debugText.text = $"Wrong size image";
            }

            image.Dispose();
        }
        else
        {
            _debugText.text = $"Can't acquire image";
        }
    }
}
