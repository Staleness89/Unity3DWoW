using Assets.Scripts.World.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class CombatMgr
{
    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    public static extern uint MM_GetTime();

    Thread loop;
    public List<Assets.Scripts.World.Object> Targets = new List<Assets.Scripts.World.Object>();
    UInt32 lastUpdateTime;

    World client;
    ObjectMgr objectMgr;
    MovementMgr movementMgr;
    Boolean isFighting = false;


    public CombatMgr(World Client)
    {
        objectMgr = Client.objectMgr;
        movementMgr = Client.movementMgr;
        client = Client;
    }

    public void Start()
    {
        try
        {
            lastUpdateTime = MM_GetTime();

            loop = new Thread(Loop);
            loop.IsBackground = true;
            loop.Start();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Exception Occured");
            Debug.LogWarning("Message: " + ex.Message);
            Debug.LogWarning("Stacktrace: " + ex.StackTrace);
        }
    }

    public void Stop()
    {
        if (loop != null)
            loop.Abort();
    }

    void Loop()
    {
        while (true)
        {
            try
            {
                if (Targets.Count > 0)
                {
                    Assets.Scripts.World.Object target = Targets.First();
                    float dist = TerrainMgr.CalculateDistance(objectMgr.getPlayerObject().Position, target.Position);
                    if (dist > 1)
                    {
                        movementMgr.Waypoints.Add(target.Position);
                    }
                    else if (dist < 1 && !isFighting)
                    {
                        //client.Attack(target);
                        isFighting = true;
                    }
                    else if (isFighting && target.Health < 0)
                    {
                        isFighting = false;
                        Targets.Remove(target);
                    }
                    else if (isFighting && target.Health > 0)
                    {
                        Debug.LogWarning(target.Health.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Exception Occured");
                Debug.LogWarning("Message: " + ex.Message);
                Debug.LogWarning("Stacktrace: " + ex.StackTrace);
            }
        }
    }
}