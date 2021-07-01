using System;

namespace Universe.Coin.App
{
    public delegate void CommandListener(ConsoleModifiers modifiers);

    public interface ICommandProvider
    {
        void AddCmd(ConsoleKey key, CommandListener cmd);
        void RemoveCmd(ConsoleKey key, CommandListener cmd);
    }
}