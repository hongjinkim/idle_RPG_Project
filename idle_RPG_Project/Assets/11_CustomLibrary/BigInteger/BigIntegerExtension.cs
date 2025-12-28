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

    public static BigInteger ExpGrowth(this BigInteger start, BigInteger constant, double exponent, int s)
    {
        // 1. 지수 함수 계산 (결과는 double)
        // 식: e^(exponent * s) - 1.0
        double expValue = Math.Exp(exponent * s);
        double multiplier = expValue - 1.0;

        // double 범위 초과 체크
        if (double.IsInfinity(multiplier))
        {

            throw new OverflowException("지수 증가량이 너무 커서 계산할 수 없습니다 (double 범위 초과).");
        }

        // 2. BigInteger(Constant) * double(multiplier) 계산
        // BigInteger는 소수점 곱셈이 안되므로 정밀도를 위해 값을 키워서 계산함.
        // 예: 1.5를 곱해야 한다면, 15000을 곱하고 나중에 10000으로 나눔.

        long precisionScale = 10000; // 소수점 4자리까지 정밀도 보장

        // multiplier를 정수로 변환 (예: 1.5555 -> 15555)
        BigInteger scaledMultiplier = (BigInteger)(multiplier * precisionScale);

        // (Constant * ScaledMultiplier) / Scale
        BigInteger calculatedVal = (constant * scaledMultiplier) / precisionScale;

        // 3. 최종 합산
        return start + calculatedVal;
    }
}
