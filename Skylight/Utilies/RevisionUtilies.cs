using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class RevisionUtilies
    {
        public static Revision StringToRevision(string revision)
        {
            switch(revision)
            {
                case "R26-20080915-0408-7984-61ccb5f8b8797a3aba62c1fa2ca80169":
                    return Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169;
                case "RELEASE63-35255-34886-201108111108":
                    return Revision.RELEASE63_35255_34886_201108111108;
                case "RELEASE63-201211141113-913728051":
                    return Revision.RELEASE63_201211141113_913728051;
                case "PRODUCTION-201601012205-226667486":
                    return Revision.PRODUCTION_201601012205_226667486;
                case "PRODUCTION-201611291003-338511768":
                    return Revision.PRODUCTION_201611291003_338511768;
                default:
                    return Revision.None;
            }
        }

        public static Crypto StringToCrypto(string crypto)
        {
            switch(crypto)
            {
                case "OLD":
                    return Crypto.OLD;
                case "NEW":
                    return Crypto.NEW;
                case "BOTH":
                    return Crypto.BOTH;
                default:
                    return Crypto.NONE;
            }
        }
    }
}
