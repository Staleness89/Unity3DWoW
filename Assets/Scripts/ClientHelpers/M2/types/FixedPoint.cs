using System;
using System.Collections;
using System.Text;

    // ReSharper disable once InconsistentNaming
    public struct FixedPoint_0_15
    {
        public float Value => _bits.ToFloat(0, 15);
        private readonly BitArray _bits;

        public FixedPoint_0_15(short p1)
        {
            _bits = new BitArray(BitConverter.GetBytes(p1));
        }

        public FixedPoint_0_15(BitArray bits)
        {
            _bits = bits;
        }

        public short ToShort() => _bits.ToShort();
    }

    // ReSharper disable once InconsistentNaming
    public struct FixedPoint_6_9
    {
        public float Value => _bits.ToFloat(6, 9);
        private readonly BitArray _bits;

        public FixedPoint_6_9(short p1)
        {
            _bits = new BitArray(BitConverter.GetBytes(p1));
        }

        public FixedPoint_6_9(BitArray bits)
        {
            _bits = bits;
        }

        public short ToShort() => _bits.ToShort();
    }

    // ReSharper disable once InconsistentNaming
    public struct FixedPoint_2_5
    {
        public float Value => _bits.ToFloat(2, 5);
        private readonly BitArray _bits;

        public FixedPoint_2_5(byte p1)
        {
            _bits = new BitArray(BitConverter.GetBytes(p1));
        }

        public FixedPoint_2_5(BitArray bits)
        {
            _bits = bits;
        }

        public byte ToByte() => _bits.ToByte();
    }

    public static class BitArrayExtensions
    {
        public static float ToFloat(this BitArray bits, int integerBits, int decimalBits)
        {
            const int signBits = 1;
            var decimalPart = bits.GetRange(0, decimalBits).ToInt();
            var integerPart = bits.GetRange(decimalBits, decimalBits + integerBits).ToInt();
            var sign = bits[decimalBits + integerBits + signBits - 1];
            return (sign ? -1.0f : 1.0f)*(integerPart + decimalPart/(float) (1 << decimalBits));
        }

        public static short ToShort(this BitArray bits)
        {
            var result = 0;
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    result |= 1 << i;
                }
            }
            result &= short.MaxValue;
            return (short) result;
        }

        public static byte ToByte(this BitArray bits)
        {
            var result = 0;
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    result |= 1 << i;
                }
            }
            result &= byte.MaxValue;
            return (byte) result;
        }

        public static int ToInt(this BitArray bits)
        {
            var result = 0;
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    result |= 1 << i;
                }
            }
            result &= int.MaxValue;
            return result;
        }

        /// <summary>
        ///     Returns a new BitArray composed of bits from this BitArray from fromIndex (inclusive) to toIndex (exclusive).
        /// </summary>
        /// <param name="bits">this</param>
        /// <param name="fromIndex">index of the first bit to include</param>
        /// <param name="toIndex">index after the last bit to include</param>
        /// <returns>a new BitArray from a range of this BitArray</returns>
        public static BitArray GetRange(this BitArray bits, int fromIndex, int toIndex)
        {
            CheckRange(bits, fromIndex, toIndex);
            var len = bits.Count;
            // If no set bits in range return empty bitarray
            if (len <= fromIndex || fromIndex == toIndex)
                return new BitArray(0);
            // An optimization
            if (toIndex > len)
                toIndex = len;
            var result = new BitArray(toIndex - fromIndex);
            var j = 0;
            for (var i = fromIndex; i < toIndex; i++)
            {
                result[j] = bits[i];
                j++;
            }
            return result;
        }

        /// <summary>
        ///     Checks that fromIndex ... toIndex is a valid range of bit indices.
        /// </summary>
        /// <param name="bits">this</param>
        /// <param name="fromIndex">index of the first bit to include</param>
        /// <param name="toIndex">index after the last bit to include</param>
        public static void CheckRange(this BitArray bits, int fromIndex, int toIndex)
        {
            if (fromIndex < 0)
                throw new IndexOutOfRangeException("fromIndex < 0: " + fromIndex);
            if (toIndex < 0)
                throw new IndexOutOfRangeException("toIndex < 0: " + toIndex);
            if (fromIndex > toIndex)
                throw new IndexOutOfRangeException("fromIndex: " + fromIndex +
                                                   " > toIndex: " + toIndex);
        }

        public static string ToBitString(this BitArray bits)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bits.Count; i++)
            {
                var c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
