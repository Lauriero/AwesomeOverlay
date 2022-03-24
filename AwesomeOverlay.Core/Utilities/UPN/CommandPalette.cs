using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AwesomeOverlay.Core.Utilities.UPN
{
    public class CommandPalette
    {
        private List<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// Gets count of commands in palette
        /// </summary>
        public int CommandsCount => commands.Count;

        /// <summary>
        /// Add new command to the list of commands in palette
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }

        /// <summary>
        /// Creates read only collection of commands in palette
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<ICommand> GetCommands()
        {
            return new ReadOnlyCollection<ICommand>(commands);
        }

        /// <summary>
        /// Clears palette
        /// </summary>
        public void ClearPalette()
        {
            commands.Clear();
        }
    }
}