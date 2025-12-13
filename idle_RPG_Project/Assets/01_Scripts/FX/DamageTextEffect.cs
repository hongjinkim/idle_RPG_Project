using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextEffect : MonoBehaviour
{
    private Animation Anim;
    [SerializeField] private TextMeshPro DamageText;
    [SerializeField] private Material enemyMt;
    [SerializeField] private Material normalMt;
    [SerializeField] private Material criticalMt;

    private Poolable _pool;

    private void OnEnable()
    {
        if (Anim == null)
            this.Anim = this.GetComponent<Animation>();
        if (this._pool == null)
            this._pool = this.GetComponent<Poolable>();
    }

    // 데미지 및 데미지 타입에 따라 텍스트 설정
    public void SetDamage(AttackInfo attackInfo)
    {
        switch (attackInfo.AttackType)
        {
            case EAttackType.Normal:
                DamageText.fontSharedMaterial = normalMt;
                break;
            case EAttackType.Critical:
                DamageText.fontSharedMaterial = criticalMt;
                break;
        }

        if (attackInfo.IsMiss == true)
            DamageText.text = $"Miss";
        else
            DamageText.text = $"{attackInfo.Damage.ToFormattedString(ENumberFormatType.Standard)}";

        if (Anim == null)
            this.Anim = this.GetComponent<Animation>();
        Anim.Play();
    }

    // 애니메이션 마지막 부분에서 호출
    // 플로팅 애니메이션 끝나면 풀에서 제거
    public void AnimationEnd()
    {
        _pool.Release();
    }
}
