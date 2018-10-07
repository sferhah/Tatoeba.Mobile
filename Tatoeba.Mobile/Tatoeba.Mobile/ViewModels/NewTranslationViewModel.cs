using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tatoeba.Mobile.Storage;
using System;

namespace Tatoeba.Mobile.ViewModels
{
    public class NewTranslationViewModel : BaseViewModel
    {
        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }

        public SentenceBase Item { get; set; }
        public Contribution Original { get; set; }

        public NewTranslationViewModel(Contribution original)
        {
            this.Original = original;
            Title = "New translation";
            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            CancelCommand = new Command(async () => await ExecuteCancelCommand());

            var selectedLanguage = MainService.Languages?.Where(x => x.Iso == LocalSettings.LastIsoTranslation).FirstOrDefault();

            if(selectedLanguage.Iso == null)
            {
                selectedLanguage = MainService.Languages?.Where(x => x.Iso == "eng").FirstOrDefault();
            }


            Item = new SentenceBase
            {
                Text = "",    
                Language = selectedLanguage,
            };
        }


        public byte[] Flag => Item.Language.Flag;


        public IEnumerable<string> LanguageList => MainService.Languages?.Where(x=> x.Iso != null).Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string LanguageChoice
        {
            get => Item.Language.Label;
            set
            {
                if(value == null)
                {
                    return;
                }

                Item.Language = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastIsoTranslation = Item.Language.Iso;
                OnPropertyChanged(nameof(Flag));
            }
        }


        public event EventHandler Save;
        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await MainService.SaveTranslation(Original.Id, Item);

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