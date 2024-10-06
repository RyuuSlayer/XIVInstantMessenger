﻿using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using ECommons.ChatMethods;
using ECommons.ExcelServices;
using Messenger.Configuration;
using Sender = Messenger.Configuration.Sender;

namespace Messenger.Services;

public class ContextMenuManager : IDisposable
{
    private static readonly string[] ValidAddons =
    [
        null,
        "PartyMemberList",
        "FriendList",
        "FreeCompany",
        "LinkShell",
        "CrossWorldLinkshell",
        "_PartyList",
        "ChatLog",
        "LookingForGroup",
        "BlackList",
        "ContentMemberList",
        "SocialList",
        "ContactList",
    ];

    private ContextMenuManager()
    {
        Svc.ContextMenu.OnMenuOpened += OpenContextMenu;
    }

    public void Dispose()
    {
        Svc.ContextMenu.OnMenuOpened -= OpenContextMenu;
    }

    private void OpenContextMenu(IMenuOpenedArgs args)
    {
        if (C.ContextMenuEnable && ValidAddons.Contains(args.AddonName) && args.Target is MenuTargetDefault def && def.TargetName != null && ExcelWorldHelper.Get(def.TargetHomeWorld.Id, true) != null)
        {
            args.AddMenuItem(new()
            {
                OnClicked = (_) =>
                {
                    Sender s = new(def.TargetName, def.TargetHomeWorld.Id);
                    P.OpenMessenger(s);
                    S.MessageProcessor.Chats[s].SetFocusAtNextFrame();
                    S.MessageProcessor.Chats[s].Scroll();
                    if (Svc.Condition[ConditionFlag.InCombat])
                    {
                        S.MessageProcessor.Chats[s].ChatWindow.KeepInCombat = true;
                        Notify.Info("This chat will not be hidden in combat");
                    }
                },
                PrefixChar = 'M',
                Priority = C.ContextMenuPriority,
                Name = "Messenger",
            });
            if(C.EnableEngagements && C.EnableEngagementsContext)
            {
                args.AddMenuItem(new()
                {
                    OnClicked = (o) =>
                    {
                        List<MenuItem> items = [];
                        Sender sender = new(def.TargetName, def.TargetHomeWorld.Id);
                        foreach(var x in C.Engagements.Where(s => s.Enabled).OrderByDescending(s => s.LastUpdated))
                        {
                            if(x.Participants.Contains(sender))
                            {
                                items.Add(new()
                                {
                                    Prefix = (SeIconChar)'',
                                    Name = x.Name,
                                    OnClicked = (_) =>
                                    {
                                        var s = x.GetSender();
                                        P.OpenMessenger(s);
                                        S.MessageProcessor.Chats[s].SetFocusAtNextFrame();
                                        S.MessageProcessor.Chats[s].Scroll();
                                        if(Svc.Condition[ConditionFlag.InCombat])
                                        {
                                            S.MessageProcessor.Chats[s].ChatWindow.KeepInCombat = true;
                                            Notify.Info("This chat will not be hidden in combat");
                                        }
                                    }
                                });
                            }
                            else
                            {
                                items.Add(new()
                                {
                                    Prefix = (SeIconChar)'',
                                    Name = x.Name,
                                    OnClicked = (_) =>
                                    {
                                        var s = x.GetSender();
                                        x.Participants.Add(sender);
                                        P.OpenMessenger(s);
                                        S.MessageProcessor.Chats[s].SetFocusAtNextFrame();
                                        S.MessageProcessor.Chats[s].Scroll();
                                        if(Svc.Condition[ConditionFlag.InCombat])
                                        {
                                            S.MessageProcessor.Chats[s].ChatWindow.KeepInCombat = true;
                                            Notify.Info("This chat will not be hidden in combat");
                                        }
                                    }
                                });
                            }
                        }
                        o.OpenSubmenu("Engagements", items);
                    },
                    PrefixChar = 'M',
                    Priority = C.ContextMenuPriority,
                    IsSubmenu = true,
                    Name = "Engagements",
                });
            }
        }
    }
}