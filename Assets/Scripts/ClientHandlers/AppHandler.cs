using Foole.Mpq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AppHandler : MonoBehaviour
{
    public static AppHandler Instance;
    public Image _image;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    
    public string REALM_LIST_ADDRESS = " ";
    public string LAST_KNOWN_REALMNAME = " ";
    public int LAST_KNOWN_REALM_PORT = 0;
    public string LAST_KNOWN_MPQ_DATA_FOLDER = " ";
    public string WEBSITE_LINK = " ";
    public string MANAGE_ACCOUNT_LINK = " ";

    public List<MpqArchive> LoadedMPQs;

    private Dictionary<string, MpqArchive> adtEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> wdtEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> wdlEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> m2Entries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> wmoEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> skinEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> animEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> dbcEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> wdbEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> blpEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> blsEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, MpqArchive> audioEntries = new Dictionary<string, MpqArchive>();
    private Dictionary<string, Texture2D> loadedCursors = new Dictionary<string, Texture2D>();
    private Dictionary<string, Sprite> loadedLoadingScreens = new Dictionary<string, Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
            return;

        Instance = this;

        REALM_LIST_ADDRESS = "127.0.0.1";
        LAST_KNOWN_REALM_PORT = 3724;
        LAST_KNOWN_MPQ_DATA_FOLDER = @Application.dataPath + "/Data/";
        WEBSITE_LINK = @"https://worldofwarcraft.com/en-us/";
        MANAGE_ACCOUNT_LINK = @"https://account.battle.net/";

        ReadRealmlistFile();

        LoadedMPQs = new List<MpqArchive>();
        LoadMPQFiles();

        SetPointer(@"Interface\CURSOR\Point.blp");
    }
    public void LoadMPQFiles()
    {
        string[] _mpqFiles = Directory.GetFiles(LAST_KNOWN_MPQ_DATA_FOLDER, "*.MPQ", SearchOption.AllDirectories);
        for (int i = _mpqFiles.Length - 1; i >= 0; i--)
        {
            MpqArchive archive = new MpqArchive(_mpqFiles[i]);
            archive.AddListfileFilenames();
            LoadedMPQs.Add(archive);

            foreach (MpqEntry entry in archive._entries)
            {
                if (entry.Filename == null)
                    continue;

                string ext = Path.GetExtension(entry.Filename);
                string fileName = entry.Filename.ToUpper();

                switch (ext.ToUpper())
                {
                    case ".ADT":
                        if (adtEntries.ContainsKey(fileName))
                            adtEntries[fileName] = archive;
                        else
                            adtEntries.Add(fileName, archive);
                        break;
                    case ".WDT":
                        if (wdtEntries.ContainsKey(fileName))
                            wdtEntries[fileName] = archive;
                        else
                            wdtEntries.Add(fileName, archive);
                        break;
                    case ".WDL":
                        if (wdlEntries.ContainsKey(fileName))
                            wdlEntries[fileName] = archive;
                        else
                            wdlEntries.Add(fileName, archive);
                        break;
                    case ".WMO":
                        if (wmoEntries.ContainsKey(fileName))
                            wmoEntries[fileName] = archive;
                        else
                            wmoEntries.Add(fileName, archive);
                        break;
                    case ".M2":
                        if (m2Entries.ContainsKey(fileName))
                            m2Entries[fileName] = archive;
                        else
                            m2Entries.Add(fileName, archive);
                        break;
                    case ".BLP":
                        if (blpEntries.ContainsKey(fileName))
                            blpEntries[fileName] = archive;
                        else
                            blpEntries.Add(fileName, archive);
                        break;
                    case ".BLS":
                        if (blsEntries.ContainsKey(fileName))
                            blsEntries[fileName] = archive;
                        else
                            blsEntries.Add(fileName, archive);
                        break;
                    case ".SKIN":
                        if (skinEntries.ContainsKey(fileName))
                            skinEntries[fileName] = archive;
                        else
                            skinEntries.Add(fileName, archive);
                        break;
                    case ".DBC":
                        if (dbcEntries.ContainsKey(fileName))
                            dbcEntries[fileName] = archive;
                        else
                            dbcEntries.Add(fileName, archive);
                        break;
                    case ".ANIM":
                        if (animEntries.ContainsKey(fileName))
                            animEntries[fileName] = archive;
                        else
                            animEntries.Add(fileName, archive);
                        break;
                    case ".WAV":
                    case ".MP3":
                        if (audioEntries.ContainsKey(fileName))
                            audioEntries[fileName] = archive;
                        else
                            audioEntries.Add(fileName, archive);
                        break;
                }
            }
        }
    }
    public MpqStream SearchMPQ(string text)
    {
        text = text.ToUpper();
        string ext = Path.GetExtension(text);

        MpqArchive value = null;

        switch (ext.ToUpper())
        {
            case ".ADT":
                if (adtEntries.ContainsKey(text))
                    adtEntries.TryGetValue(text, out value);
                break;
            case ".WDT":
                if (wdtEntries.ContainsKey(text))
                    wdtEntries.TryGetValue(text, out value);
                break;
            case ".WDL":
                if (wdlEntries.ContainsKey(text))
                    wdlEntries.TryGetValue(text, out value);
                break;
            case ".WMO":
                if (wmoEntries.ContainsKey(text))
                    wmoEntries.TryGetValue(text, out value);
                break;
            case ".M2":
                if (m2Entries.ContainsKey(text))
                    m2Entries.TryGetValue(text, out value);
                break;
            case ".BLP":
                if (blpEntries.ContainsKey(text))
                    blpEntries.TryGetValue(text, out value);
                break;
            case ".BLS":
                if (blsEntries.ContainsKey(text))
                    blsEntries.TryGetValue(text, out value);
                break;
            case ".SKIN":
                if (skinEntries.ContainsKey(text))
                    skinEntries.TryGetValue(text, out value);
                break;
            case ".DBC":
                if (dbcEntries.ContainsKey(text))
                    dbcEntries.TryGetValue(text, out value);
                break;
            case ".ANIM":
                if (animEntries.ContainsKey(text))
                    animEntries.TryGetValue(text, out value);
                break;
            case ".WAV":
            case ".MP3":
                if (audioEntries.ContainsKey(text))
                    audioEntries.TryGetValue(text, out value);
                break;
        }

        if (value != null)
        {
            return value.OpenFile(text);
        }
        else
            return null;
    }
    void SetPointer(string pointerLocation)
    {
        Texture2D newPointer = null;

        if (loadedCursors.ContainsKey(pointerLocation.ToUpper()))
            newPointer = loadedCursors[pointerLocation.ToUpper()];
        else
        {
            loadedCursors.Add(pointerLocation.ToUpper(), BLPLoader.ToTex(SearchMPQ(pointerLocation)));
            newPointer = loadedCursors[pointerLocation.ToUpper()];
        }
        
        Cursor.SetCursor(newPointer, hotSpot, cursorMode);
    }
    public void ReadRealmlistFile()
    {
        string path = @Application.dataPath + "/data.wtf";
        if (!File.Exists(path))
        {
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
            }
        }

        string[] Config = File.ReadAllLines(path);

        foreach (string line in Config)
        {
            if (line.Contains("REALM_LIST_ADDRESS "))
            {
                REALM_LIST_ADDRESS = line.Substring(19);
            }
            if (line.Contains("LAST_KNOWN_REALMNAME "))
            {
                LAST_KNOWN_REALMNAME = line.Substring(21);
            }
            if (line.Contains("LAST_KNOWN_REALM_PORT "))
            {
                LAST_KNOWN_REALM_PORT = int.Parse(line.Substring(22));
            }
            if (line.Contains("LAST_KNOWN_MPQ_DATA_FOLDER "))
            {
                LAST_KNOWN_MPQ_DATA_FOLDER = line.Substring(27);
            }
            if (line.Contains("WEBSITE_LINK "))
            {
                WEBSITE_LINK = line.Substring(13);
            }
            if (line.Contains("MANAGE_ACCOUNT_LINK "))
            {
                MANAGE_ACCOUNT_LINK = line.Substring(20);
            }
        }
    }
    public void WriteRealmlistFile()
    {
        string path = @Application.dataPath + "/data.wtf";
        if (!File.Exists(path))
        {
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
            }
        }
        else
        {
            File.Delete(path);
            File.Create(path).Close();

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALMNAME " + LAST_KNOWN_REALMNAME);
                w.WriteLine("LAST_KNOWN_REALM_PORT " + LAST_KNOWN_REALM_PORT.ToString());
                w.WriteLine("LAST_KNOWN_MPQ_DATA_FOLDER " + LAST_KNOWN_MPQ_DATA_FOLDER);
                w.WriteLine("WEBSITE_LINK " + WEBSITE_LINK);
                w.WriteLine("MANAGE_ACCOUNT_LINK " + MANAGE_ACCOUNT_LINK);
            }
        }
    }
}
