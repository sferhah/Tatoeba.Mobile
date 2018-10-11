﻿using System;
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
        }

        public event EventHandler<SentenceLoadedEventArgs> Loaded;

        public string ItemId { get; set; }
        public Contribution Original { get; private set; }
        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();

        public Command LoadItemsCommand { get; set; }

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

            var sentenceDetail = response.Content;

            if(firstLoad)
            {
                firstLoad = false;
                Loaded?.Invoke(this, new SentenceLoadedEventArgs { IsEditable = sentenceDetail.IsEditable });
            }

            Original = sentenceDetail.Sentences.FirstOrDefault();
            GroupedCells.Add(new Grouping<string, object>("Sentence #" + ItemId, sentenceDetail.Sentences.Take(1)));
            GroupedCells.Add(new Grouping<string, object>(sentenceDetail.Sentences.Skip(1).Count() + " Translations", sentenceDetail.Sentences.Skip(1)));
            GroupedCells.Add(new Grouping<string, object>(sentenceDetail.Logs.Count()+ " Logs", sentenceDetail.Logs));
            GroupedCells.Add(new Grouping<string, object>(sentenceDetail.Comments.Count() + " Comments", sentenceDetail.Comments));

            IsBusy = false;
        }


    }
}
