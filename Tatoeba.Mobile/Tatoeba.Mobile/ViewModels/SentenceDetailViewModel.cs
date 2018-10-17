using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using Tatoeba.Mobile.Storage;
using Xamarin.Forms;

namespace Tatoeba.Mobile.ViewModels
{
    public class SentenceLoadedEventArgs : EventArgs
    {
        public bool IsEditable { get; set; }
    }

    public class SentenceDetailViewModel : BaseViewModel
    {
        public SentenceDetailViewModel(string itemId)
        {   
            ItemId = itemId;
            EnableRandom = ItemId == null;

            Title = EnableRandom? "Random" : itemId;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ToggleCommand = new Command(async () => await ToggleExpand());
            PreviousSentenceCommand = new Command(async () => await ExecutePreviousSentenceCommand());
            NextSentenceCommand = new Command(async () => await ExecuteNextSentenceCommand());

            SelectedLanguage = MainService.Languages?.Where(x => x.Iso == LocalSettings.LastRandomIso).FirstOrDefault();
        }

        public bool EnableRandom { get; set; }
        public bool EnablePrevious => true;
        public bool EnableNext => true;
        public string PreviousBackgroundColor => EnablePrevious ? "#D3F3B9" : "#F1F1F1";
        public string NextBackgroundColor => EnableNext ? "#D3F3B9" : "#F1F1F1";

        public Language SelectedLanguage { get; set; }
        public IEnumerable<string> LanguageList => MainService.Languages?.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string LanguageChoice
        {
            get => SelectedLanguage?.Label;
            set
            {
                if (IsBusy || value == null)
                {
                    return;
                }

                SelectedLanguage = MainService.Languages.FirstOrDefault(c => c.Label == value);
                LocalSettings.LastRandomIso = SelectedLanguage.Iso;

                ExecuteLoadItemsCommand();
            }
        }


        public event EventHandler<SentenceLoadedEventArgs> Loaded;

        public string ItemId { get; set; }
        public Contribution Original { get; private set; }
        public SentenceDetail SentenceDetail { get; private set; }

        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();


        
        public Command PreviousSentenceCommand { get; set; }
        public Command NextSentenceCommand { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command ToggleCommand { get; set; }


        public async Task ExecutePreviousSentenceCommand()
        {   
            await ExecuteLoadItemsCommand(SentenceDetail.PreviousId); 
        }

        public async Task ExecuteNextSentenceCommand()
        {
            await ExecuteLoadItemsCommand(SentenceDetail.NextId);
        }

        bool firstLoad = true;
        async Task ExecuteLoadItemsCommand(string id = null)
        {   
            if (IsBusy)
                return;

            IsBusy = true;
            
            GroupedCells.Clear();

            var response = await MainService.GetSentenceDetail(id ?? ItemId ?? SelectedLanguage.Iso);

            if (response.Status != TatoebaStatus.Success)
            {
                OnError(response.Status);
                IsBusy = false;
                return;
            }

            SentenceDetail = response.Content;

            if(firstLoad)
            {
                firstLoad = false;
                Loaded?.Invoke(this, new SentenceLoadedEventArgs { IsEditable = SentenceDetail.IsEditable });
            }
            
            Original = SentenceDetail.Sentences.FirstOrDefault();
            GroupedCells.Add(new Grouping<string, object>("Sentence #" + (id ?? ItemId ?? Original?.Id), SentenceDetail.Sentences.Take(1)));
            GroupedCells.Add(new Grouping<string, object>(SentenceDetail.CountLabel + " Translations", SentenceDetail.CollapsableTranslations));
            GroupedCells.Add(new Grouping<string, object>(SentenceDetail.Logs.Count()+ " Logs", SentenceDetail.Logs));
            GroupedCells.Add(new Grouping<string, object>(SentenceDetail.Comments.Count() + " Comments", SentenceDetail.Comments));

            IsBusy = false;
        }

        public async Task ToggleExpand()
        {
            SentenceDetail.IsExpanded = !SentenceDetail.IsExpanded;

            await Task.Delay(1); // Hack, otherwise Uwp crashes. 

            GroupedCells.RemoveAt(1);            
            GroupedCells.Insert(1, new Grouping<string, object>($"{SentenceDetail.CountLabel} Translations", SentenceDetail.CollapsableTranslations));
         
        }
    }
}
