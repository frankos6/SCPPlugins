﻿# CISpies

#### Adds CI spies to MTF waves

### Config

- SpyChance `float` - Chance to respawn a spy in a MTF wave (in percent)

  > default: `25`
- SpawnSpyOnce `bool` - Specifies if a spy should spawn only once per round

  > default: `true`
- ChanceModifier `float` - Value added to SpyChance depending on modifier type

  > default: `1.5`
- ModifierType - Specifies how the modifier will be added

  > Valid values: `PerPlayerOnline`, `PerPlayerRespawning`, `Disabled`
- ClassDSpies - Allows cuffed Class D to escape as spies

  > default: `false`
- ClassDSpyChance - Chance for cuffed Class D to escape as a spy

  > default: `5`

### Commands

- `.reveal` - Reveals the identity of the spy