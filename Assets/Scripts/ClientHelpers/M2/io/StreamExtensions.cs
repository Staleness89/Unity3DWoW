using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

/// <summary>
///     Extensions to BinaryReader and BinaryWriter to hide the generic type identification done during IO.
/// </summary>
public static class StreamExtensions
    {
        public static readonly Dictionary<Type, Func<BinaryReader, object>> ReadFunctions =
            new Dictionary<Type, Func<BinaryReader, object>>();

        public static readonly Dictionary<Type, Action<BinaryWriter, object>> WriteFunctions =
            new Dictionary<Type, Action<BinaryWriter, object>>();

        static StreamExtensions()
        {
            ReadFunctions[typeof (bool)] = s => s.ReadBoolean();
            ReadFunctions[typeof (byte)] = s => s.ReadByte();
            ReadFunctions[typeof (short)] = s => s.ReadInt16();
            ReadFunctions[typeof (ushort)] = s => s.ReadUInt16();
            ReadFunctions[typeof (int)] = s => s.ReadInt32();
            ReadFunctions[typeof (uint)] = s => s.ReadUInt32();
            ReadFunctions[typeof (float)] = s => s.ReadSingle();
            ReadFunctions[typeof (C2Vector)] = s => s.ReadC2Vector();
            ReadFunctions[typeof (C33Matrix)] = s => s.ReadC33Matrix();
            ReadFunctions[typeof (C3Vector)] = s => s.ReadC3Vector();
            ReadFunctions[typeof (C44Matrix)] = s => s.ReadC44Matrix();
            ReadFunctions[typeof (C4Plane)] = s => s.ReadC4Plane();
            ReadFunctions[typeof (C4Quaternion)] = s => s.ReadC4Quaternion();
            ReadFunctions[typeof (C4Vector)] = s => s.ReadC4Vector();
            ReadFunctions[typeof (CAaBox)] = s => s.ReadCAaBox();
            ReadFunctions[typeof (CAaSphere)] = s => s.ReadCAaSphere();
            ReadFunctions[typeof (CArgb)] = s => s.ReadCArgb();
            ReadFunctions[typeof (M2CompQuat)] = s => s.ReadCompQuat();
            ReadFunctions[typeof (CRange)] = s => s.ReadCRange();
            ReadFunctions[typeof (FixedPoint_0_15)] = s => s.ReadFixedPoint_0_15();
            ReadFunctions[typeof (FixedPoint_6_9)] = s => s.ReadFixedPoint_6_9();
            ReadFunctions[typeof (FixedPoint_2_5)] = s => s.ReadFixedPoint_2_5();
            ReadFunctions[typeof (VertexProperty)] = s => s.ReadVertexProperty();

            WriteFunctions[typeof (bool)] = (s, t) => s.Write((bool) t);
            WriteFunctions[typeof (byte)] = (s, t) => s.Write((byte) t);
            WriteFunctions[typeof (short)] = (s, t) => s.Write((short) t);
            WriteFunctions[typeof (ushort)] = (s, t) => s.Write((ushort) t);
            WriteFunctions[typeof (int)] = (s, t) => s.Write((int) t);
            WriteFunctions[typeof (uint)] = (s, t) => s.Write((uint) t);
            WriteFunctions[typeof (float)] = (s, t) => s.Write((float) t);
            WriteFunctions[typeof (C2Vector)] = (s, t) => s.Write((C2Vector) t);
            WriteFunctions[typeof (C33Matrix)] = (s, t) => s.Write((C33Matrix) t);
            WriteFunctions[typeof (C3Vector)] = (s, t) => s.Write((C3Vector) t);
            WriteFunctions[typeof (C44Matrix)] = (s, t) => s.Write((C44Matrix) t);
            WriteFunctions[typeof (C4Plane)] = (s, t) => s.Write((C4Plane) t);
            WriteFunctions[typeof (C4Quaternion)] = (s, t) => s.Write((C4Quaternion) t);
            WriteFunctions[typeof (C4Vector)] = (s, t) => s.Write((C4Vector) t);
            WriteFunctions[typeof (CAaBox)] = (s, t) => s.Write((CAaBox) t);
            WriteFunctions[typeof (CAaSphere)] = (s, t) => s.Write((CAaSphere) t);
            WriteFunctions[typeof (CArgb)] = (s, t) => s.Write((CArgb) t);
            WriteFunctions[typeof (M2CompQuat)] = (s, t) => s.Write((M2CompQuat) t);
            WriteFunctions[typeof (CRange)] = (s, t) => s.Write((CRange) t);
            WriteFunctions[typeof (FixedPoint_0_15)] = (s, t) => s.Write((FixedPoint_0_15) t);
            WriteFunctions[typeof (FixedPoint_6_9)] = (s, t) => s.Write((FixedPoint_6_9) t);
            WriteFunctions[typeof (FixedPoint_2_5)] = (s, t) => s.Write((FixedPoint_2_5) t);
            WriteFunctions[typeof (VertexProperty)] = (s, t) => s.Write((VertexProperty) t);
        }

        public static T ReadGeneric<T>(this BinaryReader stream, M2.Format version)
            where T : new()
        {
            if (!typeof (IMarshalable).IsAssignableFrom(typeof (T))) return (T) ReadFunctions[typeof (T)](stream);
            var item = new T();
            ((IMarshalable) item).Load(stream, version);
            return item;
        }


        public static void WriteGeneric<T>(this BinaryWriter stream, M2.Format version, T item) where T : new()
        {
            if (typeof (IMarshalable).IsAssignableFrom(typeof (T)))
                ((IMarshalable) item).Save(stream, version);
            else WriteFunctions[typeof (T)](stream, item);
        }

        //READING OF STRUCTS
        public static C2Vector ReadC2Vector(this BinaryReader stream)
            => new C2Vector(stream.ReadSingle(), stream.ReadSingle());

        public static C33Matrix ReadC33Matrix(this BinaryReader stream)
            => new C33Matrix(stream.ReadC3Vector(), stream.ReadC3Vector(), stream.ReadC3Vector());

        public static C3Vector ReadC3Vector(this BinaryReader stream)
            => new C3Vector(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static C44Matrix ReadC44Matrix(this BinaryReader stream)
            => new C44Matrix(stream.ReadC4Vector(), stream.ReadC4Vector(), stream.ReadC4Vector(),
                stream.ReadC4Vector());

        public static C4Plane ReadC4Plane(this BinaryReader stream)
            => new C4Plane(stream.ReadC3Vector(), stream.ReadSingle());

        public static C4Quaternion ReadC4Quaternion(this BinaryReader stream)
            => new C4Quaternion(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static C4Vector ReadC4Vector(this BinaryReader stream)
            => new C4Vector(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static CAaBox ReadCAaBox(this BinaryReader stream)
            => new CAaBox(stream.ReadC3Vector(), stream.ReadC3Vector());

        public static CAaSphere ReadCAaSphere(this BinaryReader stream)
            => new CAaSphere(stream.ReadC3Vector(), stream.ReadSingle());

        public static CArgb ReadCArgb(this BinaryReader stream)
            => new CArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte());

        public static M2CompQuat ReadCompQuat(this BinaryReader stream)
            => new M2CompQuat(stream.ReadInt16(), stream.ReadInt16(), stream.ReadInt16(), stream.ReadInt16());

        public static CRange ReadCRange(this BinaryReader stream)
            => new CRange(stream.ReadSingle(), stream.ReadSingle());

        public static FixedPoint_0_15 ReadFixedPoint_0_15(this BinaryReader stream)
            => new FixedPoint_0_15(stream.ReadInt16());

        public static FixedPoint_6_9 ReadFixedPoint_6_9(this BinaryReader stream)
            => new FixedPoint_6_9(stream.ReadInt16());

        public static FixedPoint_2_5 ReadFixedPoint_2_5(this BinaryReader stream)
            => new FixedPoint_2_5(stream.ReadByte());

        public static VertexProperty ReadVertexProperty(this BinaryReader stream)
            => new VertexProperty(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte());

        //WRITING OF STRUCTS
        public static void Write(this BinaryWriter stream, C2Vector item)
        {
            stream.Write(item.X);
            stream.Write(item.Y);
        }

        public static void Write(this BinaryWriter stream, C33Matrix item)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < item.Columns.Length; i++)
            {
                stream.Write(item.Columns[i]);
            }
        }

        public static void Write(this BinaryWriter stream, C3Vector item)
        {
            stream.Write(item.X);
            stream.Write(item.Y);
            stream.Write(item.Z);
        }

        public static void Write(this BinaryWriter stream, C44Matrix item)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < item.Columns.Length; i++)
            {
                stream.Write(item.Columns[i]);
            }
        }

        public static void Write(this BinaryWriter stream, C4Plane item)
        {
            stream.Write(item.Normal);
            stream.Write(item.Distance);
        }

        public static void Write(this BinaryWriter stream, C4Quaternion item)
        {
            stream.Write(item.X);
            stream.Write(item.Y);
            stream.Write(item.Z);
            stream.Write(item.W);
        }

        public static void Write(this BinaryWriter stream, C4Vector item)
        {
            stream.Write(item.W);
            stream.Write(item.X);
            stream.Write(item.Y);
            stream.Write(item.Z);
        }

        public static void Write(this BinaryWriter stream, CAaBox item)
        {
            stream.Write(item.Min);
            stream.Write(item.Max);
        }

        public static void Write(this BinaryWriter stream, CAaSphere item)
        {
            stream.Write(item.Position);
            stream.Write(item.Radius);
        }

        public static void Write(this BinaryWriter stream, CArgb item)
        {
            stream.Write(item.R);
            stream.Write(item.G);
            stream.Write(item.B);
            stream.Write(item.A);
        }

        public static void Write(this BinaryWriter stream, M2CompQuat item)
        {
            stream.Write(item.X);
            stream.Write(item.Y);
            stream.Write(item.Z);
            stream.Write(item.W);
        }

        public static void Write(this BinaryWriter stream, CRange item)
        {
            stream.Write(item.Min);
            stream.Write(item.Max);
        }

        public static void Write(this BinaryWriter stream, FixedPoint_0_15 item)
            => stream.Write(item.ToShort());
        public static void Write(this BinaryWriter stream, FixedPoint_6_9 item)
            => stream.Write(item.ToShort());
        public static void Write(this BinaryWriter stream, FixedPoint_2_5 item)
            => stream.Write(item.ToByte());

        public static void Write(this BinaryWriter stream, VertexProperty item)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < item.Properties.Length; i++)
            {
                stream.Write(item.Properties[i]);
            }
        }
    }
