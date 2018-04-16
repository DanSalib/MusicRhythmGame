using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Text scoreLabel;
    public string gameDataFileName;
    public float gameSpeed;

    private int gameScore = 0;
    private Stopwatch gameTimer = new Stopwatch();

    private List<float> buttonStartTimes = new List<float>();
    private List<float> buttonXPositions = new List<float>();
    private List<float> buttonYPositions = new List<float>();

    ButtonController buttonController;

	// Use this for initialization
	void Start () {
        if(!this.LoadGameData())
        {
            return;
        }

        this.gameTimer.Start();
        ButtonController.OnClicked += OnGameButtonClick;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.buttonStartTimes.Count > 0 && this.gameTimer.ElapsedMilliseconds > this.buttonStartTimes[0])
        {
            this.CreateButton(gameTimer.ElapsedMilliseconds, buttonXPositions[0], buttonYPositions[0]);

            this.buttonStartTimes.RemoveAt(0);
            this.buttonXPositions.RemoveAt(0);
            this.buttonYPositions.RemoveAt(0);
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
        this.UpdateScoreLabel(gameScore);
    }

    private void UpdateScoreLabel(int scoreValue)
    {
        this.scoreLabel.text = "score: " + scoreValue;
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }
}
