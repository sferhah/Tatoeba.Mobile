using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System.Collections.Generic;
using System.Linq;
using Tatoeba.Mobile.Storage;

namespace Tatoeba.Mobile.ViewModels
{
    public class ContributionsViewModel : BaseViewModel
    {
        public ContributionsViewModel()
        {
            Title = Resx.AppResources.Recent;
            Items = new ObservableCollection<Contribution>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SelectedLanguage = MainService.Languages?.Where(x => x.Iso == LocalSettings.LastIsoSelection).FirstOrDefault();
        }

        protected override void OnFirstAppear()
        {
            base.OnFirstAppear();

            if (Items.Count == 0)
                LoadItemsCommand.Execute(null);
        }

        public ObservableCollection<Contribution> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Language SelectedLanguage { get; set; }


        public IEnumerable<string> LanguageList => MainService.Languages?.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string LanguageChoice
        {
            get => SelectedLanguage?.Label;
            set
            {
                if(value == null 
                    || value == LanguageChoice)
                {
                    return;
                }

                SelectedLanguage = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastIsoSelection = SelectedLanguage.Iso;

                ExecuteLoadItemsCommand();
            }
        }


        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Items.Clear();

            var response = await MainService.GetLatestContributions(SelectedLanguage.Iso);


            if(response.Status != TatoebaStatus.Success)
            {
                OnError(response);
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