using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System;

namespace Tatoeba.Mobile.ViewModels
{
    public class EditSentenceViewModel : BaseViewModel
    {   
        public EditSentenceViewModel(Contribution original)
        {
            this.Item = original;
            Title = Resx.AppResources.EditSentence;
            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            CancelCommand = new Command(async () => await ExecuteCancelCommand());            
        }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Contribution Item { get; set; }

        public byte[] Flag => Item.Language.Flag;    

        public event EventHandler Save;
        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await MainService.EditSentence(Item);

            Save?.Invoke(this, EventArgs.Empty);

            IsBusy = false;
        }

        public event EventHandler Cancel;
        async Task ExecuteCancelCommand()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }
    }
}