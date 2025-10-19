<p align="right">
  <a href="README.zh.md">🔤 中文README</a>
</p>

# YouNeedToEat-en
A Stardew Valley mod, aimed at reminding players to eat like in real life.

## Features

### 🍽️ Meal Tracking System
- "Eating" (as referenced in this mod): Specifically refers to the consumption of "cooked dishes" (i.e., items categorized under Stardew Valley's "Cooking Category")
- Tracks three daily meals (breakfast, lunch, and dinner) with time-based reminders:
  - Breakfast window: 6:00 AM - 11:00 AM
  - Lunch window: 11:00 AM - 4:00 PM
  - Dinner window: 4:00 PM - next day 2:00 AM
- Visual HUD notifications remind you when each meal time arrives
- Regular eating: eat 2 out of 3 daily meals
    - Maintains streak counter for consecutive days with regular eating

### 🏃 Stamina Management
- Gradual stamina loss every 10 minutes (*modifiable in the configuration*)
- Increased stamina drain for:
  - 1 day of irregular eating (2x stamina loss)
  - 2+ consecutive days of irregular eating (4x stamina loss)

### 🎁 Starting Gift
- New saves receive welcome mail with starter food items to help establish good eating habits

## Configuration
- Base stamina loss rate (`StaminaLossPer10min`, default: 1)

## How It Works
The mod tracks your eating habits across multiple days, rewarding consistent meal times with better stamina retention. Missed meals increase stamina drain, making regular eating more essential.
