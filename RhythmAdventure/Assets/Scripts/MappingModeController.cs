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
    public InputField speedInput;
    public AudioSource audio;

    private Stopwatch mapTimer = new Stopwatch();
    
    private SortedList<float, float[]> mappedButtons = new SortedList<float, float[]>();

    private void Start()
    {
        this.SetupAudioClip();
    }

    private void Awake()
    {
        this.speedInput.text = "1";
    }

    // Update is called once per frame
    private void Update ()
    {
        UpdatePositionLabel(Input.mousePosition.x, Input.mousePosition.y);
        UpdateTimerLabel();

        if(this.mapTimer.IsRunning && Input.GetMouseButtonDown(0))
        {
            float[] position = new float[2];
            position[0] = Input.mousePosition.x;
            position[1] = Input.mousePosition.y;
            mappedButtons.Add(this.mapTimer.ElapsedMilliseconds + this.getSpeedInputVal(), position);
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
            if(this.audio.clip != null)
            {
                audio.Pause();
            }

            this.mapTimer.Stop();
            this.playLabel.text = "Start";
        }
        else
        {
            if (audio.clip != null)
            {
                audio.Play();
            }
            if (this.speedInput != null)
            {
                this.speedInput.gameObject.SetActive(false);
            }

            this.mapTimer.Start();
            this.playLabel.text = "Stop";
        }
    }

    private int getSpeedInputVal()
    {
        int speed = 1;
        if (this.speedInput != null)
        {
            int.TryParse(this.speedInput.text, out speed);
        }
        return (speed*1000);
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

    private void SetupAudioClip()
    {
        string filePath = EditorUtility.OpenFilePanel("Select music file", Application.dataPath + "/Resources", "mp3");
        string[] path = filePath.Split('/');
        string clipName = path[path.Length - 1].Split('.')[0];
 
        AudioClip clip = Resources.Load(clipName) as AudioClip;

        this.audio.clip = clip;
    }
}
