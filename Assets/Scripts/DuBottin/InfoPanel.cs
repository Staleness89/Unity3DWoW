using Assets.Scripts.Shared;
using Client;
using Client.World.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    public WorldObject obj;
    Unit unit;
    Player player;
    Image targetHealth;
    // Use this for initialization
    void Start () {
        var guid = Convert.ToUInt64(this.name);
        obj = Exchange.authClient.Objects[guid];        
    }

    public float RevertFromRadians(float angle)
    {
        return (float)(Math.PI * 180) / angle;
    }

    // Update is called once per frame
    void Update ()
    {
        if (UnityEngine.GameObject.Find(obj .GUID.ToString()).GetComponent<Transform>().position != new Vector3(obj.Movement.Position.x, obj.Movement.Position.z, obj.Movement.Position.y) || obj.Movement.Orientation != -(Exchange.authClient.movementMgr.ConvertToRadians(transform.eulerAngles.y)) - 4.6f)
        {
            Vector3 pos = Vector3.MoveTowards(UnityEngine.GameObject.Find(obj.GUID.ToString()).GetComponent<Transform>().position, new Vector3(obj.Movement.Position.x, obj.Movement.Position.z, obj.Movement.Position.y), obj.MovementSpeed() * Time.deltaTime);
            UnityEngine.GameObject.Find(obj.GUID.ToString()).GetComponent<Rigidbody>().MovePosition(pos);
            UnityEngine.GameObject.Find(obj.GUID.ToString()).transform.rotation = Quaternion.Euler(0, (RevertFromRadians(obj.Movement.Orientation)) + 4.6f, 0);            
        }

        if (UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay"))
        {
            UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").transform.Rotate(0, 0, 350f * Time.deltaTime);
        }
    }

    public void OnMouseOver()
    {
        if (this.obj is Unit)
        {
            Unit unit = this.obj as Unit;

            UnitFlags flags = (UnitFlags)unit.UnitFlags;
            UnitFlags2 flags2 = (UnitFlags2)unit.UnitFlags2;
            NPCFlags NpcFlags = (NPCFlags)unit.NpcFlags;

            if (NpcFlags.HasAnyFlag(NPCFlags.Gossip))
            {
                float distance = Vector3.Distance(UnityEngine.GameObject.Find(Exchange.authClient.Player.GUID.ToString()).transform.position, UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString()).transform.position);

                if (distance > 4)
                {
                    Texture2D texture2D = Resources.Load("Images/login/speak") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    Texture2D texture2D = Resources.Load("Images/login/speak") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }
            }

            if (NpcFlags.HasAnyFlag(NPCFlags.QuestGiver))
            {
                float distance = Vector3.Distance(UnityEngine.GameObject.Find(Exchange.authClient.Player.GUID.ToString()).transform.position, UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString()).transform.position);
                
                if (distance > 4)
                {
                    Texture2D texture2D = Resources.Load("Images/login/unablequest") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    Texture2D texture2D = Resources.Load("Images/login/quest") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }                
            }

            if (NpcFlags.HasAnyFlag(NPCFlags.SpiritHealer))
            {
                float distance = Vector3.Distance(UnityEngine.GameObject.Find(Exchange.authClient.Player.GUID.ToString()).transform.position, UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString()).transform.position);

                if (distance > 4)
                {
                    Texture2D texture2D = Resources.Load("Images/login/speak") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    Texture2D texture2D = Resources.Load("Images/login/speak") as Texture2D;
                    Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
                }
                
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (this.obj is GameObject)
                return;

            if (Exchange.authClient.Player.Target == this.obj)
                return;

            if(Exchange.authClient.Player.Target != null)
                UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().enabled = false;

            UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().enabled = true;
            //UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().color = Color.red;
            Exchange.authClient.ThreadHelper.playAudio(obj, "interface/iselecttarget").Play();
            Exchange.authClient.ThreadHelper.setTarget(this.obj);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (Exchange.authClient.Player.Target != this.obj)
            {
                if (Exchange.authClient.Player.Target != null)
                    UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().enabled = false;

                UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().enabled = true;
                //UnityEngine.GameObject.Find(obj.GUID.ToString() + "targetSelectUnderlay").GetComponent<SpriteRenderer>().color = Color.red;
                Exchange.authClient.ThreadHelper.playAudio(obj, "interface/iselecttarget").Play();
                Exchange.authClient.ThreadHelper.setTarget(this.obj);
            }

            float distance = Vector3.Distance(UnityEngine.GameObject.Find(Exchange.authClient.Player.GUID.ToString()).transform.position, UnityEngine.GameObject.Find(Exchange.authClient.Player.Target.GUID.ToString()).transform.position);

            if (distance > 4)
            {
                System.Random random = new System.Random();
                int[] AudioFile = new int[3] { 2, 4, 5 };
                int slot = AudioFile[random.Next(0, AudioFile.Length)];

                Exchange.authClient.ThreadHelper.playAudio(Exchange.authClient.Player, "character/" +
                Exchange.authClient.Player.Race.ToString() + "/" +
                Exchange.authClient.Player.Race.ToString() +
                Exchange.authClient.Player.Gender.ToString() + "errormessages/" +
                Exchange.authClient.Player.Race.ToString() +
                Exchange.authClient.Player.Gender.ToString() + "_err_outofrange0" + slot.ToString()).Play();

                return;
            }
        }
                
        if (!UnityEngine.GameObject.Find("InfoPanel"))
        {
            UnityEngine.GameObject tempAuth = Instantiate(Resources.Load("InfoPanel") as UnityEngine.GameObject, new Vector3(Screen.width - 50, 100, 0), Quaternion.identity);
            tempAuth.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            tempAuth.transform.localScale = new Vector3(1, 1, 1);
            tempAuth.name = "InfoPanel";

            targetHealth = UnityEngine.GameObject.Find("targethealthBarInfo").GetComponent<Image>();
            targetHealth.fillAmount = 0f;

            Transform[] ts = tempAuth.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "InfoPanelName")
                {
                    Text temo = t.gameObject.GetComponent<Text>();
                    temo.text = obj.Name;
                }

                if (obj is Unit)
                {
                    unit = obj as Unit;
                    if (t.gameObject.name == "InfoPanelLevel")
                    {
                        Text temo = t.gameObject.GetComponent<Text>();
                        if (obj is Unit)
                        {
                            if(unit.Health <= 0)
                            {
                                temo.text = unit.Level.ToString() + " (Corpse)";
                            }
                            else
                            {
                                temo.text = unit.Level.ToString();
                            }
                        }
                    }

                    uint healthPercent = (unit.Health * 200 + unit.MaxHealth) / (unit.MaxHealth * 2);
                    targetHealth.fillAmount = healthPercent / 100f;
                }

                if (obj is Player)
                {
                    player = obj as Player;
                    if (t.gameObject.name == "InfoPanelLevel")
                    {
                        Text temo = t.gameObject.GetComponent<Text>();
                        temo.text = player.Level.ToString() + " " + player.Race + " " + player.Class;
                    }

                    uint healthPercent = (player.Health * 200 + player.MaxHealth) / (player.MaxHealth * 2);
                    targetHealth.fillAmount = healthPercent / 100f;
                }
            }
        }
    }

    public void OnMouseExit()
    {
        Texture2D texture2D = Resources.Load("Images/login/point") as Texture2D;

        Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
        Destroy(UnityEngine.GameObject.Find("InfoPanel"));
    }
}
