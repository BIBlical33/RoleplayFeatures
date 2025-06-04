# RoleplayFeatures

[![Version](https://img.shields.io/github/v/release/BIBlical33/RoleplayFeatures?sort=semver&style=flat-square&color=8DBBE9&label=Version)]()
[![License](https://img.shields.io/badge/License-CC%20BY%E2%80%93SA%203.0-df967f?style=flat-square)]()
[![Downloads](https://img.shields.io/github/downloads/BIBlical33/RoleplayFeatures/total?style=flat-square&label=Downloads&color=orange)]()

## Overview
Exiled API plugin designed to improve immersion on SCP SL RP servers.

## Features
- Prohibition against leaving by lift from SCP-096.
- Escape mechanic for SCP-079 using `.079download`, `.079gates`, assisted by SCP-049 or human.
- Hide the player's nickname when speaking on the radio.
- Allows a Chaos Insurgent to escape if they have taken SCP items and special weapons.
- Prohibit all SCPs from detonating the warhead.
- Prohibit all SCPs, except SCP-049 and SCP-3114, from interacting with the warhead panel or disabling generators.
- Sending a Cassie when a playable SCP escapes.

## Installation
1. Install [Exiled ](https://github.com/ExSLMod-Team/EXILED Install [Exiled framework](https://github.com/ExMod-Team/EXILED) v9.6.0+.
2. Chose git release, download `RoleplayFeatures.dll` and drop into server `.config/Exiled/Plugins` path.
 
## Default config
```yaml
is_enabled: true
debug: false
is_scp_escape_cassies_enabled: true
scp_escape_cassie_content: '{0} has escaped from the facility'
is_chaos_escape_allowed: true
# How many SCPs and special weapons should a Chaos Insurgent have in his inventory to escape?
scps_and_special_weapons_count_to_chaos_escape: 3
scp079_escape:
# Can SCP-079 escape with outside help?
  is_downloadable: true
  # How long does it take to load SCP-079 in seconds?
  download_duration: 100
  is_download_cassie_enabled: true
  cassie_download_message: 'Attention . Unauthorized access to SCP 0 7 9 containment chamber has been detected . Security check requires .'
# Hide players nicknames when using the radio?
is_unknown_transmitting_enabled: true
# A text replacing player's nickname on the radio
radio_custom_name: '???'
# How many times is it possible to escape by elevator from SCP-096?
escaping_by_elevator_max_times: 2
```
