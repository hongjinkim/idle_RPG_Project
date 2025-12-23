using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Numerics;
using System.Text;

public enum ETextTransitionType
{
    Instant,    // 즉시 변경
    Rolling,    // 숫자 롤링 효과
    Typing      // 타이핑 효과 (문자열 전용)
}

public static class TMPExtension
{
    // ★ 전역 재사용 StringBuilder (TMP 가비지 생성 최소화용)
    private static StringBuilder _sharedSb = new StringBuilder(64);

    /// <summary>
    /// BigInteger 값을 부드럽게 갱신하는 확장 메서드
    /// </summary>
    /// <param name="tmp">대상 TextMeshProUGUI</param>
    /// <param name="from">시작 값 (보통 현재 값)</param>
    /// <param name="to">목표 값</param>
    /// <param name="transType">전환 효과 (Instant / Rolling)</param>
    /// <param name="formatType">BigIntegerExtension에 정의된 포맷 타입</param>
    /// <param name="duration">연출 시간</param>
    public static void UpdateText(this TextMeshProUGUI tmp, BigInteger from, BigInteger to, ETextTransitionType transType, ENumberFormatType formatType, float duration = 0.5f)
    {
        // 기존 트윈 제거 (중복 실행 방지)
        tmp.DOKill();

        switch (transType)
        {
            case ETextTransitionType.Instant:
                SetTextInternal(tmp, to, formatType);
                break;

            case ETextTransitionType.Rolling:
                // 값이 같으면 굳이 연출하지 않음
                if (from == to)
                {
                    SetTextInternal(tmp, to, formatType);
                    return;
                }

                // 0~1까지 진행되는 가상의 트윈 생성
                DOVirtual.Float(0, 1, duration, (t) =>
                {
                    // Linear Interpolation (선형 보간) 계산
                    // BigInteger는 직접 빼기가 가능하므로 차이를 구한 뒤 비율(t)을 곱함
                    // double로 변환하여 계산 후 다시 BigInteger로 캐스팅 (연출용이라 정밀도 손실 허용)
                    BigInteger currentDifference = (BigInteger)((double)(to - from) * t);
                    BigInteger currentValue = from + currentDifference;

                    SetTextInternal(tmp, currentValue, formatType);
                })
                .SetTarget(tmp) // UI가 파괴되면 트윈도 자동 종료
                .SetEase(Ease.OutQuad) // 끝부분에서 부드럽게 감속
                .OnComplete(() =>
                {
                    // 연출 끝난 후 목표값으로 정확하게 확정
                    SetTextInternal(tmp, to, formatType);
                });
                break;
        }
    }

    // ★ 내부 최적화 로직 (StringBuilder + 사용자 정의 포맷터 연결)
    private static void SetTextInternal(TextMeshProUGUI tmp, BigInteger value, ENumberFormatType formatType)
    {
        // 1. 전역 StringBuilder 비우기
        _sharedSb.Clear();

        // 2. 사용자가 만든 BigIntegerExtension의 포맷터를 사용하여 string을 받아옴
        // (참고: ToFormattedString 내부에서 string을 리턴하므로 여기서 일부 가비지가 발생하지만,
        //  TMP에 SetText(StringBuilder)를 사용함으로써 TMP 내부의 2차 가비지 생성을 막음)
        string formattedString = value.ToFormattedString(formatType);

        // 3. StringBuilder에 담기
        _sharedSb.Append(formattedString);

        // 4. TMP에 StringBuilder 전달 (최적화 포인트)
        tmp.SetText(_sharedSb);
    }
}