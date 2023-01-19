namespace Players
{
    public interface IHealthable
    {
        #region Properties
        int Hearts { get; set; }
        int Life { get; }
        #endregion

        #region Methods
        int TakeLives();
        #endregion
    }
}