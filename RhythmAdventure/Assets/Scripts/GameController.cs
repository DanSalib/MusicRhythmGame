using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;
using UnityEngine.UI;



public class GameController : MonoBehaviour {

    public GameObject buttonPrefab;
    public Text scoreLabel;

    private List<float> buttonStartTimes = new List<float>();
    private List<float> buttonXPositions = new List<float>();
    private List<float> buttonYPositions = new List<float>();

    public Dictionary<float, float[]> buttonMap = new Dictionary<float, float[]>();

    public float gameSpeed;
    public int gameScore = 0;
    public Stopwatch gameTimer = new Stopwatch();

    ButtonController buttonController;

    private string gameDataFileName = "data.json";

    private

	// Use this for initialization
	void Start () {
        if(!LoadGameData())
        {
            return;
        }

        gameTimer.Start();
        ButtonController.OnClicked += OnGameButtonClick;
        
    }

    private bool LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if(File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            ButtonData buttonData = JsonUtility.FromJson<ButtonData>(dataAsJson);

            for (int i = 0; i < buttonData.buttons.Length; ++i)
            {
                buttonStartTimes.Add(buttonData.buttons[i].time);
                buttonXPositions.Add(buttonData.buttons[i].position[0]);
                buttonYPositions.Add(buttonData.buttons[i].position[1]);
            }

            return true;
        }

        return false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (buttonStartTimes.Count > 0 && gameTimer.ElapsedMilliseconds > buttonStartTimes[0])
        {
            CreateButton(gameTimer.ElapsedMilliseconds, buttonXPositions[0], buttonYPositions[0]);

            buttonStartTimes.RemoveAt(0);
            buttonXPositions.RemoveAt(0);
            buttonYPositions.RemoveAt(0);
        }
	}

    public void CreateButton(float startTime, float x, float y)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        buttonController = button.GetComponent<ButtonController>();
        buttonController.duration = gameSpeed;
        buttonController.InitializeButton(startTime, x, y);
    }

    public void OnGameButtonClick(ButtonController button)
    {
        this.gameScore += Mathf.RoundToInt(button.buttonScore * 1000);
        UpdateScoreLabel(gameScore);
    }

    private void UpdateScoreLabel(int scoreValue)
    {
        this.scoreLabel.text = "score: " + scoreValue;
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }

    /* OUTDATED
    private int isTimeContainedInRange(float value)
    {
        for (int i = 0; i < frameDelay; ++i)
        {
            if (this.buttonStartTimes.Contains(value + i))
            {
                return buttonStartTimes.IndexOf(value + i);
            }
        }
        return -1;
    }
    */

}
