using UnityEngine;
using UnityEngine.UI;

public class CharacterPose : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] Sprite idle;
    [SerializeField] Sprite attack;
    [SerializeField] Sprite defense;
    [SerializeField] float actionSeconds = 0.45f;

    float returnToIdleAt;

    void Awake()
    {
        ShowIdle();
    }

    public void ShowAttack()
    {
        Apply(attack);
        returnToIdleAt = Time.unscaledTime + actionSeconds;
    }

    public void ShowDefense()
    {
        Apply(defense);
        returnToIdleAt = Time.unscaledTime + actionSeconds;
    }

    public void ShowIdle()
    {
        returnToIdleAt = 0f;
        Apply(idle);
    }

    void Update()
    {
        if (returnToIdleAt > 0f && Time.unscaledTime >= returnToIdleAt)
            ShowIdle();
    }

    void Apply(Sprite sprite)
    {
        if (portrait != null && sprite != null)
            portrait.sprite = sprite;
    }
}