internal static unsafe class Extensions
{
    public static IEnumerable<byte> HexToBytes(this string str)
    {
        for (var i = 0; i < str.Length; i += 2)
        {
            var cl = str[i + 1];
            var ch = str[i];
            var cnl = cl - '0';
            var cnh = ch - '0';
            if (cnl < 0 || cnl > 9)
            {
                cnl = (cl - 'a') + 10;
                if (cnl < 10 || cnl > 15)
                    cnl = (cl - 'A') + 10;
            }
            if (cnh < 0 || cnh > 9)
            {
                cnh = (ch - 'a') + 10;
                if (cnh < 10 || cnh > 15)
                    cnh = (ch - 'A') + 10;
            }
            yield return (byte)((cnh << 4) | cnl);
        }
    }

    public static void ReadToPointer(this BinaryReader br, IntPtr dest, int size)
    {
        var bytes = br.ReadBytes(size);
        fixed (byte* b = bytes)
            UnsafeNativeMethods.CopyMemory((byte*)dest.ToPointer(), b, size);
    }

    public static uint ReadUInt32Be(this BinaryReader br)
    {
        var be = br.ReadUInt32();
        return (be >> 24) | (((be >> 16) & 0xFF) << 8) | (((be >> 8) & 0xFF) << 16) | ((be & 0xFF) << 24);
    }

