using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
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
            Title = itemId;
            ItemId = itemId;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ToggleCommand = new Command(async () => await ToggleExpand());
        }

        public event EventHandler<SentenceLoadedEventArgs> Loaded;

        public string ItemId { get; set; }
        public Contribution Original { get; private set; }
        public SentenceDetail SentenceDetail { get; private set; }

        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();

        public Command LoadItemsCommand { get; set; }
        public Command ToggleCommand { get; set; }


        bool firstLoad = true;
        async Task ExecuteLoadItemsCommand()
        {   
            if (IsBusy)
                return;

            IsBusy = true;
            
            GroupedCells.Clear();

            var response = await MainService.GetSentenceDetail(ItemId);

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
            GroupedCells.Add(new Grouping<string, object>("Sentence #" + ItemId, SentenceDetail.Sentences.Take(1)));
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

            //var group = GroupedCells[1];
            //group.Clear();

            //group.Key = $"{SentenceDetail.CountLabel} Translations";

            //foreach (var item in SentenceDetail.CollapsableTranslations)
            //{
            //    group.Add(item);
            //}
        }
    }
}
