using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Cypto
{
    public class RC4
    {


        /**
         * Método globalizado para evitar Outs of Memory
         * By Itachy & Fakundo.
         * 2012
         *        
        private int i = 0;
        private int j = 0;
        private int[] Table;
        public RC4()
        {
            this.Table = new int[256];
        }

        public RC4(byte[] key)
        {
            this.Table = new int[256];

            this.Init(key);
        }
         * **/

        public static int concurrent = 3;
        public static Queue<int> current = new Queue<int>();
        public static void Init(byte[] key, ref int i, ref int j, ref int[] Table)
        {
            int k = key.Length;
            i = 0;
            while (i < 256)
            {
                Table[i] = i;
                i++;
            }

            i = 0;
            j = 0;
            while (i < 0x0100)
            {
                j = (((j + Table[i]) + key[(i % k)]) % 256);
                Swap(i, j, ref Table);
                i++;
            }

            i = 0;
            j = 0;
        }

        public static void Swap(int a, int b, ref int[] Table)
        {
            int k = Table[a];
            Table[a] = Table[b];
            Table[b] = k;
        }

        public static byte[] Decipher(ref int[] Table, ref int i, ref int j, byte[] bytes)
        {
            do
            {
                //System.Threading.Thread.Sleep(2);
            } while (current.Count >= concurrent);
            try
            {
                current.Enqueue(1);
            }
            catch
            {
            }
            byte[] result = new byte[bytes.Length];
            try
            {
                int k = 0;
                int pos = 0;

                for (int a = 0; a < bytes.Length; a++)
                {
                    i = ((i + 1) % 256);
                    j = ((j + Table[i]) % 256);
                    Swap(i, j, ref Table);
                    k = ((Table[i] + Table[j]) % 256);
                    result[pos++] = (byte)(bytes[a] ^ Table[k]);
                }
            }
            catch
            {
            }
            try
            {
                current.Dequeue();
            }
            catch
            {
            }

            return result;
        }
    }
}
