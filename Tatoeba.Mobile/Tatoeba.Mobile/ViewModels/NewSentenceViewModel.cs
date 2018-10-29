using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System.Linq;
using System;

namespace Tatoeba.Mobile.ViewModels
{
    public class NewSentenceViewModel : BaseViewModel
    {
        public NewSentenceViewModel(string iso)
        {
            Title = Resx.AppResources.NewSentence;
            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            CancelCommand = new Command(async () => await ExecuteCancelCommand());

            var selectedLanguage = MainService.Languages?.Where(x => x.Iso == iso).FirstOrDefault();           

            Item = new SentenceBase
            {
                Text = "",    
                Language = selectedLanguage,
            };
        }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public SentenceBase Item { get; set; }
        public byte[] Flag => Item.Language.Flag;


        public event EventHandler Save;
        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await MainService.AddNewSentence(Item);

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