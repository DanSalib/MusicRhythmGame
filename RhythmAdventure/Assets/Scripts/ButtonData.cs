[System.Serializable]
public class ButtonData
{
    public ButtonItem[] buttons;
}

[System.Serializable]
public class ButtonItem
{
    public float time;
    public float[] position = new float[2];
}