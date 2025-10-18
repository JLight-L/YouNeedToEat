using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace YouNeedToEat;

public class ModEntry : Mod
{
    public ModData Data { get; private set; } = new ModData();
    public static ModConfig Config { get; private set; } = new ModConfig();

    public override void Entry(IModHelper helper)
    {
        Config = Helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.Saving += OnSaving;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        Data = Helper.Data.ReadSaveData<ModData>("YouNeedToEatSave") ?? new ModData();
        if (!Data.HasReceivedStartFood)
        {
            Game1.addMail("YouNeedToEat.StartFood1", false);
            Game1.addMail("YouNeedToEat.StartFood2", false);
            Data.HasReceivedStartFood = true;
            Monitor.Log("Start food sended.", LogLevel.Info);
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Data.AteRegularlyYesyesterday = Data.AteRegularlyYesterday;
        Data.AteRegularlyYesterday = Data.AteRegularlyToday;
        Data.AteRegularlyToday = false;
        if (!Data.AteRegularlyYesterday) Data.ConsecutiveEatDays = 0;
        else Data.ConsecutiveEatDays += 1;
        if (!Data.AteRegularlyYesterday && !Data.AteRegularlyYesyesterday)
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.hungry2"), 2));
        else if (!Data.AteRegularlyYesterday)
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.hungry1"), 2));

        Data.LastMealTime = -1;
        Data.BreakfastPushed = false;
        Data.LunchPushed = false;
        Data.DinnerPushed = false;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        var stackCost = Config.StaminaLossPer10min;
        if (!Data.AteRegularlyYesterday) stackCost *= 2;
        if (!Data.AteRegularlyYesyesterday && !Data.AteRegularlyYesterday && !Data.AteRegularlyToday) stackCost *= 2;
        if (!(Game1.player.Stamina <= stackCost * 4)) Game1.player.Stamina -= stackCost;

        int now = Game1.timeOfDay;          // HHMM 格式
        // 早餐提醒 06:00-08:00-11:00
        if (!Data.BreakfastPushed && now >= 800 && Data.LastMealTime < 600)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.breakfast"), 2));
            Data.BreakfastPushed = true;
        }
        // 午餐提醒 11:00-13:00-16:00
        if (!Data.LunchPushed && now >= 1300 && Data.LastMealTime < 1100)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.lunch"), 2));
            Data.LunchPushed = true;
        }
        // 晚餐提醒 16:00-20:00-26:00
        if (!Data.DinnerPushed && now >= 2000 && Data.LastMealTime < 1600)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.dinner"), 2));
            Data.DinnerPushed = true;
        }
    }

    bool _isEatingNow = false;
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // if (!Context.IsWorldReady) return;
        // _lastStamina = (int)Game1.player.Stamina; // 实时缓存
        var farmer = Game1.player;
        if (farmer != null)
        {
            if (!_isEatingNow)
            {
                if (farmer.isEating && farmer.itemToEat is StardewValley.Object obj && obj.Category == StardewValley.Object.CookingCategory)
                {
                    _isEatingNow = true;
                    OnEating();
                    Monitor.Log($"检测到玩家吃掉{obj.name}", LogLevel.Info);
                }
            }
            else if (!farmer.isEating) _isEatingNow = false;
        }
    }

    private void OnEating()
    {
        Data.LastMealTime = Game1.timeOfDay;
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        Helper.Data.WriteSaveData("YouNeedToEatSave", Data);
    } 
}
