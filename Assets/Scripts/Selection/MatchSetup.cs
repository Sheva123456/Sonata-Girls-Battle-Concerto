using UnityEngine;

public class MatchSetup
{
    public static readonly MatchSetup Current = new MatchSetup();

    public string displayName = "Placeholder 1";
    public Sprite leftIdle;
    public Sprite rightIdle;
    public bool hasCharacter;

    public void SetCharacter(string name, Sprite left, Sprite right)
    {
        displayName = string.IsNullOrEmpty(name) ? "Placeholder 1" : name;
        leftIdle = left;
        rightIdle = right;
        hasCharacter = left != null || right != null;
    }
}
