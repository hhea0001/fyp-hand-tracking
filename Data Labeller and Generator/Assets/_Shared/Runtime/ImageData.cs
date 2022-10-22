using System;

public enum HandAction : int
{
    NotInFrame = 0,
    None = 1,
    Pinch = 2,
    Point = 3,
}

[Serializable]
public struct ImageData
{
    public string filename;
    public HandAction action;
    public float x;
    public float y;
    public float z;
}


//[Serializable]
//public struct HandData
//{
//    public HandAction action;
//    public float x;
//    public float y;
//    public float z;
//}

//[Serializable]
//public struct ValidationImageData
//{
//    public string filename;
//    public HandData current;
//    public HandData previous;
//}
