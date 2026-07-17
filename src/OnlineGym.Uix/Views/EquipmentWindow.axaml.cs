using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using Avalonia.Markup.Xaml;

namespace OnlineGym.Uix.Views;

public partial class EquipmentWindow : Window
{
    private readonly EquipmentRepository _equipmentRepository =
        new();

    private readonly ObservableCollection<Equipment> _equipment =
        new();

    private ListBox? _equipmentListBox;
    private TextBox? _equipmentNameTextBox;
    private TextBlock? _statusTextBlock;

    private Equipment? _selectedEquipment;

    public EquipmentWindow()
    {
        InitializeComponent();

        _equipmentListBox =
            this.FindControl<ListBox>("EquipmentListBox");

        _equipmentNameTextBox =
            this.FindControl<TextBox>("EquipmentNameTextBox");

        _statusTextBlock =
            this.FindControl<TextBlock>("StatusTextBlock");

        if (_equipmentListBox is not null)
        {
            _equipmentListBox.ItemsSource = _equipment;
        }

        LoadEquipment();
    }

    private void LoadEquipment()
    {
        try
        {
            _equipment.Clear();

            foreach (
                Equipment equipment
                in _equipmentRepository.GetAll())
            {
                _equipment.Add(equipment);
            }

            ShowStatus(
                $"Učitano rekvizita: {_equipment.Count}.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri učitavanju rekvizita: " +
                exception.Message,
                isError: true
            );
        }
    }

    private void OnEquipmentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (_equipmentListBox?.SelectedItem
            is not Equipment equipment)
        {
            _selectedEquipment = null;
            return;
        }

        _selectedEquipment = equipment;

        if (_equipmentNameTextBox is not null)
        {
            _equipmentNameTextBox.Text =
                equipment.Name;
        }

        ShowStatus(
            $"Izabran rekvizit: {equipment.Name}.",
            isError: false
        );
    }

    private void OnAddEquipmentClick(
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
            Equipment equipment = new(
                id: 0,
                name: name
            );

            long generatedId =
                _equipmentRepository.Insert(equipment);

            equipment.Id = generatedId;

            _equipment.Add(equipment);

            ClearForm();

            ShowStatus(
                "Rekvizit je uspešno dodat.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri dodavanju rekvizita: " +
                GetFriendlyError(exception),
                isError: true
            );
        }
    }

    private void OnUpdateEquipmentClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_selectedEquipment is null)
        {
            ShowStatus(
                "Prvo izaberite rekvizit koji želite da izmenite.",
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
            _selectedEquipment.Name = name;

            _equipmentRepository.Update(
                _selectedEquipment
            );

            LoadEquipment();
            ClearForm();

            ShowStatus(
                "Rekvizit je uspešno izmenjen.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri izmeni rekvizita: " +
                GetFriendlyError(exception),
                isError: true
            );
        }
    }

    private void OnDeleteEquipmentClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (_selectedEquipment is null)
        {
            ShowStatus(
                "Prvo izaberite rekvizit koji želite da obrišete.",
                isError: true
            );

            return;
        }

        try
        {
            long equipmentId =
                _selectedEquipment.Id;

            string equipmentName =
                _selectedEquipment.Name;

            _equipmentRepository.Delete(
                equipmentId
            );

            _equipment.Remove(
                _selectedEquipment
            );

            ClearForm();

            ShowStatus(
                $"Rekvizit „{equipmentName}” je uspešno obrisan.",
                isError: false
            );
        }
        catch (Exception exception)
        {
            ShowStatus(
                $"Greška pri brisanju rekvizita: " +
                GetFriendlyError(exception),
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
        string name =
            _equipmentNameTextBox?.Text?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowStatus(
                "Naziv rekvizita je obavezan.",
                isError: true
            );

            return null;
        }

        if (name.Length > 255)
        {
            ShowStatus(
                "Naziv rekvizita ne sme imati više od 255 karaktera.",
                isError: true
            );

            return null;
        }

        return name;
    }

    private void ClearForm()
    {
        _selectedEquipment = null;

        if (_equipmentListBox is not null)
        {
            _equipmentListBox.SelectedItem = null;
        }

        if (_equipmentNameTextBox is not null)
        {
            _equipmentNameTextBox.Text =
                string.Empty;
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

        _statusTextBlock.Foreground =
            isError
                ? Brushes.DarkRed
                : Brushes.DarkGreen;
    }

    private static string GetFriendlyError(
        Exception exception)
    {
        string message = exception.Message;

        if (
            message.Contains(
                "duplicate key",
                StringComparison.OrdinalIgnoreCase
            ) ||
            message.Contains(
                "unique constraint",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            return "Već postoji rekvizit sa tim nazivom.";
        }

        return message;
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}