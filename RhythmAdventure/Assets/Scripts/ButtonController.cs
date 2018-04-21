using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {


    public Button gameButton;
    private Stopwatch buttonTimer;
    public Text buttonText;
    public Image indicator;

    private float startTime;
    public float duration;
    public float buttonScore;

    public delegate void ButtonClick(ButtonController button);
    public static event ButtonClick OnClicked;

    public void InitializeButton(float start, float x, float y)
    {
        this.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform, false);
        this.transform.SetAsFirstSibling();

        this.gameButton.transform.SetParent(this.gameObject.transform, false);
        this.gameButton.gameObject.transform.localPosition = new Vector3(x, y);
        this.gameButton.gameObject.SetActive(true);

        this.startTime = start;
        this.buttonTimer = new Stopwatch();
        this.buttonTimer.Start();
        StartCoroutine(this.ScaleIndicator());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(this.gameButton != null && this.gameButton.gameObject.activeSelf && this.buttonTimer.ElapsedMilliseconds > this.duration)
        {
            this.gameButton.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        //else if (this.gameButton != null && this.gameButton.gameObject.activeSelf)
        //{
        //    this.gameButton.image.color = new Vector4(1 - CalcColor(), CalcColor(), 0, 1);
        //}
	}

    public void ButtonClicked()
    {
        float clickTime = this.buttonTimer.ElapsedMilliseconds;
        this.buttonTimer.Stop();
        this.buttonScore = CalcScore(clickTime);
        this.gameButton.gameObject.SetActive(false);
        OnClicked(this);

        Destroy(this.gameObject);
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

    private IEnumerator ScaleIndicator()
    {
        Vector3 originalScale = this.indicator.transform.localScale;
        Vector3 destinationScale = new Vector3(0.6f, 0.6f, 0.6f);

        if (this.buttonTimer.IsRunning)
        {
            while(this.buttonTimer.ElapsedMilliseconds < (this.duration/2f))
            {
                this.indicator.transform.localScale = Vector3.Lerp(originalScale, destinationScale, this.buttonTimer.ElapsedMilliseconds / (this.duration / 2f));
                yield return null;
            }
        }
    }

}
