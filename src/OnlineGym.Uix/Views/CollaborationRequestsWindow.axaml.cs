using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Domain;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class CollaborationRequestsWindow : Window
{
    private CollaborationRequestsViewModel? _viewModel;

    public CollaborationRequestsWindow()
    {
        InitializeComponent();
    }

    public CollaborationRequestsWindow(long trainerId) : this()
    {
        _viewModel =
            new CollaborationRequestsViewModel(trainerId);

        LoadData(showEmptyMessage: true);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private bool LoadData(bool showEmptyMessage)
    {
        DataGrid requestsGrid =
            GetControl<DataGrid>("RequestsDataGrid");

        ComboBox packagesBox =
            GetControl<ComboBox>("PricingPackageComboBox");

        Button approveButton =
            GetControl<Button>("ApproveButton");

        Button rejectButton =
            GetControl<Button>("RejectButton");

        try
        {
            if (_viewModel is null)
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");

            List<CollaborationRequest> requests =
                _viewModel.GetPendingRequests();

            List<PricingPackage> packages =
                _viewModel.GetPricingPackages();

            requestsGrid.ItemsSource = requests;
            requestsGrid.SelectedItem = null;

            packagesBox.ItemsSource = packages;
            packagesBox.SelectedIndex =
                packages.Count > 0 ? 0 : -1;

            approveButton.IsEnabled =
                requests.Count > 0 && packages.Count > 0;

            rejectButton.IsEnabled =
                requests.Count > 0;

            if (!showEmptyMessage)
                return true;

            if (requests.Count == 0)
            {
                ShowInfo("Nema zahteva na čekanju.");
            }
            else if (packages.Count == 0)
            {
                ShowError(
                    "Nemate nijedan paket cena. Zahtev ne može biti prihvaćen dok ne postoji paket.");
            }
            else
            {
                ClearMessage();
            }

            return true;
        }
        catch (Exception exception)
        {
            requestsGrid.ItemsSource = null;
            packagesBox.ItemsSource = null;
            approveButton.IsEnabled = false;
            rejectButton.IsEnabled = false;
            ShowError(exception);
            return false;
        }
    }

    private void OnApproveClick(
        object? sender,
        RoutedEventArgs e)
    {
        DataGrid requestsGrid =
            GetControl<DataGrid>("RequestsDataGrid");

        ComboBox packagesBox =
            GetControl<ComboBox>("PricingPackageComboBox");

        if (_viewModel is null)
        {
            ShowError("Prozor nije pravilno inicijalizovan.");
            return;
        }

        if (requestsGrid.SelectedItem
            is not CollaborationRequest request)
        {
            ShowError("Izaberite zahtev.");
            return;
        }

        if (packagesBox.SelectedItem
            is not PricingPackage pricingPackage)
        {
            ShowError("Izaberite paket cena.");
            return;
        }

        Button? button = sender as Button;

        try
        {
            if (button is not null)
                button.IsEnabled = false;

            _viewModel.ApproveRequest(
                request.Id,
                pricingPackage.Id);

            LoadData(showEmptyMessage: false);
            ShowSuccess(
                "Zahtev je prihvaćen i saradnja je kreirana.");
        }
        catch (Exception exception)
        {
            // Osvežavanje pokazuje pravo stanje čak i ako je
            // baza završila deo operacije pre greške.
            LoadData(showEmptyMessage: false);
            ShowError(exception);
        }
        finally
        {
            UpdateActionButtons();
        }
    }

    private void OnRejectClick(
        object? sender,
        RoutedEventArgs e)
    {
        DataGrid requestsGrid =
            GetControl<DataGrid>("RequestsDataGrid");

        if (_viewModel is null)
        {
            ShowError("Prozor nije pravilno inicijalizovan.");
            return;
        }

        if (requestsGrid.SelectedItem
            is not CollaborationRequest request)
        {
            ShowError("Izaberite zahtev.");
            return;
        }

        Button? button = sender as Button;

        try
        {
            if (button is not null)
                button.IsEnabled = false;

            _viewModel.RejectRequest(request.Id);

            LoadData(showEmptyMessage: false);
            ShowSuccess("Zahtev je odbijen.");
        }
        catch (Exception exception)
        {
            LoadData(showEmptyMessage: false);
            ShowError(exception);
        }
        finally
        {
            UpdateActionButtons();
        }
    }

    private void UpdateActionButtons()
    {
        DataGrid grid = GetControl<DataGrid>("RequestsDataGrid");
        ComboBox packages =
            GetControl<ComboBox>("PricingPackageComboBox");

        bool hasRequests =
            grid.ItemsSource is IEnumerable<CollaborationRequest>
            requestItems &&
            requestItems.GetEnumerator().MoveNext();

        bool hasPackages =
            packages.ItemsSource is IEnumerable<PricingPackage>
            packageItems &&
            packageItems.GetEnumerator().MoveNext();

        GetControl<Button>("ApproveButton").IsEnabled =
            hasRequests && hasPackages;

        GetControl<Button>("RejectButton").IsEnabled =
            hasRequests;
    }

    private void OnCloseClick(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private T GetControl<T>(string name) where T : Control
    {
        return this.FindControl<T>(name)
            ?? throw new InvalidOperationException(
                $"Kontrola '{name}' nije pronađena u XAML-u.");
    }

    private void ShowSuccess(string text)
    {
        TextBlock message =
            GetControl<TextBlock>("MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DarkGreen;
    }

    private void ShowInfo(string text)
    {
        TextBlock message =
            GetControl<TextBlock>("MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DimGray;
    }

    private void ShowError(string text)
    {
        TextBlock message =
            GetControl<TextBlock>("MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DarkRed;
    }

    private void ShowError(Exception exception)
    {
        Console.Error.WriteLine(exception);

        if (exception is InvalidOperationException
            or ArgumentException)
        {
            ShowError(exception.Message);
        }
        else
        {
            ShowError(
                "Došlo je do greške prilikom obrade zahteva.");
        }
    }

    private void ClearMessage()
    {
        GetControl<TextBlock>(
            "MessageTextBlock").Text = string.Empty;
    }
}
