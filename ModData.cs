namespace YouNeedToEat
{
    public class ModData
    {
        public bool HasReceivedStartFood { get; set; } = false;
        public bool AteRegularlyToday { get; set; } = false;
        public bool AteRegularlyYesterday { get; set; } = false;
        public bool AteRegularlyYesyesterday { get; set; } = false;
        public int ConsecutiveEatDays { get; set; } = 0;
        
        // 规律饮食模块
        public bool BreakfastAte { get; set; } = false;
        public bool LunchAte { get; set; } = false;
        public bool DinnerAte { get; set; } = false;
    }
}