// Copyright 2025, github.com/BIBlical33
//
// Scp079 escape config
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using System.ComponentModel;

namespace RoleplayFeatures
{
    public class Scp079EscapeConfig
    {
        [Description("Can SCP-079 escape with outside help?")]
        public bool IsDownloadable { get; set; } = true;

        [Description("How long does it take to load SCP-079 in seconds?")]
        public uint DownloadDuration { get; set; } = 100;

        public bool IsDownloadCassieEnabled { get; set; } = true;

        public string CassieDownloadMessage { get; set; } = "Attention . Unauthorized access to SCP 0 7 9 containment chamber has been detected . Security check requires .";
    }
}