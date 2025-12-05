using System;
using System.Windows;
using System.Windows.Media;

namespace SmartTravelPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 1. Create Traveler Button
        private void CreateTraveler_Click(object sender, RoutedEventArgs e)
        {
            string name = TravelerNameInput.Text;
            string location = CurrentLocationInput.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter both Name and Current Location.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Traveler '{name}' created at '{location}'!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 2. Load Map Button
        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            // Simulate loading a map
            MapStatusText.Text = "Map Loaded Successfully";
            MapStatusText.Foreground = Brushes.Black;
        }

        // 3. Plan Route Button
        private void PlanRoute_Click(object sender, RoutedEventArgs e)
        {
            string destination = DestinationInput.Text;
            string currentLoc = CurrentLocationInput.Text;

            if (string.IsNullOrWhiteSpace(destination))
            {
                MessageBox.Show("Please enter a destination.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Simulate calculation logic (Random distance for demo purposes)
            Random rnd = new Random();
            int distance = rnd.Next(50, 5000); // Random distance between 50 and 5000 km

            DistanceText.Text = $"{distance} km (from {currentLoc} to {destination})";
            MapStatusText.Text = $"Route Visualized: {currentLoc} -> {destination}";
        }

        // 4. Save Button
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // In a real app, this would save to a database or file
            MessageBox.Show("Trip details saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 5. Load Button
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            // Simulating loading data
            TravelerNameInput.Text = "John Doe";
            CurrentLocationInput.Text = "New York";
            DestinationInput.Text = "London";
            DistanceText.Text = "5576 km";
            MapStatusText.Text = "Route Loaded";
        }

        // 6. Clear Route Button
        private void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            TravelerNameInput.Clear();
            CurrentLocationInput.Clear();
            DestinationInput.Clear();
            DistanceText.Text = " -";
            MapStatusText.Text = "Map View Area";
            MapStatusText.Foreground = Brushes.Gray;
        }

        // 7. Exit Button
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}