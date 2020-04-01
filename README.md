# EXILEDRoundMods
A EXILED plugin that changes game mechanics without creating entirely new gamemodes.

To display the current mods on the server list, add `{rm_current_mods}` to the name of your server.

For RA Commands, there is one permission: `roundmod.setmod`.

Current Mods are displayed on the player's screen when they join. If you are using a plugin to show some kind of Server Title when you join, do it the second the player joins, this plugin will wait ~0.1 seconds before broadcasting to the new player.

## Config

The `.` is just a subcategory in the config. See example for more.

### rm_rng.weapons

Description: The items (by number) that are seen to the plugin as weapons.

Type: List

Default: 13, 20, 24, 23, 21, 30, 16, 26, 25

### rm_rng.meds

Description: The items (by number) that are seen to the plugin as medical items.

Type: List

Default: 33, 14, 34

### rm_rng.samescps

Description: The classes (by number) used for the mod SINGLESCPTYPE (yes, you can use non-SCP classes, but I wouldn't do it).

Type: List

Default: 5, 9, 3, 0, 16, 17

### rm_rng.bossscps

Description: The classes (by number) used for the mod SCPBOSS (yes, you can use non-SCP classes, but I wouldn't do it).

Type: List

Default: 9, 3, 0, 16, 17

### rm_rng.noinfect

Description: The classes (by number) NOT used for the mod CLASSINFECT.

Type: List

Default: 7

### rm_max_mods

Description: The maximum amount of mods used at a time. Can be overridden via RA commands.

Type: int

Default: the amount of different types of mods (currently 10)

### rm_allow_`MODTYPE`

Description: Tells the plugin to allow/disallow a mod from being used EVER. If you want the mod to still be accessable but not used, set `rm_chance_MODTYPE` to `0`.

Type: bool

Default: true

### rm_chance_`MODTYPE`

Description: The amount of times to add this type to a list of possible types. Types are then selected randomly from this list.

Type: int

Default: 1

## RA Commands

### rm_mod

Displays some info about the plugin, as well as the allowed mods.

### rm_mod_`MODTYPE`

Toggles the mod of `MODTYPE` to on or off.

### rm_mod`MODTYPE` [true|false]

Sets the mod of `MODTYPE` to be on or off.

### rm_set_mod

Displays all current mods, by codename.

### rm_set_mod [modtypes, as, a, list]

Sets the current mods to the list of mods. Separate the types by `,`.

## Example Config

```yaml

rm_rng:
  weapons:
  - 13
  - 20
  - 24
  - 23
  - 21
  - 30
  - 16
  - 26
  - 25
  meds:
    - 33
    - 14
    - 34
  bossscps:
    - 9
    - 3
    - 0
    - 16
    - 17
  samescps:
    - 5
    - 9
    - 3
    - 0
    - 16
    - 17

rm_max_mods: 10

rm_allow_NONE: true
rm_allow_SINGLESCPTYPE: true
rm_allow_PLAYERSIZE: true
rm_allow_SCPBOSS: true
rm_allow_NOWEAPONS: false # possibly bugged
rm_allow_NORESPAWN: true
rm_allow_EXPLODEONDEATH: true
rm_allow_FINDWEAPONS: true
rm_allow_ITEMRANDOMIZER: true
rm_allow_CLASSINFECT: true

rm_translation_NONE: None
rm_translation_SINGLESCPTYPE: Single SCP Type
rm_translation_PLAYERSIZE: Random Player Size
rm_translation_SCPBOSS: SCP Boss
rm_translation_NOWEAPONS: No Weapons
rm_translation_NORESPAWN: No Respawn
rm_translation_EXPLODEONDEATH: Explode on Death
rm_translation_FINDWEAPONS: Find Weapons
rm_translation_ITEMRANDOMIZER: Random Items
rm_translation_CLASSINFECT: Class Infection

rm_chance_NONE: 5
rm_chance_SINGLESCPTYPE: 2
rm_chance_PLAYERSIZE: 4
rm_chance_SCPBOSS: 1
rm_chance_NOWEAPONS: 0
rm_chance_NORESPAWN: 1
rm_chance_EXPLODEONDEATH: 2
rm_chance_FINDWEAPONS: 3
rm_chance_ITEMRANDOMIZER: 1
rm_chance_CLASSINFECT: 1
```
