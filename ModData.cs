namespace YouNeedToEat
{
    public class ModData
    {
        public bool HasReceivedStarterFood { get; set; } = false;
        public bool AteRegularlyToday { get; set; } = false;
        public bool AteRegularlyYesterday { get; set; } = false;
        public int ConsecutiveEatDays { get; set; } = 0;
        
        // 规律饮食模块
        public int LastMealTime { get; set; } = -1;   // 最后一次进食的游戏分钟
        public bool BreakfastPushed { get; set; } = false;
        public bool LunchPushed { get; set; } = false;
        public bool DinnerPushed { get; set; } = false;
    }
}