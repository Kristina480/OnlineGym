using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Domain;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class ExerciseManagementWindow : Window
{
    private ExerciseManagementViewModel? _viewModel;

    public ExerciseManagementWindow()
    {
        InitializeComponent();
    }

    public ExerciseManagementWindow(
        long trainerId) : this()
    {
        _viewModel =
            new ExerciseManagementViewModel(
                trainerId);

        LoadAvailableEquipmentAndMachines();
        LoadExercises(showEmptyMessage: true);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadAvailableEquipmentAndMachines()
    {
        try
        {
            if (_viewModel is null)
            {
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");
            }

            ComboBox equipmentComboBox =
                GetControl<ComboBox>(
                    "EquipmentComboBox");

            ComboBox machineComboBox =
                GetControl<ComboBox>(
                    "MachineComboBox");

            List<Equipment> equipment =
                _viewModel.GetEquipment();

            List<Machine> machines =
                _viewModel.GetMachines();

            equipmentComboBox.ItemsSource =
                equipment;

            machineComboBox.ItemsSource =
                machines;

            // Nijedan rekvizit ili sprava nisu obavezni.
            equipmentComboBox.SelectedIndex = -1;
            machineComboBox.SelectedIndex = -1;
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private bool LoadExercises(
        bool showEmptyMessage)
    {
        DataGrid grid =
            GetControl<DataGrid>(
                "ExercisesDataGrid");

        try
        {
            if (_viewModel is null)
            {
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");
            }

            List<Exercise> exercises =
                _viewModel.GetExercises();

            grid.ItemsSource = exercises;
            grid.SelectedItem = null;

            GetControl<Button>(
                "DeleteExerciseButton")
                .IsEnabled =
                exercises.Count > 0;

            if (showEmptyMessage)
            {
                if (exercises.Count == 0)
                {
                    ShowInfo(
                        "Još nemate kreirane vežbe.");
                }
                else
                {
                    ClearMessage();
                }
            }

            return true;
        }
        catch (Exception exception)
        {
            grid.ItemsSource = null;

            GetControl<Button>(
                "DeleteExerciseButton")
                .IsEnabled = false;

            ShowError(exception);

            return false;
        }
    }

    private void OnCreateClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_viewModel is null)
        {
            ShowError(
                "Prozor nije pravilno inicijalizovan.");

            return;
        }

        TextBox nameBox =
            GetControl<TextBox>(
                "NameTextBox");

        TextBox videoBox =
            GetControl<TextBox>(
                "VideoUrlTextBox");

        ComboBox equipmentComboBox =
            GetControl<ComboBox>(
                "EquipmentComboBox");

        ComboBox machineComboBox =
            GetControl<ComboBox>(
                "MachineComboBox");

        string name =
            nameBox.Text?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError(
                "Naziv vežbe je obavezan.");

            return;
        }

        Equipment? selectedEquipment =
            equipmentComboBox.SelectedItem
                as Equipment;

        Machine? selectedMachine =
            machineComboBox.SelectedItem
                as Machine;

        long? equipmentId =
            selectedEquipment?.Id;

        long? machineId =
            selectedMachine?.Id;

        Button? button =
            sender as Button;

        try
        {
            if (button is not null)
            {
                button.IsEnabled = false;
            }

            _viewModel.CreateExercise(
                name,
                string.IsNullOrWhiteSpace(
                    videoBox.Text)
                    ? null
                    : videoBox.Text.Trim(),
                equipmentId,
                machineId);

            nameBox.Text = string.Empty;
            videoBox.Text = string.Empty;

            equipmentComboBox.SelectedIndex = -1;
            machineComboBox.SelectedIndex = -1;

            LoadExercises(
                showEmptyMessage: false);

            ShowSuccess(
                "Vežba je uspešno kreirana.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            if (button is not null)
            {
                button.IsEnabled = true;
            }
        }
    }

    private void OnDeleteClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_viewModel is null)
        {
            ShowError(
                "Prozor nije pravilno inicijalizovan.");

            return;
        }

        DataGrid grid =
            GetControl<DataGrid>(
                "ExercisesDataGrid");

        if (grid.SelectedItem
            is not Exercise exercise)
        {
            ShowError(
                "Izaberite vežbu za brisanje.");

            return;
        }

        Button? button =
            sender as Button;

        try
        {
            if (button is not null)
            {
                button.IsEnabled = false;
            }

            _viewModel.DeleteExercise(
                exercise.Id);

            LoadExercises(
                showEmptyMessage: false);

            ShowSuccess(
                "Vežba je obrisana.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            if (button is not null)
            {
                button.IsEnabled = true;
            }
        }
    }

    private void OnCloseClick(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private T GetControl<T>(
        string name)
        where T : Control
    {
        return this.FindControl<T>(name)
            ?? throw new InvalidOperationException(
                $"Kontrola '{name}' nije pronađena u XAML-u.");
    }

    private void ShowSuccess(
        string text)
    {
        TextBlock message =
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground =
            Brushes.DarkGreen;
    }

    private void ShowInfo(
        string text)
    {
        TextBlock message =
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground =
            Brushes.DimGray;
    }

    private void ShowError(
        string text)
    {
        TextBlock message =
            GetControl<TextBlock>(
                "MessageTextBlock");

        message.Text = text;
        message.Foreground =
            Brushes.DarkRed;
    }

    private void ShowError(
        Exception exception)
    {
        Console.Error.WriteLine(
            exception);

        if (exception
            is InvalidOperationException
            or ArgumentException)
        {
            ShowError(
                exception.Message);
        }
        else
        {
            ShowError(
                "Došlo je do greške pri radu sa vežbama.");
        }
    }

    private void ClearMessage()
    {
        GetControl<TextBlock>(
            "MessageTextBlock")
            .Text = string.Empty;
    }
}