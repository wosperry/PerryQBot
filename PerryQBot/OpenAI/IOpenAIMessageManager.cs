﻿namespace PerryQBot.OpenAI;

public interface IOpenAIMessageManager
{
    Task<List<string>> BuildUserRequestMessagesAsync(string senderId, string message);
}