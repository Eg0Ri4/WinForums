using System;
using System.IO;
using System.Windows;
using Microsoft.Win32; // для OpenFileDialog/SaveFileDialog
using SmartTravelPlanner.Core;

namespace SmartTravelPlanner
{
    public partial class MainWindow : Window
    {
        private Traveler ? traveler;
        private CityGraph ? map;
        private readonly Logger<string> logger = new Logger<string>();

        public MainWindow()
        {
            InitializeComponent();
            DisableRouteControls();
        }

        private void CreateTraveler_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TravelerNameInput.Text))
            {
                MessageBox.Show("Please enter traveler name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                TravelerNameInput.Focus();
                return;
            }

            string ? location = string.IsNullOrWhiteSpace(CurrentLocationInput.Text) ? null : CurrentLocationInput.Text.Trim();
            if (map != null && location != null && !map.ContainsCity(location))
            {
                MessageBox.Show($"Starting city '{location}' is not on the map!\nTraveler was NOT created.",
                                "Invalid City", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            traveler = new Traveler(TravelerNameInput.Text.Trim());
            if (location != null) traveler.SetLocation(location);

            MessageBox.Show($"Traveler '{traveler.GetName()}' created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateRouteDisplay();
            EnableRouteControls();
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new() { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*", Title = "Select Map File" };
            if (ofd.ShowDialog() != true) return;

            try
            {
                map = CityGraph.LoadFromFile(ofd.FileName);
                MessageBox.Show("Map loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                EnableRouteControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load map:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlanRoute_Click(object sender, RoutedEventArgs e)
        {
            if (traveler == null) 
            { 
                MessageBox.Show("Create a traveler first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }
            if (map == null) 
            { 
                MessageBox.Show("Load a map first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }

            string ? currentLocation = traveler.GetLocation();
            string destination = DestinationInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(destination))
            {
                MessageBox.Show("Enter destination city!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                DestinationInput.Focus();
                return;
            }

            if (!map.ContainsCity(destination))
            {
                MessageBox.Show($"Destination city '{destination}' is not on the map!", "City Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                DestinationInput.Focus();
                return;
            }

            if (!traveler.PlanRouteTo(destination, map))
            {
                MessageBox.Show("No route found between these cities.", "No Route", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            UpdateRouteDisplay();
            logger.Add($"Route planned: {traveler.GetName()} from {currentLocation} to {destination}");
            MessageBox.Show("Route planned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (traveler == null) 
            { 
                MessageBox.Show("Nothing to save. Create a traveler first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }

            SaveFileDialog sfd = new() { Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*", FileName = $"{traveler.GetName()}.json" };
            if (sfd.ShowDialog() != true) 
                return;

            try
            {
                traveler.SaveToFile(sfd.FileName);
                logger.Flush("log.txt");
                MessageBox.Show("Traveler saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new() { Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*" };
            if (ofd.ShowDialog() != true) return;

            try
            {
                traveler = Traveler.LoadFromFile(ofd.FileName);
                TravelerNameInput.Text = traveler.GetName();
                CurrentLocationInput.Text = traveler.GetLocation() ?? "";
                DestinationInput.Clear();
                UpdateRouteDisplay();
                EnableRouteControls();
                MessageBox.Show("Traveler loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            traveler?.ClearRoute();
            DestinationInput.Clear();
            UpdateRouteDisplay();
        }

        private void UpdateRouteDisplay()
        {
            DistanceText.Text = " -";
            if (traveler == null || map == null) 
                return;

            if (traveler.GetStopCount() > 1)
            {
                int distance = map.GetPathDistance(traveler.GetRouteItems());
                if (distance >= 0) DistanceText.Text = $"{distance} km";
            }

            MapStatusText.Text = traveler.GetRoute();
        }

        private void EnableRouteControls()
        {
            bool hasTraveler = traveler != null;
            bool hasMap = map != null;

            DestinationInput.IsEnabled = hasTraveler && hasMap;
        }

        private void DisableRouteControls()
        {
            DestinationInput.IsEnabled = false;
        }
    }
}
