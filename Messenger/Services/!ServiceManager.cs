﻿using Messenger.GPT4All;
using Messenger.Gui;
using Messenger.Services.EmojiLoaderService;
using Messenger.Services.MessageProcessorService;

namespace Messenger.Services;
public static class ServiceManager
{
    public static ThreadPool ThreadPool { get; private set; }
    public static Memory Memory { get; private set; }
    public static EmojiLoader EmojiLoader { get; private set; }
    public static ContextMenuManager ContextMenuManager { get; private set; }
    public static MessageProcessor MessageProcessor { get; private set; }
    public static IPCProvider IPCProvider { get; private set; }
    public static XIMIpcManager XIMIpcManager { get; private set; }
    public static XIMModalWindow XIMModalWindow { get; private set; }
    public static GPT4AllService GPT4All { get; private set; }
}
