using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Global : MonoBehaviour
{
    public static GameObject notify;
    static GameObject notifyBox;
    static Text NotifyText;
    static Button NotifyButton;
    static Text NotifyButtonText;

    public static void showNotifyBox(string error, string button)
    {
        if (!GameObject.Find("NotifyBox"))
        {
            notifyBox = Instantiate(notify, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            notifyBox.transform.parent = GameObject.Find("Canvas").gameObject.transform;
            notifyBox.name = "NotifyBox";
        }

        NotifyText = GameObject.Find("NotifyBoxText").GetComponent<Text>();
        NotifyText.text = error;

        NotifyButtonText = GameObject.Find("NotifyButtonText").GetComponent<Text>();
        NotifyButtonText.text = button;

        NotifyButton = GameObject.Find("NotifyButton").GetComponent<Button>();
        NotifyButton.onClick.AddListener(closeNotify);
    }

    public static void closeNotify()
    {
        Destroy(notifyBox);
    }
}