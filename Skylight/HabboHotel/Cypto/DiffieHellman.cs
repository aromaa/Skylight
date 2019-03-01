using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Cypto
{
    public class DiffieHellman
    {
        #region Static vars
        public readonly int BITLENGTH = 30;
        #endregion

        #region Variables
        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }

        public BigInteger PrivateKey { get; private set; }
        public BigInteger PublicKey { get; private set; }

        public BigInteger PublicClientKey { get; private set; }

        public BigInteger SharedKey { get; private set; }
        #endregion

        #region Constructor
        public DiffieHellman()
        {
            this.InitDH();
        }

        public DiffieHellman(int b)
        {
            this.BITLENGTH = b;

            this.InitDH();
        }

        private void InitDH()
        {
            this.PublicKey = 0;
            while (this.PublicKey == 0)
            {
                this.Prime = BigInteger.genPseudoPrime(this.BITLENGTH, 10, new Random());
                this.Generator = BigInteger.genPseudoPrime(this.BITLENGTH, 10, new Random());

                this.PrivateKey = new BigInteger(GenerateRandomHexString(this.BITLENGTH), 16);

                if (this.Generator > this.Prime)
                {
                    BigInteger temp = this.Prime;
                    this.Prime = this.Generator;
                    this.Generator = temp;
                }

                this.PublicKey = this.Generator.modPow(this.PrivateKey, this.Prime);
            }
        }

        public DiffieHellman(BigInteger prime, BigInteger generator)
        {
            this.Prime = prime;
            this.Generator = generator;

            this.PrivateKey = new BigInteger(GenerateRandomHexString(this.BITLENGTH), 16);

            if (this.Generator > this.Prime)
            {
                BigInteger temp = this.Prime;
                this.Prime = this.Generator;
                this.Generator = temp;
            }

            this.PublicKey = this.Generator.modPow(this.PrivateKey, this.Prime);

        }
        #endregion

        #region SharedKey
        public void GenerateSharedKey(string ckey)
        {
            this.PublicClientKey = new BigInteger(ckey, 10);

            this.SharedKey = this.PublicClientKey.modPow(this.PrivateKey, this.Prime);
        }
        #endregion

        #region RandomHexString
        public static string GenerateRandomHexString(int len)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] Bytes = new byte[len / 2];
            rng.GetBytes(Bytes);

            BigInteger Result = new BigInteger(Bytes);

            if (Result < 0)
            {
                Result *= -1;
            }

            return Result.ToString(16);
        }

        #endregion
    }
}
