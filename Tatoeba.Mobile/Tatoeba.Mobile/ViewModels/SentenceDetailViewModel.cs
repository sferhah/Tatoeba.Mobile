using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using Xamarin.Forms;

namespace Tatoeba.Mobile.ViewModels
{
    public class SentenceDetailViewModel : BaseViewModel
    {
        public string ItemId { get; set; }

        public Contribution Original { get; private set; }

        public SentenceDetailViewModel(string itemId)
        {   
            Title = itemId;
            ItemId = itemId;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();

        public Command LoadItemsCommand { get; set; }

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

            var setenceDetails = response.Content;

            Original = setenceDetails.Sentences.FirstOrDefault();
            GroupedCells.Add(new Grouping<string, object>("Sentence #" + ItemId, setenceDetails.Sentences.Take(1)));
            GroupedCells.Add(new Grouping<string, object>(setenceDetails.Sentences.Skip(1).Count() + " Translations", setenceDetails.Sentences.Skip(1)));
            GroupedCells.Add(new Grouping<string, object>(setenceDetails.Logs.Count()+ " Logs", setenceDetails.Logs));
            GroupedCells.Add(new Grouping<string, object>(setenceDetails.Comments.Count() + " Comments", setenceDetails.Comments));

            IsBusy = false;
        }


    }
}
