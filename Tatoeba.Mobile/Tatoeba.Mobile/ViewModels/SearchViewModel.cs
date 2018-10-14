using System.Threading.Tasks;
using Xamarin.Forms;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using System.Collections.Generic;
using System.Linq;
using Tatoeba.Mobile.Storage;

namespace Tatoeba.Mobile.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel()
        {
            Title = "Search";
            SearchCommand = new Command(async () => await ExecuteSearchCommand());

            SelectedLanguageSource = MainService.Languages.Where(x => x.Iso == LocalSettings.LastIsoSearchFrom).FirstOrDefault();
            SelectedLanguageTarget = MainService.Languages.Where(x => x.Iso == LocalSettings.LastIsoSearchTo).FirstOrDefault();
        }

        public string IsOrphan { get; set; } = "No";
        public string IsTransOrphan { get; set; } = "No";
        public string IsUnapproved { get; set; } = "No";
        public string IsTransUnapproved { get; set; } = "No";
        public string HasAudio { get; set; } = "Any";
        public string TransHasAudio { get; set; } = "Any";

        public IEnumerable<string> NullableBoolValues => new List<string> { "Any", "No", "Yes" };

        public string SearchText { get; set; }
        public Command SearchCommand { get; set; }
        public IEnumerable<string> LanguageList => MainService.Languages.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker

        public Language SelectedLanguageSource { get; set; }
        public string LanguageChoiceSource
        {
            get => SelectedLanguageSource?.Label;
            set
            {
                if (value == null)
                {
                    return;
                }

                SelectedLanguageSource = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastIsoSearchFrom = SelectedLanguageSource.Iso;
            }
        }


        public Language SelectedLanguageTarget { get; set; }
        public string LanguageChoiceTarget
        {
            get => SelectedLanguageTarget?.Label;
            set
            {
                if (value == null)
                {
                    return;
                }

                SelectedLanguageTarget = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastIsoSearchTo = SelectedLanguageTarget.Iso;
            }
        }


        async Task ExecuteSearchCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var response = await MainService.SearchAsync(SearchText,
                SelectedLanguageSource.Iso,
                SelectedLanguageTarget.Iso,
                NullableBooleanStringToNullableBool(IsOrphan),
                  NullableBooleanStringToNullableBool(IsUnapproved),
                   NullableBooleanStringToNullableBool(HasAudio),
                   NullableBooleanStringToNullableBool(IsTransOrphan),
                  NullableBooleanStringToNullableBool(IsTransUnapproved),
                   NullableBooleanStringToNullableBool(TransHasAudio));

            if (response.Status != TatoebaStatus.Success)
            {
                OnError(response.Status);
                IsBusy = false;
                return;
            }

            IsBusy = false;
        }


        bool? NullableBooleanStringToNullableBool(string item)
        {
            return item == "Any" ? null : (bool?)(item == "Yes" ? true : false);
        }

    }
}