    public static string ReadWString(this BinaryReader br, long pos = -1, bool returnToOrig = true)
    {
        if (pos == -1)
        {
            StringBuilder sb = new StringBuilder();
            ushort cur = br.ReadUInt16();
            while (cur != 0)
            {
                sb.Append((char)cur);
                cur = br.ReadUInt16();
            }
            return sb.ToString();
        }
        var orig = br.BaseStream.Position;
        br.BaseStream.Seek(pos, SeekOrigin.Begin);

        var s = ReadWString(br);

        if (returnToOrig)
        {
            br.BaseStream.Seek(orig, SeekOrigin.Begin);
        }

        return s;
    }

    public static string ReadCString(this BinaryReader reader)
    {
        byte num;
        List<byte> temp = new List<byte>();
        while ((num = reader.ReadByte()) != 0 && reader.BaseStream.Position != reader.BaseStream.Length)
            temp.Add(num);

        return Encoding.UTF8.GetString(temp.ToArray());
    }

    public static T Read<T>(this BinaryReader br) where T : struct
    {
        if (SizeCache<T>.TypeRequiresMarshal)
        {
            throw new ArgumentException(
                "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");
        }

        // OPTIMIZATION!
        var ret = new T();
        fixed (byte* b = br.ReadBytes(SizeCache<T>.Size))
        {
            var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret);
            UnsafeNativeMethods.CopyMemory(tPtr, b, SizeCache<T>.Size);
        }
        return ret;
    }

    public static T[] Read<T>(this BinaryReader br, long addr, long count) where T : struct
    {
        br.BaseStream.Seek(addr, SeekOrigin.Begin);
        return br.Read<T>(count);
    }

    public static T[] Read<T>(this BinaryReader br, long count) where T : struct
    {
        return br.Read<T>((int)count);
    }

    public static T[] Read<T>(this BinaryReader br, int count) where T : struct
    {
        if (SizeCache<T>.TypeRequiresMarshal)
        {
            throw new ArgumentException(
                "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");
        }

        if (count == 0)
            return new T[0];

        var ret = new T[count];
        fixed (byte* pB = br.ReadBytes(SizeCache<T>.Size * count))
        {
            var genericPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret[0]);
            UnsafeNativeMethods.CopyMemory(genericPtr, pB, SizeCache<T>.Size * count);
        }
        return ret;
    }

    public static void Write<T>(this BinaryWriter bw, T value) where T : struct
    {
        if (SizeCache<T>.TypeRequiresMarshal)
        {
            throw new ArgumentException(
                "Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");
        }

        // fastest way to copy?
        var buf = new byte[SizeCache<T>.Size];

        var valData = (byte*)SizeCache<T>.GetUnsafePtr(ref value);
        fixed (byte* pB = buf)
            UnsafeNativeMethods.CopyMemory(pB, valData, SizeCache<T>.Size);

        bw.Write(buf);
    }

    public static T[] ReadArray<T>(this BinaryReader br, int count) where T : struct
    {
        if (count == 0)
            return new T[0];

        if (SizeCache<T>.TypeRequiresMarshal)
            throw new ArgumentException(
                "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");

        // NOTE: this may be safer to just call Read<T> each iteration to avoid possibilities of moved memory, etc.
        // For now, we'll see if this works.
        var ret = new T[count];
        fixed (byte* pB = br.ReadBytes(SizeCache<T>.Size * count))
        {
            var genericPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret[0]);
            UnsafeNativeMethods.CopyMemory(genericPtr, pB, SizeCache<T>.Size * count);
        }
        return ret;
    }

    public static void ReadToArray<T>(this BinaryReader br, T[] data) where T : struct
    {
        if (SizeCache<T>.TypeRequiresMarshal)
            throw new ArgumentException(
                "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");

        // NOTE: this may be safer to just call Read<T> each iteration to avoid possibilities of moved memory, etc.
        // For now, we'll see if this works.
        fixed (byte* pB = br.ReadBytes(SizeCache<T>.Size * data.Length))
        {
            for (int i = 0; i < data.Length; i++)
            {
                var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref data[i]);
                UnsafeNativeMethods.CopyMemory(tPtr, &pB[i * SizeCache<T>.Size], SizeCache<T>.Size);
            }
        }
    }

    public static void WriteArray<T>(this BinaryWriter writer, T[] values) where T : struct
    {
        if (values.Length == 0)
            return;

        if (SizeCache<T>.TypeRequiresMarshal)
            throw new ArgumentException(
                "Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");

        var buf = new byte[SizeCache<T>.Size * values.Length];
        var valData = (byte*)SizeCache<T>.GetUnsafePtr(ref values[0]);

        fixed (byte* ptr = buf)
            UnsafeNativeMethods.CopyMemory(ptr, valData, buf.Length);

        writer.Write(buf, 0, buf.Length);
    }
}
internal static unsafe class UnsafeNativeMethods
{
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    [SuppressUnmanagedCodeSecurity]
    internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

