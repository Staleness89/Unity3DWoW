using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.Shared;
using UnityEngine.UI;
using Client.World;
using Client.World.Network;
using UnityEngine.SceneManagement;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class WorldHelper : MonoBehaviour
{

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    
    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }

        if (Exchange.connected)
        {
            if (!Exchange.IsConnected)
            {
                Exchange.gameClient.Exit();
                Exchange.disconnected = true;
                SceneManager.LoadScene("Main");
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }
    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }


    private static WorldHelper _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static WorldHelper Instance()
    {
        if (!Exists())
        {
            throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return _instance;
    }

    public IEnumerator AddToWorldOnTheMainThread(WorldObject obj)
    {
        if (obj.GUID != Exchange.SelectedCharacter.GUID)
        {
            UnityEngine.GameObject tempSpawn = Instantiate(Resources.Load("Cube") as UnityEngine.GameObject, new Vector3(obj.Movement.Position.x, obj.Movement.Position.z + 5f, obj.Movement.Position.y), Quaternion.identity);
            tempSpawn.name = obj.GUID.ToString();

            Transform[] ts = tempSpawn.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "targetSelectUnderlay")
                {
                    t.gameObject.name = obj.GUID.ToString() + "targetSelectUnderlay";

                    UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().enabled = false;
                }     
                
            }
        }
        yield return null;
    }

    public void AddToWorld(WorldObject obj)
    {
        Instance().Enqueue(AddToWorldOnTheMainThread(obj));
    }

    public IEnumerator RemoveFromWorldOnTheMainThread(WorldObject obj)
    {
        Destroy(UnityEngine.GameObject.Find(obj.GUID.ToString()));
        yield return null;
    }

    public void RemoveFromWorld(WorldObject obj)
    {
        Instance().Enqueue(RemoveFromWorldOnTheMainThread(obj));
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void setTarget(WorldObject obj)
    {
        if (UnityEngine.GameObject.Find("targetFrame"))
            Destroy(UnityEngine.GameObject.Find("targetFrame"));

        Exchange.gameClient.Player.Target = obj;

        var frame = Resources.Load("targetFrame") as UnityEngine.GameObject;
        UnityEngine.GameObject targetFrame = Instantiate(frame, new Vector3(frame.transform.position.x, frame.transform.position.y, 0), Quaternion.identity);
        targetFrame.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform, false);
        targetFrame.transform.localScale = new Vector3(1, 1, 1);
        targetFrame.name = "targetFrame";

        Transform[] ts = targetFrame.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == "targetName")
            {
                Text temo = t.gameObject.GetComponent<Text>();
                temo.text = obj.Name;
            }

            if (obj is Unit)
            {
                Unit unit = obj as Unit;
                if (t.gameObject.name == "targetLevel")
                {
                    Text temo = t.gameObject.GetComponent<Text>();
                    if (obj is Unit)
                    {
                        temo.text = unit.Level.ToString();
                    }
                }

                if (t.gameObject.name == "targetHealth")
                {
                    Text temp = t.gameObject.GetComponent<Text>();
                    temp.text = unit.Health.ToString();
                }
            }

            if (obj is Player)
            {
                Player player = obj as Player;
                if (t.gameObject.name == "targetLevel")
                {
                    Text temo = t.gameObject.GetComponent<Text>();
                    temo.text = player.Level.ToString();
                }

                if (t.gameObject.name == "targetHealth")
                {
                    Text temp = t.gameObject.GetComponent<Text>();
                    temp.text = player.Health.ToString();
                }
            }
        }

        var packet = new OutPacket(WorldCommand.CMSG_SET_SELECTION);
        packet.Write(obj.GUID);
        Exchange.gameClient.SendPacket(packet);
    }

    public void removeTarget(WorldObject obj)
    {
        if (UnityEngine.GameObject.Find("targetFrame"))
            Destroy(UnityEngine.GameObject.Find("targetFrame"));

        Exchange.gameClient.Player.Target = null;
    }

    public AudioSource playAudio(WorldObject obj, string path)
    {
        AudioSource newAudio = UnityEngine.GameObject.Find(obj.GUID.ToString()).AddComponent<AudioSource>();

        if (newAudio.isPlaying)
        {
            return null;
        }
        AudioClip sound = Resources.Load("Sounds/" + path) as AudioClip;

        newAudio.clip = sound;
        newAudio.loop = false;
        newAudio.playOnAwake = false;
        newAudio.volume = 1;

        return newAudio;
    }
}