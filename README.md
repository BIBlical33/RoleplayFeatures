# RolePlayImmersion
Exiled API plugin designed to improve immersion on SCP SL RP servers

# Features
- Prohibition against leaving by lift from SCP-096.
- Hide the player's nickname when speaking on the radio.
- Allows a Chaos Insurgent to escape if they have taken SCP items and special weapons.
- Prohibit all SCPs from detonating the warhead.
- Prohibit all SCPs, except SCP-049, from interacting with the warhead panel or disabling generators.
- Sending a Cassie when a playable SCP escapes.
- Infinite tokens for MTF/CI spawning.
- Returning effects after escaping with a command `.effectsgive`.

# Default config
```yaml
RolePlayImmersion:
  is_enabled: true
  debug: false
  is_infinity_mtf_and_ci_tokens_enabled: false
  keep_effects_after_escaping: true
  # Time to apply the effects (in seconds).
  time_to_apply_effects: 60
  is_chaos_escape_with_three_scps_or_special_weapon_allowed: true
  # How many SCPs and special weapons should a Chaos Insurgent have in his inventory to escape?
  scps_and_special_weapons_count_to_chaos_escape: 3
  # Hide players nicknames when using the radio?
  is_unknown_transmitting_enabled: true
  # A text replacing player's nickname on the radio
  radio_custom_name: '???'
  # How many times is it possible to escape by elevator from SCP-096?
  escaping_by_elevator_max_times: 2
  # 096 blocking calling elevator message
  scp096_elevator_hint: 'SCP-096 will catch up with you, it won''t work'
```
