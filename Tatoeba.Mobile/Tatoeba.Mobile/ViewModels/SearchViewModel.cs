﻿using System.Threading.Tasks;
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
            Title = Resx.AppResources.SearchTitle;
            SearchCommand = new Command(async () => await ExecuteSearchCommand());

            SelectedLanguageSource = MainService.Languages.Where(x => x.Iso == LocalSettings.LastIsoSearchFrom).FirstOrDefault();
            SelectedLanguageTarget = MainService.Languages.Where(x => x.Iso == LocalSettings.LastIsoSearchTo).FirstOrDefault();
        }

        public string IsOrphan { get; set; } = Resx.AppResources.No;
        public string IsTransOrphan { get; set; } = Resx.AppResources.Any;
        public string IsUnapproved { get; set; } = Resx.AppResources.No;
        public string IsTransUnapproved { get; set; } = Resx.AppResources.Any;
        public string HasAudio { get; set; } = Resx.AppResources.Any;
        public string TransHasAudio { get; set; } = Resx.AppResources.Any;

        public IEnumerable<string> NullableBoolValues => new List<string> { Resx.AppResources.Any, Resx.AppResources.No, Resx.AppResources.Yes };

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

        public SearchResults SearchResults;

        public async Task<bool> ExecuteSearchCommand()
        {
            if (IsBusy)
            {
                return false;
            }

            IsBusy = true;

            var response = await MainService.SearchAsync(new SearchRequest
            {
                Page =  1,
                Text = SearchText,
                IsoFrom = SelectedLanguageSource.Iso,
                IsoTo = SelectedLanguageTarget.Iso,
                IsOrphan = NullableBooleanStringToNullableBool(IsOrphan),
                IsUnapproved = NullableBooleanStringToNullableBool(IsUnapproved),
                HasAudio = NullableBooleanStringToNullableBool(HasAudio),
                IsTransOrphan = NullableBooleanStringToNullableBool(IsTransOrphan),
                IsTransUnapproved = NullableBooleanStringToNullableBool(IsTransUnapproved),
                TransHasAudio = NullableBooleanStringToNullableBool(TransHasAudio)
            });

            if (response.Status != TatoebaStatus.Success)
            {
                OnError(response);
                IsBusy = false;
                return false;
            }

            IsBusy = false;

            SearchResults = response.Content;

            return true;
        }


        bool? NullableBooleanStringToNullableBool(string item) => item == Resx.AppResources.Any ? null : (bool?)(item == Resx.AppResources.Yes ? true : false);

    }
}