using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Cypto
{
    public class HabboCrypto : DiffieHellman
    {
        private static BigInteger n = new BigInteger("90e0d43db75b5b8ffc8a77e31cc9758fa43fe69f14184bef64e61574beb18fac32520566f6483b246ddc3c991cb366bae975a6f6b733fd9570e8e72efc1e511ff6e2bcac49bf9237222d7c2bf306300d4dfc37113bcc84fa4401c9e4f2b4c41ade9654ef00bd592944838fae21a05ea59fecc961766740c82d84f4299dfb33dd", 16);
        private static BigInteger e = new BigInteger(3);
        private static BigInteger d = new BigInteger(0); //Missing secret number ;o

        private RSA RSA;

        public Boolean Initialized { get; private set; }

        public RC4 RC4 { get; private set; }

        public HabboCrypto()
            : base(200)
        {
            this.RSA = new RSA(n, e, d, 0, 0, 0, 0, 0);

            this.RC4 = new RC4();

            this.Initialized = false;
        }

        public HabboCrypto(BigInteger n, BigInteger e, BigInteger d)
            : base(200)
        {
            this.RSA = new RSA(n, e, d, 0, 0, 0, 0, 0);

            this.RC4 = new RC4();

            this.Initialized = false;
        }

        public Boolean InitializeRC4ToSession(GameClient Session, string ctext)
        {
            // return new BigInteger(clientKey, 10).modPow(this.iPrivateKey, this.m_banner.iPrime).toString(16).toUpperCase();
            try
            {
                string publickey = this.RSA.Decrypt(ctext);

                base.GenerateSharedKey(publickey.Replace(((char)0).ToString(), ""));

                RC4.Init(base.SharedKey.getBytes(), ref Session.i, ref Session.j, ref Session.table);
                Session.CryptoInitialized = true;
                this.Initialized = true;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
