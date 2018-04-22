using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MappingButton : MonoBehaviour
{
    public Button gameButton;
    public Text buttonText;
    private Stopwatch buttonTimer = new Stopwatch();

    public float duration;

    public void InitializeButton(float x, float y)
    {
        this.transform.SetAsFirstSibling();

        this.gameButton.transform.SetParent(this.gameObject.transform, false);
        this.gameButton.gameObject.transform.position = new Vector3(x, y);
        this.gameButton.gameObject.SetActive(true);

        buttonTimer.Start();
    }

    private void Update()
    {
        if(this.buttonTimer.ElapsedMilliseconds > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