    [DllImport("Kernel32.dll", SetLastError = false)]
    [SuppressUnmanagedCodeSecurity]
    internal static extern IntPtr CopyMemory(byte* dest, byte* src, int count);

    [DllImport("User32.dll", SetLastError = false)]
    [SuppressUnmanagedCodeSecurity]
    internal static extern int ToUnicode(int wVirtKey, int scanCode, byte[] keyState, StringBuilder outBuffer,
        int numBUffer, int flags);

    [DllImport("User32.dll", SetLastError = false)]
    [SuppressUnmanagedCodeSecurity]
    internal static extern void GetKeyboardState(byte[] keyboard);
}
public static class SizeCache<T> where T : struct
{
    /// <summary> The size of the Type </summary>
    public static readonly int Size;

    public static readonly Type Type;

    /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
    public static readonly bool TypeRequiresMarshal;

    internal static readonly GetUnsafePtrDelegate GetUnsafePtr;

    static SizeCache()
    {
        Type = typeof(T);
        // Bools = 1 char.
        if (typeof(T) == typeof(bool))
        {
            Size = 1;
        }
        else if (typeof(T).IsEnum)
        {
            Type = typeof(T).GetEnumUnderlyingType();
            Size = GetSizeOf(Type);
        }
        else
        {
            Size = GetSizeOf(Type);
        }

        TypeRequiresMarshal = GetRequiresMarshal(Type);

        // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
        var method = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")),
            typeof(void*),
            new[] { typeof(T).MakeByRefType() },
            typeof(SizeCache<>).Module);

