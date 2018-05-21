using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MappingButton : MonoBehaviour
{
    public Button firstButton;
    public Button lastButton;
    public Image dragRegion;
    public Text buttonText;
    public bool isDrag = false;
    public Stopwatch buttonTimer = new Stopwatch();

    private const float MinDrag = 75f;

    public float duration;

    public void InitializeFirstButton(float x, float y)
    {
        this.transform.SetAsFirstSibling();

        this.gameObject.transform.position = new Vector3(x, y);
        this.firstButton.gameObject.SetActive(true);

        buttonTimer.Start();
    }

    public void InitializeLastButton(float x, float y)
    {
        if (Mathf.Abs(this.firstButton.transform.position.x - x) > MappingButton.MinDrag || Mathf.Abs(this.firstButton.transform.position.y - y) > MappingButton.MinDrag)
        {
            this.isDrag = true;
            this.dragRegion.gameObject.SetActive(true);
            this.lastButton.transform.SetParent(this.gameObject.transform, false);
            this.lastButton.gameObject.transform.position = new Vector3(x, y);

            Vector3 diffPos = this.lastButton.transform.position - this.firstButton.transform.position;
            float angle = Mathf.Atan2(diffPos.y, diffPos.x);
            this.dragRegion.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg);

            this.lastButton.gameObject.SetActive(true);
        }
        else
        {
            this.isDrag = false;
            this.dragRegion.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (this.buttonTimer.ElapsedMilliseconds > this.duration + 20)
        {
            this.DestroyButton();
        }
    }

    public void DestroyButton()
    {
        Destroy(this.gameObject);
    }
}
