public class BattleResults
{
    public static readonly BattleResults Current = new BattleResults();

    public string winnerName = "PLAYER 1";
    public string songName = "";
    public int perfect;
    public int great;
    public int good;
    public int bad;
    public int miss;
    public int successfulAttacks;
    public int successfulDefends;

    public void Clear()
    {
        winnerName = "PLAYER 1";
        songName = "";
        perfect = 0;
        great = 0;
        good = 0;
        bad = 0;
        miss = 0;
        successfulAttacks = 0;
        successfulDefends = 0;
    }
}
