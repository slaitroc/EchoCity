# Attraction System

## 0. Tuning Parameters

> [!Caution]
>If you are here to tune the attraction system, you will likely want to modify the following parameters in the `SOSoundSource` Scriptable Object:
>
>- `Sound Class Intensity Factor Multiplier`: this will slightly adjust the intensity of the sound class without affecting the sound class.
That is to avoid messing with the sound class parameters which require a deeper understanding of the system.
>- `Audio Length Override`: this will regolate the duration of the calculation, which is often the easy way to adjust, tune the attraction behaviour to fit the sound.

## 1. Attraction Function

The attraction system quantifies how much an enemy is drawn to a sound source. This is represented by a dynamic value called **Attraction (A)**, which fluctuates based on sound properties and distance.

**A** is updated every frame if and only if the attraction system's `Compute` property is set to true.

**A** is based on the last perceived sound. It is the enemy's responsibility to filter out irrelevant sounds (e.g., sounds from other enemies) before passing them to the attraction system.

### 1.1 Increment

**A** increases based on distance and sound properties.

The following formula gets applied if the last perceived sound is still active.

$$\Delta A_{inc} = \min \left( \frac{I_{enemy} \cdot I_{sound}}{1 + \left( \frac{dist}{R_{sound}} \right)^{D_{sound}}} \cdot \Delta t, \text{MaxInc} \right)$$

* $I_{enemy}$: `SOEnemyData.AIntensity` Enemy's hearing sensitivity (constant per enemy type).
* $I_{sound}$ = `SOSoundClass.IntensityFactor` : Base strength of the sound.
* $R_{sound}$ = `SOSoundClass.RangeFactor` : Distance where attraction strength is **halved (50%)**.
* $D_{sound}$ = `SOSoundClass.Decay` : How quickly the sound's influence diminishes with distance.
* $dist$: maximum value between `SOEnemyData.DistanceLowerBound` and the actual distance between enemy and sound source.
* MaxInc: `SOEnemyData.AMaxIncrementPerFrame` Maximum attraction increase per frame (avoid using this).

### 1.2 Decrement

**A** always decreases. It decays faster depending on the sound class and time since the last sound stopped.

$$\Delta A_{dec} = (\frac{D_{enemy}}{P_{sound}}) \cdot (1 + T_{silence} \cdot G_{rate}) \cdot \Delta t$$

* $D_{enemy}$: `SOEnemyData.ADecay` Enemy's forgetfulness (constant per enemy type). Forgetfulness velocity.
* $P_{sound}$ = `SOSoundClass.Persistence` : Sound's persistence. Higher values mean the sound is remembered longer.
* $T_{silence}$: Time since the last sound ended.
* $G_{rate}$ = `SOEnemyData.DecayGrowthRate` How fast the forgetfulness accelerates. Forgetfulness acceleration.

Persistence: Using $D_{sound}$ here ensures that sounds with high D are forgotten quickly, while sounds with low D are forgotten slowly.
