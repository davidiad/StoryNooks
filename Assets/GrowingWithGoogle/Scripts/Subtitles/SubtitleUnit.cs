using System;
using System.Collections;

public class SubtitleUnit
{
    float startTime;
    float endTime;
    ArrayList display;
    public SubtitleUnit()
    {
        startTime = 0;
        endTime = 0;
    }

    public float StartTime
    {
        get
        {
            return startTime;
        }
        set
        {
            startTime = value;
        }
    }

    public float EndTime
    {
        get
        {
            return endTime;
        }
        set
        {
            endTime = value;
        }
    }

    public ArrayList Lines
    {
        get
        {
            if (display == null)
            {
                display = new ArrayList();
            }
            return display;
        }
        set
        {
            display = value;
        }
    }
}


