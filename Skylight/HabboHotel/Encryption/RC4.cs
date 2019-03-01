using SkylightEmulator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Encryption
{
    public class RC4
    {
        int[] key = new int[256];
        int[] table = new int[256];
        int q;
        int j;

        public RC4(int[] tOtherKey)
        {
            // Reset j
            this.j = 0;
            // Call init
            this.init(tOtherKey);
            // Premix the table
            this.PremixTable("NV6VVFPoC7FLDlzDUri3qcOAg9cRoFOmsYR9ffDGy5P8HfF6eekX40SFSVfJ1mDb3lcpYRqdg28sp61eHkPukKbqTu1JsVEKiRavi04YtSzUsLXaYSa5BEGwg5G2OF", 52);
        }

        private void init(int[] tKey)
        {
            // This has fucking changed loads..
            int[] tXorVals = { 109, 87, 120, 70, 82, 74, 110, 71, 74, 53, 84, 57, 83, 105, 48, 79, 77, 86, 118, 69, 66, 66, 109, 56, 108, 97, 105, 104, 88, 107, 78, 56, 71, 109, 72, 54, 102, 117, 118, 55, 108, 100, 90, 104, 76, 121, 71, 82, 82, 75, 67, 99, 71, 122, 122, 105, 80, 89, 66, 97, 74, 111, 109 };
            List<int> tModKey = new List<int>();
            int k;
            int tVal;
            int l = 1;

            for (k = 1; k <= tKey.Count(); k++)
            {
                tVal = (tKey[k - 1] ^ tXorVals[l - 1]);
                tModKey.Add(tVal);
                l = (l + 1);

                if (l > tXorVals.Count())
                {
                    l = 1;
                }
            }

            for (this.q = 0; this.q <= 255; this.q++)
            {
                this.key[this.q] = tModKey[((this.q % (tModKey.Count)))];
                this.table[this.q] = this.q;
            }

            // Reset j
            this.j = 0;

            for (this.q = 0; this.q <= 255; this.q++)
            {
                this.j = (((this.j + this.table[this.q]) + this.key[this.q]) % 256);
                k = this.table[this.q];
                this.table[this.q] = this.table[this.j];
                this.table[this.j] = k;
            }

            // Reset q and j
            this.q = 0;
            this.j = 0;
        }

        private int MoveUp()
        {
            int swap;
            int t_i;
            int t2_i;
            int t_j;
            int t2_j;

            this.q = ((this.q + 1) % 256);
            this.j = ((this.j + this.table[this.q]) % 256);
            swap = this.table[this.q];
            this.table[this.q] = this.table[this.j];
            this.table[this.j] = swap;

            t_i = ((17 * (this.q + 19)) % 256);
            t_j = ((this.j + this.table[t_i]) % 256);
            swap = this.table[t_i];
            this.table[t_i] = this.table[t_j];
            this.table[t_j] = swap;

            if (((this.q == 46) || (this.q == 67)) || (this.q == 192))
            {
                t2_i = ((297 * (t_i + 67)) % 256);
                t2_j = ((t_j + this.table[t2_i]) % 256);
                swap = this.table[t2_i];
                this.table[t2_i] = this.table[t2_j];
                this.table[t2_j] = swap;
            }

            int tmp = this.table[(this.table[this.q] + this.table[this.j]) % 256];
            return tmp;
        }

        //public string Encipher(string theData)
        //{
        //    StringBuilder ciphered = new StringBuilder();

        //    foreach (Char character in theData)
        //    {
        //        ciphered.Append(((int)character ^ moveUp()).ToString("X"));
        //    }

        //    PremixTable("xllVGKnnQcW8aX4WefdKrBWTqiW5EwT", 1);

        //    return ciphered.ToString();
        //}

        public byte[] Decipher(byte[] theData, bool premix = true)
        {
            StringBuilder decipher = new StringBuilder();

            string data = Skylight.GetDefaultEncoding().GetString(theData);
            for (int i = 0; i < theData.Length; i = i + 2)
            {
                String hexChars = data.Substring(i, 2);
                decipher.Append(Convert.ToChar(int.Parse(hexChars, System.Globalization.NumberStyles.HexNumber) ^ MoveUp()));
            }

            if (premix)
            {
                PremixTable("xllVGKnnQcW8aX4WefdKrBWTqiW5EwT", 1);
            }

            return Skylight.GetDefaultEncoding().GetBytes(decipher.ToString());
        }

        public void PremixTable(string datain, int count)
        {
            for (int a = 0; a < count; a++)
            {
                for (int b = 0; b < datain.Length; b++)
                {
                    this.MoveUp();
                }
            }
        }
    }
}
