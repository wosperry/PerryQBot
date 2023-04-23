namespace PerryQBot.Commands
{
    public interface ICommandHandler
    {
        bool IsContinueAfterHandled { get; set; }
        string ResponseMessage { get; set; }
        string RequestMessage { get; set; }

        Task HandleAsync(CommandContext context);
    }
}