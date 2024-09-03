using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Intrinsics.Arm;
using CoreLibrary.Toolkit.WinUI.Contracts;
using CoreLibrary.Toolkit.WinUI.Structs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreLibrary.Toolkit.WinUI.Controls
{
    public sealed partial class CustomNavigationView : UserControl
    {
        #region Properties
        public static DependencyProperty OpenPaneLengthProperty { get; } = NavigationView.OpenPaneLengthProperty;

        public ICollection<IPageItem>? HeaderPageItems
        {
            get => (ICollection<IPageItem>?)baseNavigationView.GetValue(NavigationView.MenuItemsSourceProperty);
            set => baseNavigationView.SetValue(NavigationView.MenuItemsSourceProperty, value);
        }

        public ICollection<IPageItem>? FooterPageItems
        {
            get => (ICollection<IPageItem>?)baseNavigationView.GetValue(NavigationView.FooterMenuItemsSourceProperty);
            set => baseNavigationView.SetValue(NavigationView.FooterMenuItemsSourceProperty, value);
        }

        public double OpenPaneLength
        {
            get => (double)baseNavigationView.GetValue(OpenPaneLengthProperty);
            set => baseNavigationView.SetValue(OpenPaneLengthProperty, value);
        }

        public NavigationView BaseNavigationView => baseNavigationView;
        public Frame ContentFrame => contentFrame;
        public PageItem? SelectedItem => (PageItem?)baseNavigationView.SelectedItem;

        #endregion


        public CustomNavigationView()
        {
            this.InitializeComponent();
        }

        private Type? CurrentPageType => contentFrame.Content?.GetType();

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            baseNavigationView.SelectedItem = PageTypeToPageItem(e.SourcePageType);
            baseNavigationView.IsBackEnabled = contentFrame.CanGoBack;
            UpdateCanExecuteCommand();
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            contentFrame.GoBack();
        }

        private async void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var target = (baseNavigationView.MenuItemFromContainer(args.InvokedItemContainer) as IPageItem)!;
            switch (target)
            {
                case PageItem pi:
                    PageNavigate(pi);
                    break;
                case CommandItem ci:
                    InvokeCommand(ci);
                    BackToContentPage();
                    break;
                case AsyncCommandItem aci:
                    await InvokeAsyncCommand(aci);
                    BackToContentPage();
                    break;
                default:
                    break;
            }
        }

        private void InvokeCommand(CommandItem target)
        {
            target.Execute();
        }

        private async Task InvokeAsyncCommand(AsyncCommandItem target)
        {
            await target.ExecuteAsync();
        }

        private void UpdateCanExecuteCommand()
        {
            if (HeaderPageItems is not null)
            {
                foreach (var item in HeaderPageItems)
                {
                    if (item is CommandItem ci)
                    {
                        ci.NotifyCanExecuteChanged();
                    }
                    else if (item is AsyncCommandItem aci)
                    {
                        aci.NotifyCanExecuteChanged();
                    }
                }
            }
            if (FooterPageItems is not null)
            {
                foreach (var item in FooterPageItems)
                {
                    if (item is CommandItem ci)
                    {
                        ci.NotifyCanExecuteChanged();
                    }
                    else if (item is AsyncCommandItem aci)
                    {
                        aci.NotifyCanExecuteChanged();
                    }
                }
            }
        }

        private void PageNavigate(PageItem target)
        {
            if (CurrentPageType is null || CurrentPageType != target.PageType)
            {
                if (
                    CurrentPageType is not null
                    && PageTypeToPageItem(CurrentPageType) is PageItem pageItem
                    && pageItem.TransitionSelector is not null
                    && pageItem.TransitionSelector.GetTransition(target.PageType) is NavigationTransitionInfo tran
                )
                {
                    contentFrame.Navigate(target.PageType, null, tran);
                }
                else
                {
                    contentFrame.Navigate(target.PageType);
                }
            }
        }

        private void BackToContentPage()
        {
            if (contentFrame.Content is null)
                return;
            baseNavigationView.SelectedItem = PageTypeToPageItem(contentFrame.Content.GetType());
        }

        private PageItem? PageTypeToPageItem(Type type)
        {
            if (
                HeaderPageItems is not null
                && HeaderPageItems.FirstOrDefault(p => (p is PageItem pi && pi.PageType == type)) is PageItem hpi
            )
            {
                return hpi;
            }
            if (
                FooterPageItems is not null
                && FooterPageItems.FirstOrDefault(p => (p is PageItem pi && pi.PageType == type)) is PageItem fpi
            )
            {
                return fpi;
            }
            return null;
        }
    }
}
