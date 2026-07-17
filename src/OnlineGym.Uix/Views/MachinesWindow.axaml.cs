using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;

namespace OnlineGym.Uix.Views;

public partial class MachinesWindow : Window
{
    private readonly MachineRepository _machineRepository = new();

    private readonly ObservableCollection<Machine> _machines = new();

    private ListBox? _machinesListBox;
    private TextBox? _machineNameTextBox;
    private TextBlock? _statusTextBlock;

    private Machine? _selectedMachine;

    public MachinesWindow()
    {
        InitializeComponent();

        _machinesListBox = this.FindControl<ListBox>("MachinesListBox");
        _machineNameTextBox = this.FindControl<TextBox>("MachineNameTextBox");
        _statusTextBlock = this.FindControl<TextBlock>("StatusTextBlock");

        if (_machinesListBox is not null)
        {
            _machinesListBox.ItemsSource = _machines;
        }

        LoadMachines();
    }

    private void LoadMachines()
    {
        try
        {
            _machines.Clear();

            foreach (Machine machine in _machineRepository.GetAll())
            {
                _machines.Add(machine);
            }

            ShowStatus(
                $"Učitano sprava: {_machines.Count}.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri učitavanju sprava: {exception.Message}",
                isError: true
            );
        }
    }

    private void OnMachineSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (_machinesListBox?.SelectedItem is not Machine machine)
        {
            _selectedMachine = null;
            return;
        }

        _selectedMachine = machine;

        if (_machineNameTextBox is not null)
        {
            _machineNameTextBox.Text = machine.Name;
        }

        ShowStatus(
            $"Izabrana sprava: {machine.Name}.",
            isError: false
        );
    }

    private void OnAddMachineClick(
        object? sender,
        RoutedEventArgs e)
    {
        string? name = GetValidatedName();

        if (name is null)
        {
            return;
        }

        try
        {
            Machine machine = new(
                id: 0,
                name: name
            );

            long generatedId = _machineRepository.Insert(machine);
            machine.Id = generatedId;

            _machines.Add(machine);

            ClearForm();

            ShowStatus(
                "Sprava je uspešno dodata.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri dodavanju sprave: {GetFriendlyError(exception)}",
                isError: true
            );
        }
    }

    private void OnUpdateMachineClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_selectedMachine is null)
        {
            ShowStatus(
                "Prvo izaberite spravu koju želite da izmenite.",
                isError: true
            );

            return;
        }

        string? name = GetValidatedName();

        if (name is null)
        {
            return;
        }

        try
        {
            _selectedMachine.Name = name;

            _machineRepository.Update(_selectedMachine);
            
            LoadMachines();
            ClearForm();

            ShowStatus(
                "Sprava je uspešno izmenjena.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri izmeni sprave: {GetFriendlyError(exception)}",
                isError: true
            );
        }
    }

    private void OnDeleteMachineClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_selectedMachine is null)
        {
            ShowStatus(
                "Prvo izaberite spravu koju želite da obrišete.",
                isError: true
            );

            return;
        }

        try
        {
            long machineId = _selectedMachine.Id;
            string machineName = _selectedMachine.Name;

            _machineRepository.Delete(machineId);
            _machines.Remove(_selectedMachine);

            ClearForm();

            ShowStatus(
                $"Sprava „{machineName}” je uspešno obrisana.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri brisanju sprave: {GetFriendlyError(exception)}",
                isError: true
            );
        }
    }

    private void OnClearSelectionClick(
        object? sender,
        RoutedEventArgs e)
    {
        ClearForm();

        ShowStatus(
            "Izbor je poništen.",
            isError: false
        );
    }

    private void OnCloseClick(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private string? GetValidatedName()
    {
        string name = _machineNameTextBox?.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowStatus(
                "Naziv sprave je obavezan.",
                isError: true
            );

            return null;
        }

        if (name.Length > 255)
        {
            ShowStatus(
                "Naziv sprave ne sme imati više od 255 karaktera.",
                isError: true
            );

            return null;
        }

        return name;
    }

    private void ClearForm()
    {
        _selectedMachine = null;

        if (_machinesListBox is not null)
        {
            _machinesListBox.SelectedItem = null;
        }

        if (_machineNameTextBox is not null)
        {
            _machineNameTextBox.Text = string.Empty;
        }
    }

    private void ShowStatus(
        string message,
        bool isError)
    {
        if (_statusTextBlock is null)
        {
            return;
        }

        _statusTextBlock.Text = message;
        _statusTextBlock.Foreground = isError
            ? Brushes.DarkRed
            : Brushes.DarkGreen;
    }

    private static string GetFriendlyError(Exception exception)
    {
        string message = exception.Message;

        if (message.Contains(
                "duplicate key",
                StringComparison.OrdinalIgnoreCase) ||
            message.Contains(
                "unique constraint",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Već postoji sprava sa tim nazivom.";
        }

        return message;
    }
}