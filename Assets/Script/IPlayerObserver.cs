namespace Script
{
    public interface IPlayerObserver
    {
        public void OnLifeChange(int life, int value);
        public void OnScoreChange(int score, int value);
    }
}