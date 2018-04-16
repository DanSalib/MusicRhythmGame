using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ButtonData
{
    public List<ButtonItem> buttons = new List<ButtonItem>();
}

[System.Serializable]
public class ButtonItem
{
    public float time;
    public float[] position = new float[2];
}