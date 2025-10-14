using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace YouNeedToEat;

public class ModEntry : Mod
{
    public ModData Data { get; private set; } = new ModData();
    public static ModConfig Config { get; private set; } = new ModConfig();
    private Item? lastItemEaten;

    public override void Entry(IModHelper helper)
    {
        Data = helper.Data.ReadSaveData<ModData>("YouNeedToEatSave") ?? new ModData();
        Config = Helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.Saving += OnSaving;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (!Data.HasReceivedStarterFood)
        {
            Game1.addMail("YouNeedToEat.StartFood1", false);
            Game1.addMail("YouNeedToEat.StartFood2", false);
            Data.HasReceivedStarterFood = true;
            Monitor.Log("Start food sended.", LogLevel.Info);
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Data.AteRegularlyYesterday = Data.AteRegularlyToday;
        Data.AteRegularlyToday = false;
        if (Data.AteRegularlyYesterday ) Data.ConsecutiveEatDays = 0;

        Data.LastMealTime = -1;
        Data.BreakfastPushed = false;
        Data.LunchPushed = false;
        Data.DinnerPushed = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        var stackCost = Config.StaminaLossPer10min;
        if (!Data.AteRegularlyToday && !Data.AteRegularlyYesterday) stackCost *= 2;
        if (!Context.IsWorldReady) return;
        Game1.player.Stamina -= stackCost;

        int now = Game1.timeOfDay;          // HHMM 格式
        // 早餐提醒 08:00
        if (!Data.BreakfastPushed && now >= 800 && Data.LastMealTime < 800)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.breakfast"), 2));
            Data.BreakfastPushed = true;
        }
        // 午餐提醒 13:00
        if (!Data.LunchPushed && now >= 1300 && Data.LastMealTime < 1300)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.lunch"), 2));
            Data.LunchPushed = true;
        }
        // 晚餐提醒 20:00
        if (!Data.DinnerPushed && now >= 2000 && Data.LastMealTime < 2000)
        {
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.dinner"), 2));
            Data.DinnerPushed = true;
        }
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        Helper.Data.WriteSaveData("YouNeedToEatSave", Data);
    } 
}
