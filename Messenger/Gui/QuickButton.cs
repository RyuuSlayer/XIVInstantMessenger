﻿using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Messenger.Gui;

internal unsafe class QuickButton : Window
{
    internal QuickButton() : base("MessengerQuickButton", 
        ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.AlwaysUseWindowPadding
        , true)
    {
        this.IsOpen = true;
        this.RespectCloseHotkey = false;
    }

    public override bool DrawConditions()
    {
        AtkUnitBase* addon = null;
        var ret = C.QuickOpenButton
            && (C.AddonName == string.Empty || (TryGetAddonByName(C.AddonName, out addon)
            && addon->IsVisible));
        if (ret)
        {
            this.Position = new Vector2(C.QuickOpenPositionX2, C.QuickOpenPositionY2);
            if (addon != null)
            {
                this.Position += new Vector2(addon->X, addon->Y);
            }
        }
        return ret;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);
    }

    public override void Draw()
    {
        if (C.QuickOpenButtonOnTop)
        {
            CImGui.igBringWindowToDisplayFront(CImGui.igGetCurrentWindow());
        }
        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
        var col = false;
        if(P.Hidden)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiCol.Text.GetFlashColor(C.DefaultChannelCustomization));
            col = true;
        }
        if (ImGuiEx.IconButton(FontAwesomeIcon.MailBulk))
        {
            ImGui.OpenPopup("Select target");
        }
        if (col)
        {
            ImGui.PopStyleColor();
        }
        ImGui.PopStyleColor(3);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(7, 7));
        if (ImGui.BeginPopup("Select target"))
        {
            if (P.Chats.Count == 0)
            {
                ImGuiEx.Text("There is nothing here yet...");
            }
            else
            {
                ImGuiEx.WithTextColor(ImGuiColors.DalamudGrey, delegate
                {
                    if (ImGui.Selectable("Close all chat windows"))
                    {
                        Svc.Commands.ProcessCommand("/xim close");
                    }
                });
                var tsize = ImGui.CalcTextSize("");
                Sender? toRem = null;
                foreach (var x in P.Chats)
                {
                    var cur = ImGui.GetCursorPos();
                    if (ImGui.Selectable($"{x.Key.GetChannelName()} ({x.Value.Messages.Count})", false, ImGuiSelectableFlags.None, new Vector2(200f.Scale(), tsize.Y)))
                    {
                        P.Hidden = false;
                        x.Value.ChatWindow.IsOpen = true;
                        x.Value.SetFocus = true;
                        if (Svc.Condition[ConditionFlag.InCombat])
                        {
                            x.Value.ChatWindow.KeepInCombat = true;
                            Notify.Info("This chat will not be hidden in combat");
                        }
                    }
                    ImGui.SameLine(0, 0);
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                    if (ImGui.Selectable($"   ##{x.Key.GetChannelName()}", false, ImGuiSelectableFlags.DontClosePopups))
                    {
                        toRem = x.Key;
                    }
                    ImGui.PopStyleColor();
                }
                if (toRem != null)
                {
                    P.WindowSystemChat.RemoveWindow(P.Chats[toRem.Value].ChatWindow);
                    P.Chats.Remove(toRem.Value);
                    toRem = null;
                }
            }
            ImGui.EndPopup();
        }
        ImGui.PopStyleVar();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar(2);
    }
}
