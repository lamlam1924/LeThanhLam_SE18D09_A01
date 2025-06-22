using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Models;
using Services;

namespace LeThanhLamWPF.Views
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly ICustomerService _customerService;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public AdminWindow()
        {
            InitializeComponent();

            try
            {
                _customerService = new CustomerService();
                _roomService = new RoomService();
                _bookingService = new BookingService();

                // Load data after the window is fully loaded
                this.Loaded += AdminWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize admin window: {ex.Message}", "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load initial data: {ex.Message}", "Data Loading Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadData()
        {
            try
            {
                // Load customers
                if (_customerService != null && CustomersDataGrid != null)
                    CustomersDataGrid.ItemsSource = _customerService.GetAllCustomers();

                // Load rooms
                if (_roomService != null && RoomsDataGrid != null)
                    RoomsDataGrid.ItemsSource = _roomService.GetAllRooms();

                // Load bookings only if the tab control and filter are available
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Data Loading Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadBookings()
        {
            try
            {
                if (_bookingService == null || BookingsDataGrid == null)
                {
                    return;
                }

                var bookings = _bookingService.GetAllBookings();

                // Apply filter only if BookingStatusFilter is available and has a selection
                if (BookingStatusFilter != null && BookingStatusFilter.SelectedIndex > 0)
                {
                    int statusFilter = BookingStatusFilter.SelectedIndex;
                    bookings = bookings?.Where(b => b != null && b.BookingStatus == statusFilter).ToList() ?? new List<BookingReservation>();
                }

                BookingsDataGrid.ItemsSource = bookings ?? new List<BookingReservation>();
            }
            catch
            {
                // Silent fail - just set empty data source
                if (BookingsDataGrid != null)
                    BookingsDataGrid.ItemsSource = new List<BookingReservation>();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Logout Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_customerService == null || CustomersDataGrid == null) return;

                string searchTerm = CustomerSearchTextBox?.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(searchTerm))
                {
                    CustomersDataGrid.ItemsSource = _customerService.GetAllCustomers();
                }
                else
                {
                    CustomersDataGrid.ItemsSource = _customerService.SearchCustomers(searchTerm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during customer search: {ex.Message}", "Search Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_customerService == null) return;

                var dialog = new CustomerDialog();
                if (dialog.ShowDialog() == true && dialog.Customer != null)
                {
                    _customerService.AddCustomer(dialog.Customer);
                    LoadCustomerData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Add Customer Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_customerService == null || CustomersDataGrid == null) return;

                var selectedCustomer = CustomersDataGrid.SelectedItem as Customer;
                if (selectedCustomer != null)
                {
                    var dialog = new CustomerDialog(selectedCustomer);
                    if (dialog.ShowDialog() == true && dialog.Customer != null)
                    {
                        _customerService.UpdateCustomer(dialog.Customer);
                        LoadCustomerData();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a customer to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing customer: {ex.Message}", "Edit Customer Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_customerService == null || CustomersDataGrid == null) return;

                var selectedCustomer = CustomersDataGrid.SelectedItem as Customer;
                if (selectedCustomer != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete customer '{selectedCustomer.CustomerFullName}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _customerService.DeleteCustomer(selectedCustomer.CustomerID);
                        LoadCustomerData();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a customer to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Delete Customer Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchRoomsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_roomService == null || RoomsDataGrid == null) return;

                string searchTerm = RoomSearchTextBox?.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(searchTerm))
                {
                    RoomsDataGrid.ItemsSource = _roomService.GetAllRooms();
                }
                else
                {
                    RoomsDataGrid.ItemsSource = _roomService.SearchRooms(searchTerm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during room search: {ex.Message}", "Search Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_roomService == null) return;

                var dialog = new RoomDialog();
                if (dialog.ShowDialog() == true && dialog.Room != null)
                {
                    _roomService.AddRoom(dialog.Room);
                    LoadRoomData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding room: {ex.Message}", "Add Room Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_roomService == null || RoomsDataGrid == null) return;

                var selectedRoom = RoomsDataGrid.SelectedItem as RoomInformation;
                if (selectedRoom != null)
                {
                    var dialog = new RoomDialog(selectedRoom);
                    if (dialog.ShowDialog() == true && dialog.Room != null)
                    {
                        _roomService.UpdateRoom(dialog.Room);
                        LoadRoomData();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a room to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing room: {ex.Message}", "Edit Room Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_roomService == null || RoomsDataGrid == null) return;

                var selectedRoom = RoomsDataGrid.SelectedItem as RoomInformation;
                if (selectedRoom != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete room '{selectedRoom.RoomNumber}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _roomService.DeleteRoom(selectedRoom.RoomID);
                        LoadRoomData();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a room to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting room: {ex.Message}", "Delete Room Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookingStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Only load bookings if the window is fully loaded
                if (this.IsLoaded)
                {
                    LoadBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering bookings: {ex.Message}", "Filter Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Helper methods to load specific data
        private void LoadCustomerData()
        {
            try
            {
                if (_customerService != null && CustomersDataGrid != null)
                    CustomersDataGrid.ItemsSource = _customerService.GetAllCustomers();
            }
            catch
            {
                // Silent fail for helper method
            }
        }

        private void LoadRoomData()
        {
            try
            {
                if (_roomService != null && RoomsDataGrid != null)
                    RoomsDataGrid.ItemsSource = _roomService.GetAllRooms();
            }
            catch
            {
                // Silent fail for helper method
            }
        }
    }
}
