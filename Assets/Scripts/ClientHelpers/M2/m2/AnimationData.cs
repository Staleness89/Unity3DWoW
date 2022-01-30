using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
//using Microsoft.VisualBasic.FileIO;

    /// <summary>
    ///     Static class providing friendly access to AnimationData.dbc
    /// </summary>
    public static class AnimationData
    {
        public static readonly IDictionary<ushort, ushort> Fallback;
        public static readonly IDictionary<string, ushort> NameToId;
        public static readonly IDictionary<ushort, string> IdToName;
        public static readonly ISet<ushort> PlayThenStop;
        public static readonly ISet<ushort> PlayBackwards;

        /*static AnimationData()
        {
            Fallback = new Dictionary<ushort, ushort>();
            NameToId = new Dictionary<string, ushort>();
            IdToName = new Dictionary<ushort, string>();
            PlayThenStop = new HashSet<ushort>();
            PlayBackwards = new HashSet<ushort>();
            var assembly = Assembly.GetExecutingAssembly();
            var embeddedStream = assembly.GetManifestResourceStream("M2Lib.src.csv.AnimationData.csv");
            Debug.Assert(embeddedStream != null, "Could not open embedded ressource AnimationData");
            var csvParser = new TextFieldParser(embeddedStream) {CommentTokens = new[] {"#"}};
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;
            csvParser.ReadLine(); // Skip first line
            while (!csvParser.EndOfData)
            {
                var fields = csvParser.ReadFields();
                Debug.Assert(fields != null);
                var id = Convert.ToUInt16(fields[0]);
                var name = fields[1];
                var fallback = Convert.ToUInt16(fields[3]);
                Fallback[id] = fallback;
                NameToId[name] = id;
                IdToName[id] = name;
            }
            csvParser.Close();
            ushort[] playThenStopValues =
            {
                NameToId["Dead"],
                NameToId["SitGround"],
                NameToId["Sleep"],
                NameToId["KneelLoop"],
                NameToId["UseStandingLoop"],
                NameToId["Drowned"],
                NameToId["LootHold"]
            };
            foreach (var value in playThenStopValues) PlayThenStop.Add(value);
            ushort[] playBackwardsValues =
            {
                NameToId["Walkbackwards"],
                NameToId["SwimBackwards"],
                NameToId["SleepUp"],
                NameToId["LootUp"]
            };
            foreach (var value in playBackwardsValues) PlayBackwards.Add(value);

            //TODO FIXME There a loops by following the fallbacks in AnimationData.dbc. Happens with Close and FlyClose.
            Fallback[146] = 0;//Close
            Fallback[375] = 0;//FlyClose
        }*/
    }
