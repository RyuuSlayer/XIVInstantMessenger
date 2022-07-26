﻿using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.GameFonts;
using Messenger.FontControl;

namespace Messenger
{
    internal class Config : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        public int ContextMenuIndex = 1;
        public bool ContextMenuEnable = true;
        public bool AutoOpenTellIncoming = true;
        public bool AutoOpenTellOutgoing = true;
        public bool AutoFocusTellOutgoing = true;
        public bool QuickOpenButton = false;
        public int QuickOpenPositionX2 = 0;
        public int QuickOpenPositionY2 = 0;
        public Vector4 ColorToTitle = new(0.77f, 0.7f, 0.965f, 1f);
        public Vector4 ColorToMessage = new(0.86f, 0.52f, 0.98f, 1f);
        public Vector4 ColorFromTitle = new(0.47f, 0.30f, 0.96f, 1f);
        public Vector4 ColorFromMessage = new(0.77f, 0.69f, 1f, 1f);
        public Vector4 ColorGeneric = new(1f, 1f, 1f, 1f);
        public Vector4 ColorTitleFlash = new(0.91f, 1f, 0f, 1f);
        public bool IRCStyle = true;
        public bool PrintDate = true;
        public int HistoryAmount = 50;
        public string MessageTimestampFormat = "HH:mm:ss";
        public string DateFormat = "D";
        public bool AutoHideCombat = true;
        public bool AutoReopenAfterCombat = true;
        public bool EnableKey = true;
        public ModifierKey ModifierKey = ModifierKey.Alt;
        public VirtualKey Key = VirtualKey.R;
        public bool CommandPassthrough = true;
        public bool WindowShift = true;
        public int WindowShiftX = 50;
        public int WindowShiftY = 50;
        public float TransMin = 0.5f;
        public float TransMax = 1f;
        public float TransDelta = 0.02f;
        public bool WindowCascading = false;
        public int WindowCascadingX = 100;
        public int WindowCascadingY = 100;
        public int WindowCascadingXDelta = 50;
        public int WindowCascadingYDelta = 50;
        public int WindowCascadingReset = 10;
        public int WindowCascadingMaxColumns = 3;
        public bool ClickToOpenLink = true;
        public bool NoBringWindowToFrontIfTyping = true;
        public bool AutoTarget = true;
        public Sounds IncomingTellSound = Sounds.Sound01;
        public FontType FontType = FontType.Game;
        public GameFontFamilyAndSize Font = GameFontFamilyAndSize.Axis14;
        public ExtraGlyphRanges ExtraGlyphRanges;
        public string GlobalFont = "";
        public float FontSize = 12f;
        public bool IncreaseSpacing = false;
        public string AddonName = "_NaviMap";
        public bool CycleChatHotkey = false;
        public bool SuppressDMs = true;
        public bool QuickOpenButtonOnTop = true;
    }
}
