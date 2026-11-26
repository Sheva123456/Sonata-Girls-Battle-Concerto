using UnityEngine;
using UnityEngine.UI;

public class BattleResultsScreen : MonoBehaviour
{
    [SerializeField] Text winnerLabel;
    [SerializeField] Text songLabel;
    [SerializeField] Text perfectLabel;
    [SerializeField] Text greatLabel;
    [SerializeField] Text goodLabel;
    [SerializeField] Text badLabel;
    [SerializeField] Text missLabel;
    [SerializeField] Text attacksLabel;
    [SerializeField] Text defendsLabel;

    void Start()
    {
        BattleResults results = BattleResults.Current;
        Set(winnerLabel, string.IsNullOrEmpty(results.winnerName) ? "PLAYER 1" : results.winnerName);
        Set(songLabel, string.IsNullOrEmpty(results.songName) ? "SONG" : results.songName.ToUpperInvariant());
        Set(perfectLabel, results.perfect.ToString());
        Set(greatLabel, results.great.ToString());
        Set(goodLabel, results.good.ToString());
        Set(badLabel, results.bad.ToString());
        Set(missLabel, results.miss.ToString());
        Set(attacksLabel, results.successfulAttacks.ToString());
        Set(defendsLabel, results.successfulDefends.ToString());
    }

    static void Set(Text label, string value)
    {
        if (label != null)
            label.text = value;
    }
}
