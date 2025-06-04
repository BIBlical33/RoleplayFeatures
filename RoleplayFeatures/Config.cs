// Copyright 2025, github.com/BIBlical33
//
// Sets plugin config
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using Exiled.API.Interfaces;
using System.ComponentModel;

namespace RoleplayFeatures;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; } = false;

    public bool IsScpEscapeCassiesEnabled { get; set; } = true;

    public string ScpEscapeCassieContent { get; set; } = "{0} has escaped from the facility";

    public bool IsChaosEscapeAllowed { get; set; } = true;

    [Description("How many SCPs and special weapons should a Chaos Insurgent have in his inventory to escape?")]
    public uint ScpsAndSpecialWeaponsCountToChaosEscape { get; set; } = 3;

    public Scp079EscapeConfig Scp079Escape { get; set; } = new();

    public bool IsUnknownTransmittingEnabled { get; set; } = true;

    [Description("A text replacing player's nickname on the radio")]
    public string RadioCustomName { get; set; } = "???";

    [Description("How many times is it possible to escape by elevator from SCP-096?")]
    public uint EscapingByElevatorMaxTimes { get; set; } = 2;
}
