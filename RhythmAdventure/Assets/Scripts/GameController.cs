using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameController instance;

    public GameObject buttonPrefab;
    public Text scoreLabel;
    public string gameDataFileName;
    public float gameSpeed;

    private int gameScore = 0;
    private int roundedButtonCount;
    public Stopwatch gameTimer = new Stopwatch();

    private List<float> buttonStartTimes = new List<float>();
    private List<float> buttonXPositions = new List<float>();
    private List<float> buttonYPositions = new List<float>();

	// Use this for initialization
	void Start () {
        if(!this.LoadGameData())
        {
            return;
        }

        this.gameTimer.Start();
        ButtonController.OnClicked += OnGameButtonClick;

        this.roundedButtonCount = this.ButtonCountInitializer();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.buttonStartTimes.Count > 0 && this.gameTimer.ElapsedMilliseconds > this.buttonStartTimes[0])
        {
            int buttonNum = 4 - this.roundedButtonCount % 4;
            this.CreateButton(gameTimer.ElapsedMilliseconds, buttonXPositions[0], buttonYPositions[0], buttonNum);

            this.buttonStartTimes.RemoveAt(0);
            this.buttonXPositions.RemoveAt(0);
            this.buttonYPositions.RemoveAt(0);
            this.roundedButtonCount--;
        }
	}

    private bool LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            ButtonData buttonData = JsonUtility.FromJson<ButtonData>(dataAsJson);

            for (int i = 0; i < buttonData.buttons.Count; ++i)
            {
                this.buttonStartTimes.Add(buttonData.buttons[i].time);
                this.buttonXPositions.Add(buttonData.buttons[i].position[0]);
                this.buttonYPositions.Add(buttonData.buttons[i].position[1]);
            }

            return true;
        }

        return false;
    }

    public void CreateButton(float startTime, float x, float y, int buttonNum)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform, false);
        ButtonController buttonController = button.GetComponent<ButtonController>();
        buttonController.buttonText.text = buttonNum.ToString();
        buttonController.duration = gameSpeed;
        buttonController.InitializeButton(startTime, x, y);
    }

    public void OnGameButtonClick(ButtonController button)
    {
        this.gameScore += Mathf.RoundToInt(button.buttonScore * 1000);
        this.UpdateScoreLabel(gameScore);
    }

    private void UpdateScoreLabel(int scoreValue)
    {
        this.scoreLabel.text = "score: " + scoreValue;
    }

    private int ButtonCountInitializer()
    {
        int count = this.buttonStartTimes.Count;
        int nearestMultiple = (int)System.Math.Round((count / (double)4), System.MidpointRounding.AwayFromZero) * 4;
        return nearestMultiple - 1;
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }
}
