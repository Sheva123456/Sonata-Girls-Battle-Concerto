using UnityEngine;
using UnityEngine.UI;

// Legacy UI text has no font until one is assigned. This fills any blank labels when the scene opens.
[ExecuteAlways]
public class RhythmPlaceholderFont : MonoBehaviour
{
    void OnEnable()
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            return;

        foreach (var text in GetComponentsInChildren<Text>(true))
        {
            if (text.font == null)
                text.font = font;
        }
    }
}
