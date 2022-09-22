using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


public class Sha1Hash
{
    private SHA1 mSha;
    private static byte[] ZeroArray = new byte[0];

    public Sha1Hash()
    {
        mSha = SHA1.Create();
    }

    public void Initialize()
    {
        mSha.Initialize();
    }

    public void Update(byte[] Data)
    {
        mSha.TransformBlock(Data, 0, Data.Length, Data, 0);
    }

    public void Update(string s)
    {
        Update(Encoding.Default.GetBytes(s));
    }

    public void Update(Int32 data)
    {
        Update(BitConverter.GetBytes(data));
    }

    public void Update(UInt32 data)
    {
        Update(BitConverter.GetBytes(data));
    }

    public byte[] Final()
    {
        mSha.TransformFinalBlock(ZeroArray, 0, 0);
        return mSha.Hash;
    }

    public byte[] Final(byte[] Data)
    {
        mSha.TransformFinalBlock(Data, 0, Data.Length);
        return mSha.Hash;
    }
}
