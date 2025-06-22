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
    /// Interaction logic for BookingDialog.xaml
    /// </summary>
    public partial class BookingDialog : Window
    {
        private readonly IBookingService _bookingService;
        private readonly Customer _customer;
        public BookingReservation Booking { get; private set; }

        public BookingDialog(Customer customer)
        {
            InitializeComponent();
            _bookingService = new BookingService();
            _customer = customer;
            Booking = new BookingReservation
            {
                CustomerID = customer.CustomerID,
                Customer = customer
            };

            // Set minimum dates
            StartDatePicker.DisplayDateStart = DateTime.Today;
            EndDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);

            // Set default dates
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(1);
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(2);

            // Load available rooms
            LoadAvailableRooms();
        }

        private void LoadAvailableRooms()
        {
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;

                if (startDate < endDate)
                {
                    var availableRooms = _bookingService.GetAvailableRooms(startDate, endDate);
                    RoomsDataGrid.ItemsSource = availableRooms;

                    // Update nights count
                    int nights = (endDate - startDate).Days;
                    NightsTextBlock.Text = $"Nights: {nights}";

                    // Clear selection
                    RoomsDataGrid.SelectedItem = null;
                    BookButton.IsEnabled = false;
                    PricePerNightTextBlock.Text = "Price per night: $0.00";
                    TotalPriceTextBlock.Text = "Total: $0.00";
                }
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure end date is after start date
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                if (EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
                {
                    EndDatePicker.SelectedDate = StartDatePicker.SelectedDate.Value.AddDays(1);
                }
            }

            LoadAvailableRooms();
        }

        private void RoomsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRoom = RoomsDataGrid.SelectedItem as RoomInformation;
            if (selectedRoom != null && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;
                int nights = (endDate - startDate).Days;

                decimal pricePerNight = selectedRoom.RoomPricePerDate;
                decimal totalPrice = pricePerNight * nights;

                PricePerNightTextBlock.Text = $"Price per night: {pricePerNight:C}";
                TotalPriceTextBlock.Text = $"Total: {totalPrice:C}";

                BookButton.IsEnabled = true;

                // Update booking object
                Booking.RoomID = selectedRoom.RoomID;
                Booking.RoomInformation = selectedRoom;
                Booking.ActualStartDate = startDate;
                Booking.ActualEndDate = endDate;
                Booking.TotalPrice = totalPrice;
            }
            else
            {
                BookButton.IsEnabled = false;
                PricePerNightTextBlock.Text = "Price per night: $0.00";
                TotalPriceTextBlock.Text = "Total: $0.00";
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (_bookingService.AddBooking(Booking))
            {
                MessageBox.Show($"Room {Booking.RoomInformation.RoomNumber} booked successfully from " +
                               $"{Booking.ActualStartDate.ToShortDateString()} to {Booking.ActualEndDate.ToShortDateString()}.",
                               "Booking Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Unable to complete booking. Please try again.",
                               "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
