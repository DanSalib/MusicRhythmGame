using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class MappingModeController : MonoBehaviour
{

    public Text positionLabel;
    public Text timerLabel;
    public Text playLabel;

    private Stopwatch mapTimer = new Stopwatch();
    
    private SortedList<float, float[]> mappedButtons = new SortedList<float, float[]>();
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePositionLabel(Input.mousePosition.x, Input.mousePosition.y);
        UpdateTimerLabel();

        if(this.mapTimer.IsRunning && Input.GetMouseButtonDown(0))
        {
            float[] position = new float[2];
            position[0] = Input.mousePosition.x;
            position[1] = Input.mousePosition.y;
            mappedButtons.Add(this.mapTimer.ElapsedMilliseconds, position);
        }
	}

    private void UpdatePositionLabel(float x, float y)
    {
        this.positionLabel.text = "x: " + x + ", y: " + y;
    }

    private void UpdateTimerLabel()
    {
        this.timerLabel.text = System.TimeSpan.FromMilliseconds(this.mapTimer.ElapsedMilliseconds).ToString();
    }

    public void OnPlayButtonPress()
    {
        if (this.mapTimer.IsRunning)
        {
            this.mapTimer.Stop();
            this.playLabel.text = "Start";
        }
        else
        {
            this.mapTimer.Start();
            this.playLabel.text = "Stop";
        }
    }

    public void OnMapFinish()
    {
        ButtonData buttonData = new ButtonData();

        foreach(KeyValuePair<float, float[]> pair in this.mappedButtons)
        {
            ButtonItem button = new ButtonItem();
            button.time = pair.Key;
            button.position[0] = pair.Value[0];
            button.position[1] = pair.Value[1];

            buttonData.buttons.Add(button);
        }

        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(buttonData);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}