        ILGenerator generator = method.GetILGenerator();

        // ldarg 0
        generator.Emit(OpCodes.Ldarg_0);
        // (IntPtr)arg0
        generator.Emit(OpCodes.Conv_U);
        // ret arg0
        generator.Emit(OpCodes.Ret);
        GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
    }

    private static int GetSizeOf(Type t)
    {
        try
        {
            // Try letting the marshaler handle getting the size.
            // It can *sometimes* do it correctly
            // If it can't, fall back to our own methods.
            var o = Activator.CreateInstance(t);
            return Marshal.SizeOf(o);
        }
        catch (Exception)
        {
            int totalSize = 0;
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof(FixedBufferAttribute), false);

                if (attr.Length > 0)
                {
                    var fba = (FixedBufferAttribute)attr[0];
                    totalSize += GetSizeOf(fba.ElementType) * fba.Length;
                    continue;
                }

                totalSize += GetSizeOf(field.FieldType);
            }
            return totalSize;
        }
    }

    private static bool GetRequiresMarshal(Type t)
    {
        var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var requires = field.GetCustomAttributes(typeof(MarshalAsAttribute), true).Length != 0;

            if (requires)
                return true;

            if (t == typeof(IntPtr))
                continue;

            if (Type.GetTypeCode(t) == TypeCode.Object)
                requires |= GetRequiresMarshal(field.FieldType);

            return requires;
        }
        return false;
    }

    #region Nested type: GetUnsafePtrDelegate

    internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);

    #endregion
}
