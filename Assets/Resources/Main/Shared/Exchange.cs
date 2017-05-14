using Assets.Scripts.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Exchange
{
    public static Realm currRealm;
    public static Realm[] realms;
    public static AuthSocket authClient = null;
    public static World worldClient = null;
    public static Character newCharacter;
    public static GameObject maleCharacter;
    public static Texture2D[] Pointers = new Texture2D[5];

}
