using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hand : Randomiser
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _pivot;
    [SerializeField] private Transform _offset;
    [SerializeField] private Transform _pointer;
    [SerializeField] private Transform[] _ensureInFrame;
    [SerializeField] private List<AnimationClip> _noneClips = new();
    [SerializeField] private List<AnimationClip> _pinchClips = new();
    [SerializeField] private List<AnimationClip> _pointClips = new();
    private Vector3 _position;
    private HandAction _action;
    private Camera _camera;

    public Animator Animator => _animator;
    public HandAction Action => _action;
    public Vector3 Position => _position;
    public List<AnimationClip> Clips => new List<AnimationClip>()
        .Concat(_noneClips)
        .Concat(_pinchClips)
        .Concat(_pointClips)
        .ToList();
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


    public bool IsFullyInView
	{
        get
		{
            bool inView = true;
            foreach(Transform point in _ensureInFrame)
			{
                Vector3 screenPos = Camera.WorldToViewportPoint(point.position);

                if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
				{
                    inView = false;
                    break;
				}
            }
            return inView;
		}
	}

	public override void Randomise()
	{
        _action = (HandAction)Random.Range(0, 4);
        Shader.SetGlobalFloat("_HandRandom", Random.value);

        if (_action != HandAction.NotInFrame)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }

        ResetPose();

        switch (_action)
		{
            case HandAction.None:
                SetPose(_noneClips);
                break;
            case HandAction.Pinch:
                SetPose(_pinchClips);
                break;
            case HandAction.Point:
                SetPose(_pointClips);
                break;
            default:
                break;
		}


        if (_action != HandAction.NotInFrame)
        { 
            _animator.Update(1f);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        else
		{
            gameObject.SetActive(false);
		}
	}

    public void SetPosition(Vector3 screenPosition)
	{
        _position = screenPosition;

        Vector3 position = Camera.ViewportToWorldPoint(screenPosition);
        _pivot.position = position;

        Vector3 currentPosition = _pointer.transform.position;
        _offset.Translate(position - currentPosition, Space.World);
    }

    public void SetRotation(Vector3 rotation)
	{
        _pivot.LookAt(Camera.transform);
        _pivot.transform.rotation *= Quaternion.Euler(rotation);
    }

    private void ResetPose()
	{
        int layerCount = _animator.layerCount;
        for (int i = 0; i < layerCount; i++)
		{
            _animator.SetLayerWeight(i, 0);
		}
	}

    private void SetPose(List<AnimationClip> clips)
	{
        int mainPose = Random.Range(0, clips.Count);

        float[] weights = new float[clips.Count];
        float sum = 0;

        // Create random weight for each pose
        // Give a larger weight to the 'main pose'
        for (int i = 0; i < clips.Count; i++)
		{
            weights[i] = i == mainPose ? 1 : 0.5f * Random.value;
            sum += weights[i];
		}

        // Apply weights
        for (int i = 0; i < clips.Count; i++)
		{
            int layerIndex = _animator.GetLayerIndex(clips[i].name);
            _animator.SetLayerWeight(layerIndex, weights[i]);
		}
	}
}
