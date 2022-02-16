using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;
using System.IO;

[RequireComponent(typeof(AudioSource))]

public class FileSelector : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Audio Stuff")]
    public List<AudioClip> clipList = new List<AudioClip>();
    public List<string> clipListNames = new List<string>();
    [HideInInspector]
    public AudioSource audioSource;
    public string soundPath = "";
    int index = 0;
    bool on = false;
    int listSize = 0;


    string path;

    public GameObject playButton;
    public GameObject pauseButton;
    public Slider musicSlider;
    public Slider volumeSlider;
    public GameObject volumeObject;
    public TextMeshProUGUI songName;
    public Sprite[] volumeImage;
    private bool firstplay = true;
    private bool timeReset = false;
    private float tmp = 0;
    private float forward_backward = 0;
    private float pauseTime = 0;

    public void Start()
    {
        DirectoryInfo dir = new DirectoryInfo(OpenExplorer());
        Debug.Log(dir);
        int i = 0;
        FileInfo[] info = dir.GetFiles("*.mp3");
        foreach (FileInfo f in info)
        {
            clipListNames.Add(f.Name);
            //clipNames[i] = f.Name;
            i++;
        }
        listSize = i;
        audioSource = gameObject.AddComponent<AudioSource>();
        soundPath = dir.ToString();
        StartCoroutine(LoadAudio());
        if (i > 0)
        {
            songName.text = clipListNames[0];
        }
    }
    public void Update()
    {
        if (on)
        {
            musicSlider.value = Time.time - tmp + forward_backward;
        }
        audioSource.volume = volumeSlider.value;
        VolumeControl();
    }

    private IEnumerator LoadAudio()
    {
        for (int i = 0;i< listSize; i++)
        {
            WWW request = GetAudioFromFile(soundPath, clipListNames[i]);
            yield return request;

            clipList.Add(request.GetAudioClip());
            clipList[i].name = clipListNames[i];
        }
    }

    private void PlayAudioFile(int i)
    {
        audioSource.time = 0;
        forward_backward = 0;
        songName.text = clipListNames[index];
        TimeReset();
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        audioSource.clip = clipList[i];
        musicSlider.maxValue = clipList[i].length;
        audioSource.Play();
        audioSource.loop = true;
    }

    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = string.Format(path + "{0}", filename);
        WWW request = new WWW(audioToLoad);
        return request;
    }

    public string OpenExplorer()
    {
        path = EditorUtility.OpenFolderPanel("Seçim Ekranı", "", "")+"/";
        return path;
    }

    public void StartMusic()
    {
        songName.text = clipListNames[index];
        //OpenExplorer();
        pauseButton.SetActive(true);
        playButton.SetActive(false);
        on = false;
        if (!timeReset)
        {
            TimeReset();
        }
        audioSource.Pause();
        pauseTime = musicSlider.value - forward_backward;
    }
    public void PauseMusic()
    {
        songName.text = clipListNames[index];
        //OpenExplorer();
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        on = true;
        if (firstplay)
        {
            PlayAudioFile(index);
            firstplay = false;
        }
        else
            audioSource.UnPause();

        TimeReset();
        tmp -= pauseTime;
    }

    public void Left()
    {
        index--;
        if (index < 0)
        {
            index = listSize - 1;
        }
        PlayAudioFile(index);

    }

    public void Right()
    {
        index++;
        if (index > listSize - 1)
        {
            index = 0;
        }
        PlayAudioFile(index);
    }
    public void Forward()
    {
        if (musicSlider.value >= clipList[index].length)
        {
            Right();
        }
        else
        {
            audioSource.time += 5;
            forward_backward += 5;
            musicSlider.value += 5;
        }
    }
    public void Backward()
    {
        if (musicSlider.value - 5 > 0)
        {
            audioSource.time -= 5;
            forward_backward -= 5;
            musicSlider.value -= 5;
        }
    }

    public void TimeReset()
    {
        tmp = Time.time;
        timeReset = true;
    }

    public void VolumeControl()
    {
        if (audioSource.volume > 0.5f)
        {
            volumeObject.GetComponent<Image>().sprite = volumeImage[0];
        }
        else if (audioSource.volume <= 0.5f && audioSource.volume > 0)
        {
            volumeObject.GetComponent<Image>().sprite = volumeImage[1];
        }
        else
        {
            volumeObject.GetComponent<Image>().sprite = volumeImage[2];
        }
    }

#endif

}
