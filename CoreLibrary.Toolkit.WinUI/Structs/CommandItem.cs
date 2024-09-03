using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.WinUI.Contracts;

namespace CoreLibrary.Toolkit.WinUI.Structs
{
    public sealed class CommandItem(
        string title,
        string iconGlyph,
        Action command,
        Func<bool>? canExecuteCommand = null
    ) : IPageItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Title => title;
        public string IconGlyph => iconGlyph;

        private bool _canExecuteCommand = canExecuteCommand?.Invoke() ?? true;
        public bool CanExecuteCommand
        {
            get => _canExecuteCommand;
            set
            {
                _canExecuteCommand = value;
                PropertyChanged?.Invoke(this, new(nameof(CanExecuteCommand)));
            }
        }

        public void Execute()
        {
            if (CanExecuteCommand)
            {
                CanExecuteCommand = false;
                command.Invoke();
                CanExecuteCommand = canExecuteCommand?.Invoke() ?? true;
            }
        }

        public void NotifyCanExecuteChanged() => CanExecuteCommand = canExecuteCommand?.Invoke() ?? true;
    }
}
