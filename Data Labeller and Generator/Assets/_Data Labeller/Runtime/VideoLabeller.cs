using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

[ExecuteAlways, RequireComponent(typeof(VideoPlayer))]
public class VideoLabeller : MonoBehaviour
{
	[Header("Playback Settings")]
    [SerializeField] private VideoPlayer _video;
    [SerializeField] private RenderTexture _renderTexture;    
    [SerializeField] private float _playbackSpeed = 0.5f;
    [SerializeField] private long _currentFrame = 0;
    private bool _writePosition = false;
    private Vector2 _currentPosition = Vector2.zero;

    [Header("Data Export Settings")]
    [SerializeField] private HandAction _handAction = HandAction.NotInFrame;
    [SerializeField] private Vector2[] _data = new Vector2[0];

    void OnEnable()
    {
        _video = GetComponent<VideoPlayer>();
        _video.prepareCompleted += OnVideoLoad;
		_video.loopPointReached += OnVideoEnd;
		_video.frameReady += OnNextFrame;
        _video.sendFrameReadyEvents = true;
    }

	private void OnDisable()
    {
        _video.prepareCompleted -= OnVideoLoad;
        _video.frameReady -= OnNextFrame;
    }

	private void OnValidate()
	{
        if (_currentFrame < 0) _currentFrame = 0;
        if (_currentFrame >= (long)_video.frameCount) _currentFrame = (long)_video.frameCount - 1;
        _video.frame = _currentFrame;
	}

	private void OnNextFrame(VideoPlayer source, long frame)
	{
        if (_writePosition) _data[_currentFrame] = _currentPosition;
		_currentFrame = frame;
	}

    private void OnVideoEnd(VideoPlayer source) => OnNextFrame(source, _currentFrame);

    public void OpenVideo(string filename)
	{
        // Start loading video
        _video.url = filename;
        _video.Pause();
        _video.Prepare();

        // Set up component
        _video.audioOutputMode = VideoAudioOutputMode.None;
        _video.renderMode = VideoRenderMode.RenderTexture;
        _video.targetTexture = _renderTexture;
        _video.isLooping = false;
        _currentFrame = 0;
    }

	private void OnVideoLoad(VideoPlayer source)
	{
        // Refresh preview
        _video.Play();
        _video.Pause();

        // Reset data
        _video.frame = _currentFrame = 0;
        _data = new Vector2[_video.frameCount];
	}

    public void SaveData(string folder)
	{
        ImageData[] data = new ImageData[_data.Length];

        HandData previousHandData = new HandData
        {
            action = _handAction,
            x = _data[0].x,
            y = _data[0].y,
            z = 2,
        };

        for (int i = 0; i < _data.Length; i++)
		{
            data[i] = new ImageData
            {
                filename = i + ".png",
                current = new HandData
                {
                    action = _handAction,
                    x = _data[i].x,
                    y = _data[i].y,
                    z = 2,
                },
                previous = previousHandData,
            };

            previousHandData = data[i].current;
		}

        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(folder + "/data.json", jsonData);
        Debug.Log($"Saved data to {folder}/data.json");
    }

    public void Play()
	{
        _writePosition = true;
        _currentFrame = _video.frame;
        _video.playbackSpeed = _playbackSpeed;
        _video.Play();
	}

    public void Pause()
	{
        _writePosition = false;
        _video.Pause();
	}

    public void SetCurrentPosition(Vector2 position)
	{
        _currentPosition = position;
    }
}
