using System;
using System.Threading;

namespace Universe.Coin.App
{
    public delegate void CommandAction(ConsoleModifiers modifiers);

    public interface ICommandProvider
    {
        //void AddAction(ConsoleKey key, CommandAction cmd);
        //void RemoveAction(ConsoleKey key, CommandAction cmd);
        void AddSignal(ConsoleKey key, EventWaitHandle signal);
        void RemoveSignal(ConsoleKey key, EventWaitHandle signal);
    }
}