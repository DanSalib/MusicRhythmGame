using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audio;
    public string clipName;

	// Use this for initialization
	void Start ()
    {
        //LoadMusic();

	    if(audio.clip != null)
        {
            audio.Play();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void LoadMusic()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, clipName);

        WWW www = new WWW(filePath);
        if(www != null)
        {
            this.audio.clip = www.GetAudioClip();
        }
    }
}
