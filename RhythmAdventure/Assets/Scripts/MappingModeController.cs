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

    public GameObject buttonPrefab;

    private Stopwatch mapTimer = new Stopwatch();
    
    private SortedList<float, float[]> mappedButtons = new SortedList<float, float[]>();
    private SortedList<float, MappingButton> displayingButtons = new SortedList<float, MappingButton>();

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

        if (this.mapTimer.IsRunning && Input.GetMouseButtonDown(0))
        {
            float[] position = new float[2];
            position[0] = Input.mousePosition.x;
            position[1] = Input.mousePosition.y;
            mappedButtons.Add(this.mapTimer.ElapsedMilliseconds + this.GetSpeedInputVal(), position);

            CreateButton(position[0], position[1]);
        }
	}

    private void UpdatePositionLabel(float x, float y)
    {
        this.positionLabel.text = "x: " + x + ", y: " + y;
    }

    private void CreateButton(float x, float y)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(this.transform, false);

        MappingButton mappingButton = button.GetComponent<MappingButton>();
        mappingButton.duration = this.GetSpeedInputVal();
        mappingButton.InitializeButton(x, y);

        //this.displayingButtons.Add(clickTime, mappingButton);
    }

    private void ButtonsToHide(float start, float end)
    {
        foreach(KeyValuePair<float, MappingButton> pair in displayingButtons)
        {
            if ((pair.Key - GetSpeedInputVal()) < start)
            {
                Destroy(pair.Value);
            }
            else if (pair.Key + GetSpeedInputVal() > end)
            {
                Destroy(pair.Value);
            }
        }
    }

    private void ButtonsToShow(float start, float end)
    {
        int buttonNum = this.RoundTo4(this.mappedButtons.Count) - 1;
        int guessIndex = Mathf.RoundToInt(start / this.mappedButtons.Keys[this.mappedButtons.Count - 1]);
        if (this.mappedButtons.Keys[guessIndex] > start)
        {
            int i = guessIndex;
            while (this.mappedButtons.Keys[i] > start)
            {
                i--;
            }
            while(this.mappedButtons.Keys[i] < end)
            {
                float time = this.mappedButtons.Keys[i];
                if (this.displayingButtons.ContainsKey(time))
                {
                    continue;
                }
                this.CreateButton(this.mappedButtons[time][0], this.mappedButtons[time][1]);
                buttonNum++;
                i++;
            }
        }
        else if (this.mappedButtons.Keys[guessIndex] < start)
        {
            int i = guessIndex;
            while (this.mappedButtons.Keys[i] < start)
            {
                i++;
            }
            while (this.mappedButtons.Keys[i] < end)
            {
                float time = this.mappedButtons.Keys[i];
                if (this.displayingButtons.ContainsKey(time))
                {
                    continue;
                }
                this.CreateButton(this.mappedButtons[time][0], this.mappedButtons[time][1]);
                buttonNum++;
                i++;
            }
        }

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

    private float GetSpeedInputVal()
    {
        float speed = 1f;
        if (this.speedInput != null)
        {
            float.TryParse(this.speedInput.text, out speed);
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

    private int RoundTo4(int num)
    {
        int nearestMultiple = (int)System.Math.Round((num / (double)4), System.MidpointRounding.AwayFromZero) * 4;
        return nearestMultiple;
    }
}
