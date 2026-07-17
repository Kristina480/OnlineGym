using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Services;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class RateWorkoutItemsWindow : Window
{
    public RateWorkoutItemsWindow(long clientId)
    {
        InitializeComponent();

        var service = new WorkoutItemService(
            new WorkoutItemRepository(),
            new WorkoutRepository(),
            new CollaborationRepository());

        var exerciseRepository = new ExerciseRepository();

        DataContext = new RateWorkoutItemsViewModel(service, exerciseRepository, clientId);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}