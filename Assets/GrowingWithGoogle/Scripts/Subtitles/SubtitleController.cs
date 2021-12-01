using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Text/Subtitle Controller")]
public class SubtitleController : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    private bool isPlaying = false;
    private bool isFading = false;
    public bool manualTimestampOverride = false;
    public Text text;
    private string currFileName;
    public float lingerFull = 1.0f;
    public float lingerFade = 1.0f;
    private float lingerTotal;
    private float movieT = 0f;
    private float tLinger = 0f;
    private List<SubtitleUnit> subs;
    private static Dictionary<String, List<SubtitleUnit>> allSubs;
    private static Dictionary<String, float> subTimes;
    private int subsIndex = 0;
    private SubtitleUnit su;
    public Color subtitleColor = Color.white;

    public int SubsIndex
    {
        get
        {
            return subsIndex;
        }
        set
        {
            subsIndex = value;
            if (subs != null && subsIndex < subs.Count)
            {
                su = (SubtitleUnit)subs[subsIndex];
            }
        }
    }

    void Start()
    {
        lingerTotal = lingerFull + lingerFade;
        if (text == null)
        {
            text = GetComponent<Text>();
        }

    }

    private void OnEnable()
    {

        lingerTotal = lingerFull + lingerFade;
        if (text == null)
        {
            text = GetComponent<Text>();
        }

    }

    // call from FeatureScreen OnEnable to ensure that the file name has been set before calling
    public void SetFileName(string srtFileName)
    {
        if (srtFileName != "")
        {
            currFileName = srtFileName;
            LoadSubtitles(currFileName, true);
            Play();
        }
    }

    //private IEnumerator loadFromFileOrWWW(string filename, bool isInResources)
    private void LoadFromFile(string filename, bool isInResources)
    {
        subs = null;
        string fileText = "";
        if (isInResources)
        {
            TextAsset asset = Resources.Load(filename) as TextAsset;
            if (asset != null)
            {
                fileText = asset.text;
            }
            else
            {
                Debug.LogError("Could not find the subtitle file for " + filename + ". Is the file extension set to .txt? Is the file in a resources folder?");
            }
        }
        //else
        //{
        //    if (filename[0] == '/')
        //    {
        //        filename = filename.Remove(0, 1);
        //    }
        //    string usefilename = filename.Replace('\\', '/');
        //    WWW getFile = new WWW("file:///" + usefilename);
        //    while (!getFile.isDone)
        //    {
        //        yield return null;
        //    }
        //    if (String.IsNullOrEmpty(getFile.error))
        //    {
        //        fileText = getFile.text;
        //    }
        //    else
        //    {
        //        Debug.LogError("Could not find the subtitle file for " + filename + ". Is it the full path? Is it a file on the local machine?");
        //        Debug.LogError(getFile.error);
        //    }
        //}
        Stream s = GenerateStreamFromString(fileText);
        if (s != null)
        {
            SubtitleParser sp = new SubtitleParser();
            subs = sp.ParseStream(s, Encoding.UTF8);
            allSubs.Add(filename, subs);
            SubsIndex = 0;
        }
        else
        {
            Debug.LogError("Failed to load subs for " + filename + ".");
        }
    }

    public void LoadSubtitles(string filename, bool isInResources = false)
    {
        if (allSubs == null)
        {
            allSubs = new Dictionary<string, List<SubtitleUnit>>();
        }
        if (subTimes == null)
        {
            subTimes = new Dictionary<string, float>();
        }
        currFileName = filename;
        // check if file has been loaded already
        if (allSubs.ContainsKey(filename))
        {
            allSubs.TryGetValue(filename, out subs);
            movieT = 0f;
            subTimes.TryGetValue(filename, out movieT);
        }
        else
        {
            //StartCoroutine(LoadFromFile(filename, isInResources));
            LoadFromFile(filename, isInResources);
        }
    }

    private MemoryStream GenerateStreamFromString(string value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
    }

    public void Stop()
    {
        if (manualTimestampOverride)
        {
            print("Unnecessary call to Stop(); ManualTimestampOverride is on");
        }
        isPlaying = false;
        SubsIndex = 0;
        movieT = 0;
    }

    public void Play()
    {
        if (manualTimestampOverride)
        {
            print("Unnecessary call to Play(); ManualTimestampOverride is on");
        }
        isFading = false;
        isPlaying = true;
        tLinger = 0;
        SubsIndex = 0;
    }

    public void Pause()
    {
        if (manualTimestampOverride)
        {
            print("Unnecessary call to Pause(); ManualTimestampOverride is on");
        }
        isFading = true;
        isPlaying = false;
        if (subTimes == null)
        {
            subTimes = new Dictionary<string, float>();
        }
        if (subTimes.ContainsKey(currFileName))
        {
            subTimes.Remove(currFileName);
        }
        subTimes.Add(currFileName, movieT);
    }

    public void UnPause()
    {
        if (manualTimestampOverride)
        {
            print("Unnecessary call to UnPause(); ManualTimestampOverride is on");
        }
        isFading = false;
        isPlaying = true;
        tLinger = 0;
        SubsIndex = 0;
    }

    public void setTimestamp(float timestamp)
    {
        movieT = timestamp;
        if (subs != null)
        {
            DisplaySubtitle();
        }
    }

    void DisplaySubtitle()
    {
        text.color = subtitleColor;
        if (su == null)
        {
            su = (SubtitleUnit)subs[subsIndex];
        }
        while (movieT > su.EndTime && subsIndex < subs.Count - 1)
        {
            subsIndex++;
            su = (SubtitleUnit)subs[subsIndex];
        }
        while (movieT < su.StartTime && subsIndex > 0)
        {
            subsIndex--;
            su = (SubtitleUnit)subs[subsIndex];
        }
        if (movieT >= su.StartTime && movieT < su.EndTime)
        {
            bgImage.enabled = true;
            string s = "";
            for (int i = 0; i < su.Lines.Count; i++)
            {
                s += (string)su.Lines[i];
                if (i + 1 < su.Lines.Count)
                {
                    s += "\r\n";
                }
            }
            text.text = s;
        }
        else
        {
            text.text = "";
            bgImage.enabled = false;
        }
    }

    void Update()
    {
        if (!manualTimestampOverride)
        {
            if (isPlaying && subs != null)
            {
                movieT += Time.deltaTime;
                DisplaySubtitle();
            }
        }
        if (!isPlaying && isFading)
        {
            tLinger += Time.deltaTime;
            if (tLinger > lingerFull && tLinger < lingerTotal)
            {
                Color nColor = new Color(text.color.r, text.color.g, text.color.b, (lingerTotal - tLinger) / lingerFade);
                text.color = nColor;
            }
            if (tLinger > lingerTotal)
            {
                text.text = "";
                text.color = subtitleColor;
                isFading = false;
                tLinger = 0f;
            }
        }
    }
}

