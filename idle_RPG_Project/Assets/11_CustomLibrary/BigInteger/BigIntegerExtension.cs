using System;
using System.Numerics;


// 표기 형식
public enum ENumberFormatType
{
    Standard,
    Alphabet,
    Scientific,
    Korean
}

public static class BigIntegerExtension
{
    private static readonly string[] AlphabetUnits = new string[]
    {
        "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap"
        // 추가 단위는 필요에 따라 여기에 추가
    };
    private static readonly string[] KoreanUnits = new string[]
    {
        "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정","재", "극"
        // 추가 단위는 필요에 따라 여기에 추가
    };

    public static string ToFormattedString(this BigInteger value, ENumberFormatType formatType, int decimalPlaces = 2)
    {
        if (value < 1000)
            return value.ToString();
        switch (formatType)
        {
            case ENumberFormatType.Standard:
                return StandardFormat(value, decimalPlaces);
            case ENumberFormatType.Alphabet:
                return AlphabetFormat(value, decimalPlaces);
            case ENumberFormatType.Scientific:
                return ScientificFormat(value, decimalPlaces);
            case ENumberFormatType.Korean:
                return KoreanFormat(value, decimalPlaces);
            default:
                return value.ToString();
        }
    }

    private static string StandardFormat(BigInteger value, int decimalPlaces)
    {
        string formatString = "N" + decimalPlaces;
        return value.ToString(formatString);
    }
    private static string AlphabetFormat(BigInteger value, int decimalPlaces)
    {
        int unitIndex = 0;
        BigInteger divisor = 1;
        while (value / divisor >= 1000 && unitIndex < AlphabetUnits.Length - 1)
        {
            divisor *= 1000;
            unitIndex++;
        }
        decimal shortValue = (decimal)value / (decimal)divisor;
        return shortValue.ToString("F" + decimalPlaces) + AlphabetUnits[unitIndex];
    }
    private static string ScientificFormat(BigInteger value, int decimalPlaces)
    {
        double exponent = BigInteger.Log10(value);
        int expInt = (int)exponent;
        double mantissa = (double)value / Math.Pow(10, expInt);
        return mantissa.ToString("F" + decimalPlaces) + "e" + expInt;
    }
    private static string KoreanFormat(BigInteger value, int decimalPlaces)
    {
        int unitIndex = 0;
        BigInteger divisor = 1;
        while (value / divisor >= 10000 && unitIndex < KoreanUnits.Length - 1)
        {
            divisor *= 10000;
            unitIndex++;
        }
        decimal shortValue = (decimal)value / (decimal)divisor;
        return shortValue.ToString("F" + decimalPlaces) + KoreanUnits[unitIndex];
    }
}
