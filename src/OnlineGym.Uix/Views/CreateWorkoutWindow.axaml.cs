using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Domain;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class CreateWorkoutWindow : Window
{
    private long _trainerId;
    private CreateWorkoutViewModel? _viewModel;
    private readonly ObservableCollection<WorkoutItem>
        _items = new();

    public CreateWorkoutWindow()
    {
        InitializeComponent();

        GetControl<DatePicker>(
            "WorkoutDatePicker").SelectedDate =
            DateTimeOffset.Now.AddDays(1);

        GetControl<DataGrid>(
            "WorkoutItemsDataGrid").ItemsSource =
            _items;
    }

    public CreateWorkoutWindow(long trainerId) : this()
    {
        _trainerId = trainerId;
        _viewModel =
            new CreateWorkoutViewModel(trainerId);

        LoadInitialData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadInitialData()
    {
        bool clientsLoaded = LoadClients();
        bool exercisesLoaded = LoadExercises();

        GetControl<Button>(
            "SaveWorkoutButton").IsEnabled =
            clientsLoaded &&
            GetControl<ComboBox>(
                "ClientComboBox").ItemsSource is not null;

        GetControl<Button>(
            "AddWorkoutItemButton").IsEnabled =
            exercisesLoaded &&
            GetControl<ComboBox>(
                "ExerciseComboBox").ItemsSource is not null;

        if (clientsLoaded && exercisesLoaded)
            ClearMessage();
    }

    private bool LoadClients()
    {
        ComboBox clientBox =
            GetControl<ComboBox>("ClientComboBox");

        try
        {
            if (_viewModel is null)
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");

            List<Client> clients =
                _viewModel.GetActiveClients();

            clientBox.ItemsSource = clients;
            clientBox.SelectedIndex =
                clients.Count > 0 ? 0 : -1;

            if (clients.Count == 0)
            {
                ShowInfo(
                    "Nema aktivnih klijenata za koje može da se kreira trening.");
            }

            return clients.Count > 0;
        }
        catch (Exception exception)
        {
            clientBox.ItemsSource = null;
            ShowError(exception);
            return false;
        }
    }

    private bool LoadExercises()
    {
        ComboBox exerciseBox =
            GetControl<ComboBox>("ExerciseComboBox");

        try
        {
            if (_viewModel is null)
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");

            List<Exercise> exercises =
                _viewModel.GetExercises();

            exerciseBox.ItemsSource = exercises;
            exerciseBox.SelectedIndex =
                exercises.Count > 0 ? 0 : -1;

            if (exercises.Count == 0)
            {
                ShowInfo(
                    "Kreirajte bar jednu vežbu pre dodavanja stavki treninga.");
            }

            GetControl<Button>(
                "AddWorkoutItemButton").IsEnabled =
                exercises.Count > 0;

            return exercises.Count > 0;
        }
        catch (Exception exception)
        {
            exerciseBox.ItemsSource = null;
            GetControl<Button>(
                "AddWorkoutItemButton").IsEnabled = false;
            ShowError(exception);
            return false;
        }
    }

    private void OnClientSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        ComboBox clientBox =
            GetControl<ComboBox>("ClientComboBox");

        TextBlock goalText =
            GetControl<TextBlock>(
                "ClientGoalTextBlock");

        TextBlock healthText =
            GetControl<TextBlock>(
                "ClientHealthIssuesTextBlock");

        if (clientBox.SelectedItem is not Client client)
        {
            goalText.Text = "Izaberite klijenta";
            healthText.Text = "Izaberite klijenta";
            return;
        }

        goalText.Text =
            string.IsNullOrWhiteSpace(client.Goal)
                ? "Cilj nije unet."
                : client.Goal;

        healthText.Text =
            string.IsNullOrWhiteSpace(
                client.HealthIssues)
                ? "Nema unetih zdravstvenih problema."
                : client.HealthIssues;
    }

    private void OnAddItemClick(
        object? sender,
        RoutedEventArgs e)
    {
        ComboBox exerciseBox =
            GetControl<ComboBox>("ExerciseComboBox");

        TextBox repetitionsBox =
            GetControl<TextBox>(
                "RepetitionCountTextBox");

        TextBox commentBox =
            GetControl<TextBox>(
                "ItemCommentTextBox");

        if (exerciseBox.SelectedItem
            is not Exercise exercise)
        {
            ShowError("Izaberite vežbu.");
            return;
        }

        if (_items.Any(
                item =>
                    item.ExerciseId == exercise.Id))
        {
            ShowError(
                "Ova vežba je već dodata u trening.");
            return;
        }

        int? repetitions = null;

        if (!string.IsNullOrWhiteSpace(
                repetitionsBox.Text))
        {
            if (!int.TryParse(
                    repetitionsBox.Text,
                    out int parsed) ||
                parsed <= 0)
            {
                ShowError(
                    "Broj ponavljanja mora biti pozitivan ceo broj.");
                return;
            }

            repetitions = parsed;
        }

        WorkoutItem item = new WorkoutItem(
            0,
            0,
            exercise.Id,
            repetitions,
            false,
            null,
            string.IsNullOrWhiteSpace(
                commentBox.Text)
                ? null
                : commentBox.Text.Trim());

        _items.Add(item);

        repetitionsBox.Text = string.Empty;
        commentBox.Text = string.Empty;

        ShowSuccess("Stavka je dodata u trening.");
    }

    private void OnRemoveItemClick(
        object? sender,
        RoutedEventArgs e)
    {
        DataGrid itemsGrid =
            GetControl<DataGrid>(
                "WorkoutItemsDataGrid");

        if (itemsGrid.SelectedItem
            is not WorkoutItem item)
        {
            ShowError(
                "Izaberite stavku za uklanjanje.");
            return;
        }

        _items.Remove(item);
        ShowInfo("Stavka je uklonjena.");
    }

    private void OnManageExercisesClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_trainerId <= 0)
        {
            ShowError(
                "Prozor nije pravilno inicijalizovan.");
            return;
        }

        ExerciseManagementWindow window =
            new ExerciseManagementWindow(_trainerId);

        Hide();

        window.Closed += (_, _) =>
        {
            LoadExercises();
            Show();
            Activate();
        };

        window.Show();
    }

    private void OnSaveWorkoutClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_viewModel is null)
        {
            ShowError(
                "Prozor nije pravilno inicijalizovan.");
            return;
        }

        ComboBox clientBox =
            GetControl<ComboBox>("ClientComboBox");

        DatePicker datePicker =
            GetControl<DatePicker>(
                "WorkoutDatePicker");

        TextBox timeBox =
            GetControl<TextBox>(
                "WorkoutTimeTextBox");

        TextBox commentBox =
            GetControl<TextBox>(
                "WorkoutCommentTextBox");

        if (clientBox.SelectedItem
            is not Client client)
        {
            ShowError("Izaberite klijenta.");
            return;
        }

        if (datePicker.SelectedDate is null)
        {
            ShowError("Izaberite datum treninga.");
            return;
        }

        if (!TimeSpan.TryParseExact(
                timeBox.Text?.Trim(),
                @"hh\:mm",
                CultureInfo.InvariantCulture,
                out TimeSpan time))
        {
            ShowError(
                "Vreme unesite u formatu HH:mm, na primer 18:00.");
            return;
        }

        if (_items.Count == 0)
        {
            ShowError(
                "Dodajte najmanje jednu vežbu u trening.");
            return;
        }

        DateTime scheduledAt =
            datePicker.SelectedDate.Value.Date.Add(time);

        if (scheduledAt <= DateTime.Now)
        {
            ShowError(
                "Datum i vreme treninga moraju biti u budućnosti.");
            return;
        }

        Button? button = sender as Button;

        try
        {
            if (button is not null)
                button.IsEnabled = false;

            long workoutId =
                _viewModel.CreateWorkout(
                    client.ClientId,
                    scheduledAt,
                    string.IsNullOrWhiteSpace(
                        commentBox.Text)
                        ? null
                        : commentBox.Text.Trim(),
                    _items.ToList());

            _items.Clear();
            commentBox.Text = string.Empty;

            ShowSuccess(
                $"Trening je uspešno kreiran. ID: {workoutId}.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            if (button is not null)
                button.IsEnabled = true;
        }
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
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DarkGreen;
    }

    private void ShowInfo(string text)
    {
        TextBlock message =
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DimGray;
    }

    private void ShowError(string text)
    {
        TextBlock message =
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DarkRed;
    }

    private void ClearMessage()
    {
        GetControl<TextBlock>(
            "MessageTextBlock").Text = string.Empty;
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
                "Došlo je do greške pri kreiranju treninga.");
        }
    }
}
