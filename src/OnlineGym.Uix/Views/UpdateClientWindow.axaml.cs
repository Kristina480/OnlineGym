using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;

namespace OnlineGym.Uix.Views;

public partial class UpdateClientWindow : Window
{
    private Client _client;
    private ClientRepository _repo = new ClientRepository();
    public UpdateClientWindow(Client client)
    {
        InitializeComponent();
        _client = client;
        LoadClientData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadClientData()
    {
        try
        {
            var firstnameBox = this.FindControl<TextBox>("FirstNameBox");
            var lastnameBox = this.FindControl<TextBox>("LastNameBox");
            var heightBox = this.FindControl<TextBox>("HeightBox");
            var weightBox = this.FindControl<TextBox>("WeightBox");
            var goalBox = this.FindControl<TextBox>("GoalBox");
            var healthIssuesBox = this.FindControl<TextBox>("HealthIssuesBox");
            var workoutsPerWeekBox = this.FindControl<TextBox>("WorkoutsPerWeekBox");
            
            if (firstnameBox != null) firstnameBox.Text = _client.FirstName;
            if (lastnameBox != null) lastnameBox.Text = _client.LastName;
            if (heightBox != null) heightBox.Text = _client.Height.ToString();
            if (weightBox != null) weightBox.Text = _client.Weight.ToString();
            if (goalBox != null) goalBox.Text = _client.Goal ?? string.Empty;
            if (healthIssuesBox != null) healthIssuesBox.Text = _client.HealthIssues ?? string.Empty;
            if (workoutsPerWeekBox != null) workoutsPerWeekBox.Text = _client.WorkoutsPerWeek.ToString();
        }
        catch (Exception ex)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
            {
                errorText.Text = $"Greska: {ex.Message}";
            }
        }
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var firstnameBox = this.FindControl<TextBox>("FirstNameBox");
        var lastnameBox = this.FindControl<TextBox>("LastNameBox");
        var heightBox = this.FindControl<TextBox>("HeightBox");
        var weightBox = this.FindControl<TextBox>("WeightBox");
        var goalBox = this.FindControl<TextBox>("GoalBox");
        var healthIssuesBox = this.FindControl<TextBox>("HealthIssuesBox");
        var workoutsPerWeekBox = this.FindControl<TextBox>("WorkoutsPerWeekBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        
        string? first = firstnameBox?.Text?.Trim();
        string? last = lastnameBox?.Text?.Trim();
        string? heightText = heightBox?.Text?.Trim();
        string? weightText = weightBox?.Text?.Trim();
        string? goal = goalBox?.Text?.Trim();
        string? health = healthIssuesBox?.Text?.Trim();
        string? workoutsText = workoutsPerWeekBox?.Text?.Trim();
        
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) || string.IsNullOrWhiteSpace(heightText) 
            || string.IsNullOrWhiteSpace(weightText) || string.IsNullOrWhiteSpace(workoutsText))
        {
            errorText.Text = "Niste popunili obavezna polja.";
            return;
        }
        
        
        if (!double.TryParse(heightText, out double height))
        {
            errorText.Text = "Visina mora biti broj.";
            return;
        }
        
        if (!double.TryParse(weightText, out double weight))
        {
            errorText.Text = "Tezina mora biti broj.";
            return;
        }
        if (!int.TryParse(workoutsText, out int workoutsPerWeek))
        {
            errorText.Text = "Broj treninga mora biti ceo broj.";
            return;
        }
        
        try
        {
            _repo.UpdateClient(_client.ClientId, first, last, height, weight, string.IsNullOrWhiteSpace(goal) ? null : goal, 
                string.IsNullOrWhiteSpace(health) ? null : health, workoutsPerWeek);
            
            _client.FirstName = first;
            _client.LastName = last;
            _client.Height = height;
            _client.Weight = weight;
            _client.Goal = string.IsNullOrWhiteSpace(goal) ? null : goal;
            _client.HealthIssues = string.IsNullOrWhiteSpace(health) ? null : health;
            _client.WorkoutsPerWeek = workoutsPerWeek;
            
            errorText.Text = "Uspesno ste azuirirali podatke!";
        }
        catch (Exception ex)
        {
            errorText.Text = $"Greska: {ex.Message}";
        }
    }
}