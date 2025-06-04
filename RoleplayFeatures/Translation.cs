// Copyright 2025, github.com/BIBlical33
//
// Sets plugin's translations
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using System.ComponentModel;
using Exiled.API.Interfaces;

namespace RoleplayFeatures;

public sealed class Translation : ITranslation
{
    public string Scp079DownloadStoppingHint { get; set; } = "The download of SCP-079 has been interrupted";

    public string Scp079DownloadCompletedHint { get; set; } = "The download of SCP-079 completed";

    [Description("096 blocking calling elevator message")]
    public string Scp096ElevatorHint { get; set; } = "SCP-096 will catch up with you, it won't work";
}
