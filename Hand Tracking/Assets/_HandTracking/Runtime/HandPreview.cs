using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPreview : MonoBehaviour
{
    [SerializeField] private Vector2Int _cameraRatio = new(4, 3);
    [SerializeField] private float _minMoveDistance = 0.1f;
    private Material _material;
    private Camera _camera;

    private HandAction _previousAction;

    private float _previousTime;
    private float _dt;
    private Vector3 _screenPos;
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;
    private Vector3 _velocity;

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

	private void Update()
	{
        //_currentPosition = _previousPosition + _velocity * (Time.time - _previousTime);

        transform.localPosition = Vector3.Lerp(_previousPosition, _currentPosition, (Time.time - _previousTime)/_dt);
	}

	public void Preview(HandAction action, Vector3 cameraPosition)
	{
        cameraPosition.y = 1 - cameraPosition.y;

        float cameraAspect = _cameraRatio.x / (float)_cameraRatio.y;
        float screenAspect = Camera.aspect;

        cameraPosition.y = (cameraPosition.y - 0.5f) * screenAspect / cameraAspect + 0.5f;

        //if (Vector3.Distance(_screenPos, cameraPosition) > _minMoveDistance)
		//{
            _screenPos = cameraPosition;
        //}

        Vector3 worldPosition = Camera.ViewportToWorldPoint(_screenPos);

        _previousPosition = _currentPosition;
        _currentPosition = Camera.transform.InverseTransformPoint(worldPosition); 

        SetAction(action);

        // Interpolation
        float currentTime = Time.time;
        _dt = currentTime - _previousTime;
        _previousTime = currentTime;

        //_velocity = (_currentPosition - _previousPosition) / _dt;


        if (action != _previousAction)
		{
            _previousAction = action;
            _previousPosition = _currentPosition;
		}
    }

    private void SetAction(HandAction action)
    {
        switch (action)
        {
            case HandAction.NotInFrame:
                gameObject.SetActive(false);
                break;
            case HandAction.None:
                SetColor(Color.white);
                gameObject.SetActive(true);
                break;
            case HandAction.Pinch:
                SetColor(Color.yellow);
                gameObject.SetActive(true);
                break;
            case HandAction.Point:
                SetColor(Color.red);
                gameObject.SetActive(true);
                break;
        }
    }

    private void SetColor(Color color)
    {
        if (_material == null)
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            _material = Instantiate(mr.sharedMaterial);
            mr.sharedMaterial = _material;
        }

        _material.color = color;
    }
}
