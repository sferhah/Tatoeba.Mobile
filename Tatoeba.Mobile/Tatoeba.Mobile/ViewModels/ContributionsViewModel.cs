using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tatoeba.Mobile.Storage;

namespace Tatoeba.Mobile.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        public ObservableCollection<Contribution> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ContributionsViewModel()
        {
            Title = "Random";
            Items = new ObservableCollection<Contribution>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }


        public Language SelectedLanguage { get; set; }


        public IEnumerable<string> LanguageList => MainService.Languages?.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string LanguageChoice
        {
            get => SelectedLanguage?.Label;
            set
            {
                SelectedLanguage = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastIsoSelection = SelectedLanguage.Iso;

                Filter();
            }
        }


        async Task ExecuteLoadItemsCommand()
        {   
            SelectedLanguage = MainService.Languages?.Where(x => x.Iso == LocalSettings.LastIsoSelection).FirstOrDefault();

            OnPropertyChanged(nameof(LanguageList));
            OnPropertyChanged(nameof(LanguageChoice));
        }

        async Task Filter()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Items.Clear();

            var response = await MainService.GetLatestContributions(SelectedLanguage.Iso);


            if(response.Status != TatoebaStatus.Success)
            {
                OnError(response.Status);
                IsBusy = false;
                return;
            }

            foreach (var item in response.Content)
            {
                Items.Add(item);
            }

            IsBusy = false;
        }
    }
}