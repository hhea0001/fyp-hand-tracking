using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public enum HandAction
{
    NotInFrame = 0,
    None = 1,
    Pinch = 2,
    Point = 3,
}

public class HandPredictor : MonoBehaviour
{
    public delegate void HandPrediction(HandAction action, Vector3 position);

    [SerializeField] private NNModel _modelAsset;
    [SerializeField] private int _sync = 5;
    private HandAction _action;
    private Vector3 _position;
    private Model _model;
    private IWorker _worker;
    private bool _working = false;

    public HandAction Action => _action;
    public Vector3 Position => _position;
    public bool IsWorking => _working;

    private void OnEnable()
	{
        // Setup model
        _model = ModelLoader.Load(_modelAsset);
        _worker = _model.CreateWorker();
        //_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _model);
    }

    private void OnDisable()
    {
        _worker.Dispose();
    }

    public void Predict(Texture2D texture, HandPrediction onPredict)
    {
        StartCoroutine(Execute(texture, onPredict, _sync));
        //Execute(texture, onPredict, _sync);
    }

    IEnumerator Execute(Texture2D texture, HandPrediction onPredict, int syncEveryNthLayer = 5)
    {
        //float start = Time.realtimeSinceStartup;
        _working = true;

        Tensor image = new(texture, 3);
        Tensor prev_action = new(new int[] {1, 1}, new float[] { (int)_action });
        Tensor prev_position = new(new int[] {1, 3}, new float[] { _position.x, _position.y, _position.z });

        IDictionary<string, Tensor> inputs = new Dictionary<string, Tensor>()
        {
            { "image", image },
            { "prev_action", prev_action },
            { "prev_position", prev_position }
        };

        //IEnumerator enumerator = _worker.StartManualSchedule(inputs);
        //int layer = 0;
        //bool areMoreLayers = true;

        //while (areMoreLayers)
        //{
        //    areMoreLayers = enumerator.MoveNext();
        //    if (++layer % syncEveryNthLayer == 0)
        //    {
        //        _worker.FlushSchedule();
        //        yield return new WaitForEndOfFrame();
        //    }
        //}

        // execute neural network with specific input and get results back
        var worker = _worker.Execute(inputs);
        Tensor pred_action = worker.PeekOutput("pred_action");
        Tensor pred_position = worker.PeekOutput("pred_position");
        
        // allow main thread to run until neural network execution has finished
        //yield return new WaitForCompletion(pred_action);
        yield return new WaitForCompletion(pred_position);

		//Tensor pred_action = _worker.PeekOutput("pred_action");
		int action = pred_action.ArgMax()[0];
		_action = (HandAction)action;

		//Tensor pred_position = _worker.PeekOutput("pred_position");
		_position = new Vector3(pred_position[0], pred_position[1], pred_position[2]);

		if (onPredict != null)
		{
			onPredict.Invoke(_action, _position);
		}

		image.Dispose();
        prev_action.Dispose();
        prev_position.Dispose();

        _working = false;
        //float end = Time.realtimeSinceStartup;
        //Debug.Log(end - start);
    }
}
