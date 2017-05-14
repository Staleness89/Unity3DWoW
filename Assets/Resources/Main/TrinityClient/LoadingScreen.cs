using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    Image texture;
    public System.Timers.Timer uTimer = new System.Timers.Timer();
    public System.Timers.Timer uTimerw = new System.Timers.Timer();
    int LoadBar = 1;
    public static int Loaded = 0;
    float yPixelDistance;
    float xPixelDistance;

    // Use this for initialization
    void Start()
    {

        texture = GameObject.Find("barProgress").GetComponent<Image>();
        uTimer.Elapsed += new ElapsedEventHandler(Pinging);
        uTimer.Interval = 300;
        uTimer.Enabled = true;

        uTimerw.Elapsed += new ElapsedEventHandler(Pingup);
        uTimerw.Interval = 100;
        uTimerw.Enabled = true;
    }

    public void Pinging(object source, ElapsedEventArgs e)
    {
        if (LoadBar < 150)
        {
            LoadBar = LoadBar + 0;
        }
        else
        {
            LoadBar = LoadBar + 7;
        }

        if (LoadBar > 320)
        {
            LoadBar = 372;
        }

        if (LoadBar == 372)
        {
            uTimer.Enabled = false;
            uTimer.Stop();
            Loaded = 1;
            return;
        }
    }

    public void Pingup(object source, ElapsedEventArgs e)
    {
        if (LoadBar > 150)
        {
            uTimerw.Enabled = false;
            uTimerw.Stop();
            return;
        }

        LoadBar = LoadBar + 4;
    }

    // Update is called once per frame
    void Update()
    {

        texture.rectTransform.sizeDelta = new Vector2(LoadBar, 15);

    }
}
