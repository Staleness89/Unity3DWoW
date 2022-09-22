using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class PacketCrypt
{
    /// <summary>
    /// The amount of bytes to drop from the stream initially.
    /// 
    /// This is to resist the FMS attack.
    /// </summary>
    public const int DropN = 1024;

    /// <summary>
    /// This is the key the client uses to encrypt its packets
    /// This is also the key the server uses to decrypt the packets
    /// </summary>
    private static readonly byte[] ServerDecryptionKey =
        {
                0xCC, 0x98, 0xAE, 0x04, 0xE8, 0x97, 0xEA, 0xCA,
                0x12, 0xDD, 0xC0, 0x93, 0x42, 0x91, 0x53, 0x57
            };

    /// <summary>
    /// This is the key the client uses to decrypt server packets
    /// This is also the key the server uses to encrypt the packets
    /// </summary>
    private static readonly byte[] ServerEncryptionKey =
        {
                0xC2, 0xB3, 0x72, 0x3C, 0xC6, 0xAE, 0xD9, 0xB5,
                0x34, 0x3C, 0x53, 0xEE, 0x2F, 0x43, 0x67, 0xCE
            };

    // This is valid as HMAC-SHA1 transforms can be reused
    static readonly HMACSHA1 s_decryptClientDataHMAC = new HMACSHA1(ServerDecryptionKey);
    static readonly HMACSHA1 s_encryptServerDataHMAC = new HMACSHA1(ServerEncryptionKey);

    /// <summary>
    /// Encrypts data sent to the client
    /// </summary>
    private readonly ARC4 encryptServerData;
    /// <summary>
    /// Decrypts data sent from the client
    /// </summary>
    private readonly ARC4 decryptClientData;

    private bool Initialised;

    public void Init()
    {
        Initialised = true;
    }

    public byte _send_i, _send_j, _recv_i, _recv_j;
    protected byte[] _key;
    public bool _initialized = false;

    public void init(byte[] key)
    {
        _key = key;
        _send_i = _send_j = _recv_i = _recv_j = 0;
        _initialized = true;
    }

    public byte[] decrypt(byte[] data, int length)
    {
        if (!_initialized) return data;

        for (int t = 0; t < length; t++)
        {
            _recv_i %= (byte)_key.Length;
            byte x = (byte)((data[t] - _recv_j) ^ _key[_recv_i]);
            ++_recv_i;
            _recv_j = data[t];
            data[t] = x;
        }
        return data;
    }

    public byte[] encrypt(byte[] data)
    {
        if (!_initialized) return data;

        for (int t = 0; t < data.Length; t++)
        {
            _send_i %= (byte)_key.Length;
            byte x = (byte)((data[t] ^ _key[_send_i]) + _send_j);
            ++_send_i;
            data[t] = _send_j = x;
        }
        return data;
    }
    public PacketCrypt(byte[] sessionKey)
    {
        var encryptHash = s_encryptServerDataHMAC.ComputeHash(sessionKey);
        var decryptHash = s_decryptClientDataHMAC.ComputeHash(sessionKey);

        // Used by the client to decrypt packets sent by the server
        //var decryptServerData = new ARC4(encryptHash); // CLIENT-SIDE
        // Used by the server to decrypt packets sent by the client
        decryptClientData = new ARC4(decryptHash); // SERVER-SIDE
                                                   // Used by the server to encrypt packets sent to the client
        encryptServerData = new ARC4(encryptHash); // SERVER-SIDE
                                                   // Used by the client to encrypt packets sent to the server
                                                   //var encryptClientData = new ARC4(decryptHash); // CLIENT-SIDE

        // Use the 2 encryption objects to generate a common starting point
        var syncBuffer = new byte[DropN];
        encryptServerData.Process(syncBuffer, 0, syncBuffer.Length);
        //encryptClientData.Process(syncBuffer, 0, syncBuffer.Length);

        // Use the 2 decryption objects to generate a common starting point
        syncBuffer = new byte[DropN];
        //decryptServerData.Process(syncBuffer, 0, syncBuffer.Length);
        decryptClientData.Process(syncBuffer, 0, syncBuffer.Length);
    }

    public void Decrypt(byte[] data, int start, int count)
    {
        if (Initialised)
            decryptClientData.Process(data, start, count);
    }

    public void Encrypt(byte[] data, int start, int count)
    {
        if (Initialised)
            encryptServerData.Process(data, start, count);
    }
}