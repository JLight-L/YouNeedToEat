using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace YouNeedToEat;

public class ModEntry : Mod
{ 
    public static ModEntry Instance;
    public ModData Data { get; private set; }

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Data = helper.Data.ReadSaveData<ModData>("YouNeedToEatSave") ?? new ModData();

        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.GameLoop.Saving += OnSaving;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // 只在第一次加载存档时发邮件
        if (!Data.HasReceivedStarterFood)
        {
            Game1.addMail("YouNeedToEat.StarterFood", false);
            Data.HasReceivedStarterFood = true;
            Monitor.Log("已发送开局食物邮件！", LogLevel.Info);
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Data.AteYesterday = Data.AteToday;
        Data.AteToday = false;
        if (!Data.AteYesterday) Data.ConsecutiveEatDays = 0;

        Data.LastMealTime = -1;
        Data.BreakfastPushed = false;
        Data.LunchPushed = false;
        Data.DinnerPushed = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady) return;

        // 每10分钟减少体力
        if (e.IsMultipleOf(60 * 10)) // 10分钟 = 600 tick
        {
            Game1.player.Stamina -= 2;
            Monitor.Log("体力减少2点", LogLevel.Trace);
        }
        if (!Context.IsWorldReady) return;
        if (!e.IsMultipleOf(60)) return;   // 60 ticks ≈ 1 游戏分钟

        int now = Game1.timeOfDay;          // HHMM 格式

        // 早餐提醒 06:00-08:00
        if (!Data.BreakfastPushed && now >= 800 && Data.LastMealTime < 800)
        {
            Game1.addHUDMessage(new HUDMessage("早餐时间到！吃点东西吧～", 2));
            Data.BreakfastPushed = true;
        }

        // 午餐提醒 13:00
        if (!Data.LunchPushed && now >= 1300 && Data.LastMealTime < 1300)
        {
            Game1.addHUDMessage(new HUDMessage("午餐时间到！别饿着自己哦～", 2));
            Data.LunchPushed = true;
        }

        // 晚餐提醒 20:00
        if (!Data.DinnerPushed && now >= 2000 && Data.LastMealTime < 2000)
        {
            Game1.addHUDMessage(new HUDMessage("晚餐时间到！吃完再休息吧～", 2));
            Data.DinnerPushed = true;
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            foreach (var item in e.Added)
            {
                if (item is StardewValley.Object obj && obj.Edibility > 0)
                {
                    Data.AteToday = true;
                    Data.AteYesterday = true;
                    Data.ConsecutiveEatDays++;
                    Monitor.Log($"吃了 {obj.Name}，连续吃饭天数：{Data.ConsecutiveEatDays}", LogLevel.Info);
                    break;
                }
            }
        }
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        Helper.Data.WriteSaveData("YouNeedToEatSave", Data);
    } 
}
