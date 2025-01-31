using Exiled.API.Interfaces;
using System.ComponentModel;

namespace RoleplayImmersion
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;
        
        [Description("Hide players nicknames when using the radio?")]
        public bool IsUnknownTransmittingEnabled { get; set; } = true;
        
        [Description("A Text replacing player's nickname on the radio")]
        public string RadioCustomName { get; set; } = "???";

        [Description("How many times is it possible to escape by elevator from SCP-096?")]
        public uint EscapingByElevatorMaxTimes { get; set; } = 2;

        [Description("096 blocking calling elevator message")]
        public string HintMessage { get; set; } = "SCP-096 will catch up with you, it won't work";
    }
}
