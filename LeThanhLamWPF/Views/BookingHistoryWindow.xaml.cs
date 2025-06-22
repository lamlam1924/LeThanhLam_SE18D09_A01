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
    /// Interaction logic for BookingHistoryWindow.xaml
    /// </summary>
    public partial class BookingHistoryWindow : Window
    {
        private readonly IBookingService _bookingService;
        private readonly Customer _customer;
        private List<BookingViewModel> _bookings;

        public BookingHistoryWindow(Customer customer)
        {
            InitializeComponent();
            _bookingService = new BookingService();
            _customer = customer;
            LoadBookings();
        }

        private void LoadBookings()
        {
            var bookings = _bookingService.GetBookingsByCustomerId(_customer.CustomerID);
            _bookings = bookings.Select(b => new BookingViewModel(b)).ToList();
            BookingsDataGrid.ItemsSource = _bookings;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int bookingId)
            {
                var result = MessageBox.Show("Are you sure you want to cancel this booking?",
                    "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_bookingService.CancelBooking(bookingId))
                    {
                        MessageBox.Show("Booking cancelled successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadBookings();
                    }
                    else
                    {
                        MessageBox.Show("Failed to cancel booking.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // View model to add computed properties for the DataGrid
        private class BookingViewModel
        {
            private readonly BookingReservation _booking;

            public BookingViewModel(BookingReservation booking)
            {
                _booking = booking;
            }

            public int BookingReservationID => _booking.BookingReservationID;
            public DateTime BookingDate => _booking.BookingDate;
            public decimal TotalPrice => _booking.TotalPrice;
            public int CustomerID => _booking.CustomerID;
            public int RoomID => _booking.RoomID;
            public DateTime ActualStartDate => _booking.ActualStartDate;
            public DateTime ActualEndDate => _booking.ActualEndDate;
            public int BookingStatus => _booking.BookingStatus;
            public Customer Customer => _booking.Customer;
            public RoomInformation RoomInformation => _booking.RoomInformation;

            public string BookingStatusText
            {
                get
                {
                    if (BookingStatus == 1) return "Active";
                    if (BookingStatus == 2) return "Cancelled";
                    if (BookingStatus == 3) return "Completed";
                    return "Unknown";
                }
            }

            public bool CanCancel
            {
                get
                {
                    return BookingStatus == 1 && ActualStartDate > DateTime.Today;
                }
            }
        }
    }
}
