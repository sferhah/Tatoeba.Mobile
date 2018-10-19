using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Views
{
    // Android: If there are too many Items, Xamrin.Forms Picker causes a GC overhead and then crashes.
    public class PagePicker : Button
    {
        public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create("ItemsSource", typeof(IEnumerable<int>), typeof(PagePicker), null, propertyChanged: OnItemsSourceChanged);

        public IEnumerable<int> ItemsSource
        {
            get { return (IEnumerable<int>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is PagePicker myself)
                || !(newValue is IEnumerable<int> items))
                return;

            myself.listView.ItemsSource = items;
            myself.UpdateText();
        }
        

        public static readonly BindableProperty SelectedItemProperty =
              BindableProperty.Create("SelectedItem", typeof(int), typeof(PagePicker), default(int), BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);


        public int SelectedItem
        {
            get { return (int)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is PagePicker myself)
                || !(newValue is int item))
                return;

            myself.UpdateText();
            myself.listView.SelectedItem = item;

        }         


        public ListView listView;

        public PagePicker()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BorderColor = Color.Gray;
            BackgroundColor = Color.White;
            UpdateText();
            CreatePopup();
            Clicked += (s,e) => Navigation.PushPopupAsync(popup);
        }

        public void UpdateText() => Text = "Page: " + SelectedItem + "/" + ItemsSource?.Count();

        Rg.Plugins.Popup.Pages.PopupPage popup;
        public void CreatePopup()
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


            listView = new ListView();
            listView.ItemTapped += ListView_ItemTapped;
            stackLayout2.Children.Add(listView);


            frame.Content = stackLayout2;
            stackLayout.Children.Add(frame);

            popup = new Rg.Plugins.Popup.Pages.PopupPage
            {
                Content = stackLayout,
                WidthRequest = 50,
            };

            popup.Disappearing += (s,e) => UpdateText(); 
            popup.Appearing += Popup_Appearing;            
        }

        private async void Popup_Appearing(object sender, EventArgs e)
        {
            await Task.Delay(300); // Hack in order to force scrolling.
            listView.ScrollTo(SelectedItem, ScrollToPosition.Center, false);
        }   

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            SelectedItem = (int)e.Item;

            UpdateText();

            await PopupNavigation.PopAsync();
        }

    }

}
