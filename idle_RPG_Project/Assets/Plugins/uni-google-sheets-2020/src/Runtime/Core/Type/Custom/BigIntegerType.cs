using GoogleSheet.Type;
using System.Numerics;

namespace GoogleSheet
{
    [Type(typeof(BigInteger), new string[] { "Big", "biginteger", "big", "BigInteger" })]
    public class BigIntegerType : IType
    {
        public object DefaultValue => BigInteger.Zero;

        public object Read(string value)
        {
            return BigInteger.Parse(value);
        }
        public string Write(object value)
        {
            return "";
        }
    }

}

