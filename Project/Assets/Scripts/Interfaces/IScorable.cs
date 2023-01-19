namespace Players
{
    public interface IScorable
    {
        #region Properties
        int ScoreValueIncrement { get; set; }
        int Score { get; }
        #endregion

        #region Methods
        int IncrementScore();
        #endregion
    }
}