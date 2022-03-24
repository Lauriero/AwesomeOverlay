using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows.Input;

namespace AwesomeOverlay.Core.Utilities.CollectionUtilities
{
    public class MenuElementController : BindableBase
    {
        public event Action OnSelect;

        public MenuElementController()
        {
            SelectCommand = new DelegateCommand(Select);
        }

        public ICommand SelectCommand { get; }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set {
                SetProperty(ref _selected, value);
            }
        }


        public void Select()
        {
            Selected = true;
            OnSelect?.Invoke();
        }

        public void Unselect()
        {
            Selected = false;
        }
    }
}
