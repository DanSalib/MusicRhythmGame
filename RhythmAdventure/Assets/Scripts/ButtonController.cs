using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {


    public Button gameButton;
    private Stopwatch buttonTimer;
    private float startTime;
    public float duration;
    public float buttonScore;

    public delegate void ButtonClick(ButtonController button);
    public static event ButtonClick OnClicked;

    public void InitializeButton(float start, float x, float y)
    {
        this.startTime = start;
        this.gameButton.gameObject.transform.localPosition = new Vector3(x, y);
        this.gameButton.gameObject.SetActive(true);
        this.buttonTimer = new Stopwatch();
        this.buttonTimer.Start();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(this.gameButton != null && this.gameButton.gameObject.activeSelf && this.buttonTimer.ElapsedMilliseconds > this.duration)
        {
            this.gameButton.gameObject.SetActive(false);
        }
        else if (this.gameButton != null && this.gameButton.gameObject.activeSelf)
        {
            this.gameButton.image.color = new Vector4(1 - CalcColor(), CalcColor(), 0, 1);
        }
	}

    public void ButtonClicked()
    {
        float clickTime = this.buttonTimer.ElapsedMilliseconds;
        this.buttonTimer.Stop();
        this.buttonScore = CalcScore(clickTime);
        this.gameButton.gameObject.SetActive(false);
        OnClicked(this);
    }

    public float CalcPerfectTime()
    {
        return ((this.duration) / 2f);
    }

    public float CalcScore(float clickTime)
    {
        return 1 - Mathf.Abs(clickTime - CalcPerfectTime()) / CalcPerfectTime();
    }

    public float CalcColor()
    {
        if(((this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime()) <= 1f)
        {
            return (this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        else if ((duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime() <= 1)
        {
            return (duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        return 0;
       
    }

}
