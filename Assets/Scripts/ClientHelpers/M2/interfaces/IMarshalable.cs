using System.IO;


    /// <summary>
    ///     Simple I/O interface to ensure objects can be marshalled.
    ///     The version fields allows to pass hints around to dynamically parse versioned formats.
    /// </summary>
    public interface IMarshalable
    {
        void Load(BinaryReader stream, M2.Format version);
        void Save(BinaryWriter stream, M2.Format version);
    }
