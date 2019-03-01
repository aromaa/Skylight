using System;
namespace SkylightEmulator.Utilies
{
	public static class WireEncoding
	{
		public const byte NEGATIVE = 72;
		public const byte POSITIVE = 73;
		public const int MAX_INTEGER_BYTE_AMOUNT = 6;
		public static byte[] EncodeInt32(int i)
		{
			byte[] wf = new byte[6];
			int pos = 0;
			int numBytes = 1;
            int startPos = pos;
			int num4 = (i >= 0) ? 0 : 4;
			i = Math.Abs(i);
			wf[pos++] = (byte)(64 + (i & 3));
			for (i >>= 2; i != 0; i >>= 6)
			{
				numBytes++;
				wf[pos++] = (byte)(64 + (i & 63));
			}
			wf[startPos] = (byte)((int)wf[startPos] | numBytes << 3 | num4);
			byte[] bzData = new byte[numBytes];
			for (int x = 0; x < numBytes; x++)
			{
				bzData[x] = wf[x];
			}
			return bzData;
		}
		public static int DecodeInt32(byte[] bzData, out int totalBytes)
		{
			bool flag = (bzData[0] & 4) == 4;
			totalBytes = (bzData[0] >> 3 & 7);
			int v = (int)(bzData[0] & 3);
			int pos = 0 + 1;
			int shiftAmount = 2;
			for (int i = 1; i < totalBytes; i++)
			{
				v |= (int)(bzData[pos] & 63) << shiftAmount;
				shiftAmount = 2 + 6 * i;
				pos++;
			}
			if (flag)
			{
				v *= -1;
			}
			return v;
		}
	}
}
