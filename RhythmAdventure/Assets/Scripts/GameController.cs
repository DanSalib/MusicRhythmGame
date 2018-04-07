using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GameController : MonoBehaviour {

    public GameObject buttonPrefab;
    public Text scoreLabel;
    public List<float> buttonStartTimes;
    public List<float> buttonXPositions;
    public List<float> buttonYPositions;
    public float gameSpeed;
    public Stopwatch gameTimer = new Stopwatch();
    ButtonController buttonController;
    ButtonController[] buttonMap;
    public int gameScore = 0;
    public float buttonDelay = 20;

	// Use this for initialization
	void Start () {
        gameTimer.Start();
        ButtonController.OnClicked += OnGameButtonClick;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float currentGameTime = gameTimer.ElapsedMilliseconds;
        int buttonIndex = isTimeContainedInRange(currentGameTime);
        if (buttonIndex != -1)
        {
            CreateButton(currentGameTime, buttonXPositions[buttonIndex], buttonYPositions[buttonIndex]);

            buttonStartTimes.RemoveAt(buttonIndex);
            buttonXPositions.RemoveAt(buttonIndex);
            buttonYPositions.RemoveAt(buttonIndex);
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

    public void InitializeMap()
    {
        for(int i = 0; i < buttonStartTimes.Count; ++i)
        {
            CreateButton(buttonStartTimes[i], buttonXPositions[i], buttonYPositions[i]);
        }
    }

    private int isTimeContainedInRange(float value)
    {
        for (int i = 0; i < buttonDelay; ++i)
        {
            if (this.buttonStartTimes.Contains(value + i))
            {
                return buttonStartTimes.IndexOf(value + i);
            }
        }
        return -1;
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
