using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CN_Core;
using CN_Core.Interfaces;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Presentation.ViewModel.Controls
{
    public class BubbleMenuButtonViewModel:BaseViewModel
    {
        public FontFamilyType IconFontFamily { get; set; }

        public IconType IconFontText { get; set; }

        public string ButtonText { get; set; }

        public bool IsSelected { get; set; }

        public bool IsDisplayed { get; set; } = true;

        public ICommand ClickCommand { get; set; }

        public Func<BubbleMenuButtonViewModel, Task> ClickHandler
        {
            get { return _clickHandler; }
            set
            {
                _clickHandler = value;
                if (_clickHandler != null)
                {
                    ClickCommand = new RelayCommand(async () =>
                    {
                        await _clickHandler(this);
                    });
                }
                else
                {
                    ClickCommand = new RelayCommand(async () =>
                    {
                        await IoC.Get<IUIManager>().ShowMessage(new MessageBoxDialogViewModel()
                        {
                            Title = "Programe Error!",
                            Message = "Undefined command invoked"
                        });
                    });
                }
            }
        }

        private Func<BubbleMenuButtonViewModel, Task> _clickHandler;
        /// <summary>
        /// Constructor for design model
        /// </summary>
        public BubbleMenuButtonViewModel()
        {
        }

        /// <summary>
        /// Contructor of a BubbleMenuButton
        /// </summary>
        /// <param name="iconFontFamily"></param>
        /// <param name="iconFontText"></param>
        /// <param name="buttonText"></param>
        /// <param name="clickHandler"></param>
        public BubbleMenuButtonViewModel(
            FontFamilyType iconFontFamily, 
            IconType iconFontText, 
            string buttonText,  
            Func<BubbleMenuButtonViewModel, Task> clickHandler = null)
        {
            this.IconFontFamily = iconFontFamily;
            this.IconFontText = iconFontText;
            this.ButtonText = buttonText;
            this.ClickHandler = clickHandler;
            
        }
    }
}