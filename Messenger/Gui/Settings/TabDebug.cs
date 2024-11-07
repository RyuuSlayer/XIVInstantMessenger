﻿using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Messenger.Configuration;
using Messenger.FriendListManager;
using NightmareUI.ImGuiElements;
using System.Threading;

namespace Messenger.Gui.Settings;

internal unsafe class TabDebug
{
    private XivChatType MType = XivChatType.TellIncoming;
    private string MName = "";
    private int MWorld = 0;
    private string MMessage = "";
    private string PMLMessage = "";
    private PseudoMultilineInput PseudoMultilineInput = new();
    internal void Draw()
    {

        try
        {
            if(ImGui.Button("Open assist window")) S.GPT4All.Window.IsOpen = true;
            if(ImGui.CollapsingHeader("AI"))
            {
                ref var cont = ref Ref<string>.Get("AIPromptDebug");
                ImGui.InputTextMultiline("##ai", ref cont, 1000, new Vector2(ImGui.GetContentRegionAvail().X, 150));
                if(ImGui.Button("Do request"))
                {
                    S.GPT4All.ResendRequest(cont);
                }
            }
            if(ImGui.CollapsingHeader("MultilineInput"))
            {
                ImGuiEx.InputTextWrapMultilineExpanding("Test", ref Ref<string>.Get(), 10000);
            }
            if(ImGui.CollapsingHeader("IPC"))
            {
                try
                {
                    ref var name = ref Ref<string>.Get("PlayerName");
                    ref var world = ref Ref<int>.Get("PlayerWorld");
                    ImGui.InputText("Players's name", ref name, 50);
                    WorldSelector.Instance.Draw(ref world);
                    ImGuiEx.Text($"GetConversationCount {S.XIMIpcManager.GetConversationCount()}");
                    ImGuiEx.Text($"GetUnreadConversationCount {S.XIMIpcManager.GetUnreadConversationCount()}");
                    ImGuiEx.TreeNodeCollapsingHeader("Conversations", () => ImGuiEx.Text(S.XIMIpcManager.GetConversations().Print("\n")));
                    ref var focus = ref Ref<bool>.Get("Focus");
                    ImGui.Checkbox("Set focus", ref focus);
                    if(ImGui.Button("Open messenger")) S.XIMIpcManager.OpenMessenger(new Sender(name, (uint)world).ToString(), focus);
                    ref var sameWorld = ref Ref<bool>.Get("SameWorld");
                    ImGui.Checkbox("Same world", ref sameWorld);
                    if(ImGui.Button("Invite")) DuoLog.Information(S.XIMIpcManager.InviteToParty(new Sender(name, (uint)world).ToString(), sameWorld) ?? "Success");
                    ImGuiEx.Text($"GetCID {S.XIMIpcManager.GetCID(new Sender(name, (uint)world).ToString()):X16}");
                }
                catch(Exception e)
                {
                    ImGuiEx.TextWrapped(e.ToString());
                }
            }
            if(ImGui.CollapsingHeader("Context"))
            {
                var c = AgentContext.Instance();
                ImGuiEx.Text($"""
                    TargetAccountId {c->TargetAccountId:X16}
                    TargetContentId {c->TargetContentId:X16}
                    TargetHomeWorldId {c->TargetHomeWorldId}
                    TargetMountSeats {c->TargetMountSeats}
                    TargetName {c->TargetName}
                    TargetObjectId {(ulong)c->TargetObjectId:X16}
                    TargetSex {c->TargetSex}
                    YesNoEventId {c->YesNoEventId}
                    YesNoTargetAccountId {c->YesNoTargetAccountId:X16}
                    YesNoTargetContentId {c->YesNoTargetContentId:X16}
                    YesNoTargetHomeWorldId {c->YesNoTargetHomeWorldId}
                    YesNoTargetName {c->YesNoTargetName}
                    YesNoTargetObjectId {(ulong)c->YesNoTargetObjectId:X16}
                    """);
            }
            if(ImGui.CollapsingHeader("Thread pool"))
            {
                ImGuiEx.Text($"{S.ThreadPool.State}");
                if(ImGui.Button("Create 10s task"))
                {
                    S.ThreadPool.Run(() =>
                    {
                        Thread.Sleep(10000);
                        DuoLog.Information("10000 end!");
                    });
                }
                if(ImGui.Button("Create 1s task"))
                {
                    S.ThreadPool.Run(() =>
                    {
                        Thread.Sleep(1000);
                        DuoLog.Information("1000 end!");
                    });
                }
            }
            if(ImGui.CollapsingHeader("CIDMAP"))
            {
                ImGuiEx.Text(S.MessageProcessor.CIDlist.Select(x => $"{x.Key}: {x.Value:X16}").Print("\n"));
            }
            if(ImGui.CollapsingHeader("LogMessages"))
            {
                var map = S.MessageProcessor.RetrieveCIDsFromLog();
                foreach(var m in map)
                {
                    ImGuiEx.Text($"{m}");
                    ImGui.Separator();
                }
            }
            if(ImGui.CollapsingHeader("Emoji"))
            {
                if(ImGui.Button("Rebuild cache"))
                {
                    S.EmojiLoader.BuildCache();
                }
                ImGui.Columns(4);
                foreach(var x in S.EmojiLoader.Emoji)
                {
                    var w = x.Value.GetTextureWrap();
                    if(w != null)
                    {
                        ImGui.Image(w.ImGuiHandle, new Vector2(24f));
                        ImGui.SameLine();
                    }
                    ImGuiEx.Text($"{x.Key}\n{x.Value.IsReady}");
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
            }
            if(ImGui.CollapsingHeader("PML"))
            {
                PseudoMultilineInput.DrawMultiline();
            }
            if(ImGui.CollapsingHeader("Senders"))
            {
                foreach(var x in S.MessageProcessor.Chats)
                {
                    ImGuiEx.Text($"{x.Key.Name}@{x.Key.HomeWorld}, {x.Key.IsGenericChannel()}");
                }
            }
            ImGuiEx.Text($"Fake event:");
            ImGuiEx.EnumCombo("XivChatType", ref MType);
            ImGui.InputText("Sender's name", ref MName, 50);
            ImGui.InputInt("Sender's world", ref MWorld);
            if(MWorld <= 0) MWorld = (int)(Svc.ClientState.LocalPlayer?.HomeWorld.Id ?? 0);
            ImGui.InputText($"Message", ref MMessage, 500);
            if(ImGui.Button("Fire event"))
            {
                var s = SeString.Empty;
                if(MName != "")
                {
                    s = new SeStringBuilder().Add(new PlayerPayload(MName, (uint)MWorld)).Build();
                }
                var n = false;
                var m = new SeStringBuilder().AddText(MMessage).Build();
                S.MessageProcessor.OnChatMessage(MType, 0, ref s, ref m, ref n);
            }
            ImGui.Separator();
            ImGuiEx.Text($"Is in instance: {P.GameFunctions.IsInInstance()}");
            ImGuiEx.Text($"Last received message: {S.MessageProcessor.LastReceivedMessage.GetPlayerName()}");
            if(ImGui.Button("Mark all as unread"))
            {
                foreach(var x in S.MessageProcessor.Chats.Values)
                {
                    x.ChatWindow.Unread = true;
                }
            }

            if(ImGui.CollapsingHeader("Friends"))
            {
                ImGuiEx.Text("Friend list: ");
                foreach(var x in FriendList.Get())
                {
                    ImGuiEx.TextCopy(x.IsOnline ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudWhite,
                        $"{x.Name} onl={x.OnlineStatus} home world={x.HomeWorld} current world={x.CurrentWorld} CID {x.ContentId:X16}");
                    List<string> sb =
                    [
                        .. MemoryHelper.Read<byte>((IntPtr)x.Data + 8, 14).Select(x => $"{x:X2}"),
                        "|",
                        .. MemoryHelper.Read<byte>((IntPtr)x.Data + 25, 8).Select(x => $"{x:X2}"),
                        "|",
                        .. MemoryHelper.Read<byte>((IntPtr)x.Data + 71, 25).Select(x => $"{x:X2}"),
                    ];
                    ImGuiEx.TextCopy(sb.Join(" "));
                }
            }
            ImGui.Separator();
            ImGuiEx.Text("Target commands");
            foreach(var x in P.TargetCommands)
            {
                ImGuiEx.Text(x);
            }
            ImGui.Separator();
            ImGuiEx.Text("Width-to-spaces");

            foreach(var x in P.WhitespaceMap)
            {
                ImGuiEx.Text($"{x.Key} => {x.Value.Length}x");
            }
            if(ImGui.Button("Fire logout event"))
            {
                P.ClientState_Logout();
            }
            ImGuiEx.Text($"a1[48]: {*(byte*)((nint)AgentCharaCard.Instance() + 48)}");
        }
        catch(Exception e)
        {
            e.Log();
        }
    }
}
