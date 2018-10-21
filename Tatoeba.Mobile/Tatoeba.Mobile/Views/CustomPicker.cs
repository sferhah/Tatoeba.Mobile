using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Views
{
    public class LanguagePicker : CustomPicker<string>
    {
        // public override void UpdateText() => Text = "Language: " + SelectedItem;
    }

    public class PagePicker : CustomPicker<int>
    {
        public override void UpdateText() => Text = "Page: " + SelectedItem + "/" + ItemsSource?.Count();
    }

    public class CustomPicker : CustomPicker<object> {}

    // Android: If there are too many Items, Xamarin.Forms Picker causes a GC overhead and then crashes.
    public class CustomPicker<T> : Button
    {
        public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create("ItemsSource", typeof(IEnumerable<T>), typeof(CustomPicker<T>), null, propertyChanged: OnItemsSourceChanged);

        public IEnumerable<T> ItemsSource
        {
            get { return (IEnumerable<T>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is CustomPicker<T> myself)
                || !(newValue is IEnumerable<T> items))
                return;
          
            myself.internalItemsSource = items;
            myself.UpdateText();
        }
        

        public static readonly BindableProperty SelectedItemProperty =
              BindableProperty.Create("SelectedItem", typeof(T), typeof(CustomPicker<T>), default(T), BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);


        public T SelectedItem
        {
            get { return (T)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is CustomPicker<T> myself)
                || !(newValue is T item))
                return;

            myself.UpdateText();

            myself.internalSelectedItem = item;

            if (myself.listView != null)
            {
                myself.listView.SelectedItem = item;
            }
        }

        IEnumerable<T> internalItemsSource;
        object internalSelectedItem;

        public ListView listView;
        SearchBar searchBox;

        public CustomPicker()
        {
            BorderColor = Color.Gray;
            BackgroundColor = Color.Transparent;
            BorderWidth = 1;
            CornerRadius = 2;

            Padding = new Thickness(20, 5, 20, 5);                

            UpdateText();            
            Clicked += (s, e) => Navigation.PushPopupAsync(CreatePopup());
        }

        virtual public void UpdateText() => Text = Convert.ToString(SelectedItem);

        public Rg.Plugins.Popup.Pages.PopupPage CreatePopup()
        {
            StackLayout stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(20, 20, 20, 20),                
                HorizontalOptions = LayoutOptions.Center,
            };

            Frame frame = new Frame
            {
                CornerRadius = 6,
                BackgroundColor = Color.White,
                HasShadow = true
            };

            StackLayout stackLayout2 = new StackLayout();

            Button close = new Button
            {
                Text = "❌",
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Color.Transparent,
            };

            close.Clicked += (s,e) => PopupNavigation.PopAsync();

            stackLayout2.Children.Add(close);

            if(internalItemsSource.Count() > 3)
            {
                searchBox = new SearchBar();
                searchBox.TextChanged += SearchBar_TextChanged;
                stackLayout2.Children.Add(searchBox);
            }

            listView = new ListView();
            listView.WidthRequest = 300;
            listView.HeightRequest = 300;
            listView.ItemTemplate = new DataTemplate(() => new PickerCell());
            listView.ItemsSource = internalItemsSource;
            listView.SelectedItem = internalSelectedItem;
            listView.ItemTapped += ListView_ItemTapped;
            
            stackLayout2.Children.Add(listView);


            frame.Content = stackLayout2;
            stackLayout.Children.Add(frame);

            var popup = new Rg.Plugins.Popup.Pages.PopupPage
            {
                Content = stackLayout,
                WidthRequest = 50,
            };

            popup.Disappearing += Popup_Disappearing; 
            popup.Appearing += Popup_Appearing;

            return popup;
        }


        

        private async void Popup_Appearing(object sender, EventArgs e)
        {
            // Hack in order to force scrolling.
            while (true)
            {
                await Task.Delay(10);

                if(listView.Height > 0)
                {            
                    break;
                }
            }            

            listView.ScrollTo(SelectedItem, ScrollToPosition.Center, false);            
        }

        private void Popup_Disappearing(object sender, EventArgs e)
        {
            if(searchBox != null)
            {
                searchBox.Text = null;
            }
            
            UpdateText();
        }


        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(searchBox?.Text))
            {
                listView.ItemsSource = ItemsSource;
            }
            else
            {
                listView.ItemsSource = ItemsSource
                .Where(x => CultureInfo.InvariantCulture.CompareInfo.IndexOf(Convert.ToString(x), searchBox.Text, CompareOptions.IgnoreCase) >= 0)
                .ToList();
            }            
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            SelectedItem = (T)e.Item;

            UpdateText();

            await PopupNavigation.PopAsync();
        }

    }

}
