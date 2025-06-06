﻿using ECommons.Automation;
using ECommons.ExcelServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.UI;

#pragma warning disable CS8632,CS0649
namespace Messenger;

internal unsafe class GameFunctions : IDisposable
{
    internal GameFunctions()
    {
    }

    public void Dispose()
    {
    }

    internal void PlaySound(Sounds id)
    {
        UIGlobals.PlaySoundEffect((uint)id);
    }

    internal bool IsInInstance()
    {
        return Svc.Condition[ConditionFlag.BoundByDuty56];
    }

    internal void SendFriendRequest(string name, ushort world)
    {
        if(ExcelWorldHelper.Get(world) != null && EzThrottler.Throttle("AddToFriendlist"))
        {
            S.Memory.ReplaceName = $"{name}@{ExcelWorldHelper.GetName(world)}";
            Chat.Instance.ExecuteCommand($"/friendlist add {S.Memory.Placeholder}");
        }
    }

    internal void AddToBlacklist(string name, ushort world)
    {
        if(ExcelWorldHelper.Get(world) != null && EzThrottler.Throttle("AddToFriendlist"))
        {
            S.Memory.ReplaceName = $"{name}@{ExcelWorldHelper.GetName(world)}";
            Chat.Instance.ExecuteCommand($"/blacklist add {S.Memory.Placeholder}");
        }
    }
}
