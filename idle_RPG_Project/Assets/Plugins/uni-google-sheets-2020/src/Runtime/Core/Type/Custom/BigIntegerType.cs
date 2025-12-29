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
            if (string.IsNullOrWhiteSpace(value))
                return BigInteger.Zero;

            if (BigInteger.TryParse(value, out BigInteger result))
            {
                return result;
            }
            throw new UGSValueParseException($"Parse Failed => '{value}' To BigInteger");
        }
        public string Write(object value)
        {
            return value.ToString();
        }
    }

}

