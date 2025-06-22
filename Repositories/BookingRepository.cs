using Models;

namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private static BookingRepository _instance;
        private static readonly object _lock = new object();
        private List<BookingReservation> _bookings;
        private readonly IRoomRepository _roomRepository;
        private readonly ICustomerRepository _customerRepository;

        private BookingRepository()
        {
            try
            {
                _roomRepository = RoomRepository.Instance;
                _customerRepository = CustomerRepository.Instance;
                InitializeData();
            }
            catch (Exception)
            {
                _bookings = new List<BookingReservation>();
            }
        }

        public static BookingRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new BookingRepository();
                    }
                }
                return _instance;
            }
        }

        private void InitializeData()
        {
            try
            {
                _bookings = new List<BookingReservation>();
                Console.WriteLine($"CustomerRepository: {_customerRepository != null}, RoomRepository: {_roomRepository != null}");

                // Only add sample data if repositories are available
                if (_customerRepository != null && _roomRepository != null)
                {
                    var customer1 = _customerRepository.GetCustomerById(1);
                    var customer2 = _customerRepository.GetCustomerById(2);
                    var room1 = _roomRepository.GetRoomById(1);
                    var room2 = _roomRepository.GetRoomById(2);

                    if (customer1 != null && room1 != null)
                    {
                        _bookings.Add(new BookingReservation
                        {
                            BookingReservationID = 1,
                            BookingDate = DateTime.Now.AddDays(-5),
                            TotalPrice = 200,
                            CustomerID = 1,
                            RoomID = 1,
                            ActualStartDate = DateTime.Now.AddDays(5),
                            ActualEndDate = DateTime.Now.AddDays(7),
                            BookingStatus = 1,
                            Customer = customer1,
                            RoomInformation = room1
                        });
                    }

                    if (customer2 != null && room2 != null)
                    {
                        _bookings.Add(new BookingReservation
                        {
                            BookingReservationID = 2,
                            BookingDate = DateTime.Now.AddDays(-10),
                            TotalPrice = 600,
                            CustomerID = 2,
                            RoomID = 2,
                            ActualStartDate = DateTime.Now.AddDays(-5),
                            ActualEndDate = DateTime.Now.AddDays(-1),
                            BookingStatus = 1,
                            Customer = customer2,
                            RoomInformation = room2
                        });
                    }
                }
                Console.WriteLine($"Bookings count: {_bookings.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeData: {ex.Message}");
                _bookings = new List<BookingReservation>();
            }
        }

        public List<BookingReservation> GetAllBookings()
        {
            try
            {
                return _bookings?.Where(b => b != null && b.BookingStatus != 3).ToList() ?? new List<BookingReservation>();
            }
            catch (Exception)
            {
                return new List<BookingReservation>();
            }
        }

        public List<BookingReservation> GetBookingsByCustomerId(int customerId)
        {
            try
            {
                return _bookings?.Where(b => b != null && b.CustomerID == customerId && b.BookingStatus != 3).ToList() ?? new List<BookingReservation>();
            }
            catch (Exception)
            {
                return new List<BookingReservation>();
            }
        }

        public BookingReservation GetBookingById(int id)
        {
            try
            {
                return _bookings?.FirstOrDefault(b => b != null && b.BookingReservationID == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddBooking(BookingReservation booking)
        {
            try
            {
                if (booking == null || _bookings == null) return;

                booking.BookingReservationID = _bookings.Count > 0 ? _bookings.Max(b => b?.BookingReservationID ?? 0) + 1 : 1;
                booking.BookingDate = DateTime.Now;
                booking.BookingStatus = 1; // 1 = Active

                if (_customerRepository != null)
                    booking.Customer = _customerRepository.GetCustomerById(booking.CustomerID);

                if (_roomRepository != null)
                    booking.RoomInformation = _roomRepository.GetRoomById(booking.RoomID);

                // Calculate total price based on days and room price
                if (booking.RoomInformation != null)
                {
                    TimeSpan duration = booking.ActualEndDate - booking.ActualStartDate;
                    int days = duration.Days > 0 ? duration.Days : 1;
                    booking.TotalPrice = days * booking.RoomInformation.RoomPricePerDate;
                }

                _bookings.Add(booking);
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public void UpdateBooking(BookingReservation booking)
        {
            try
            {
                if (booking == null || _bookings == null) return;

                var existingBooking = _bookings.FirstOrDefault(b => b != null && b.BookingReservationID == booking.BookingReservationID);
                if (existingBooking != null)
                {
                    existingBooking.ActualStartDate = booking.ActualStartDate;
                    existingBooking.ActualEndDate = booking.ActualEndDate;
                    existingBooking.RoomID = booking.RoomID;

                    if (_roomRepository != null)
                        existingBooking.RoomInformation = _roomRepository.GetRoomById(booking.RoomID);

                    // Recalculate total price
                    if (existingBooking.RoomInformation != null)
                    {
                        TimeSpan duration = existingBooking.ActualEndDate - existingBooking.ActualStartDate;
                        int days = duration.Days > 0 ? duration.Days : 1;
                        existingBooking.TotalPrice = days * existingBooking.RoomInformation.RoomPricePerDate;
                    }
                }
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public void CancelBooking(int id)
        {
            try
            {
                if (_bookings == null) return;

                var booking = _bookings.FirstOrDefault(b => b != null && b.BookingReservationID == id);
                if (booking != null)
                {
                    booking.BookingStatus = 2; // 2 = Cancelled
                }
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public List<RoomInformation> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (_roomRepository == null) return new List<RoomInformation>();

                var allRooms = _roomRepository.GetAllRooms();
                var availableRooms = new List<RoomInformation>();

                foreach (var room in allRooms ?? new List<RoomInformation>())
                {
                    if (room != null && IsRoomAvailable(room.RoomID, startDate, endDate))
                    {
                        availableRooms.Add(room);
                    }
                }

                return availableRooms;
            }
            catch (Exception)
            {
                return new List<RoomInformation>();
            }
        }

        public bool IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            try
            {
                if (_bookings == null) return true;

                var overlappingBookings = _bookings.Where(b =>
                    b != null &&
                    b.RoomID == roomId &&
                    b.BookingStatus == 1 && // Only check active bookings
                    (excludeBookingId == null || b.BookingReservationID != excludeBookingId) &&
                    (
                        // Check if the new booking overlaps with existing bookings
                        (startDate >= b.ActualStartDate && startDate < b.ActualEndDate) || // Start date falls within existing booking
                        (endDate > b.ActualStartDate && endDate <= b.ActualEndDate) || // End date falls within existing booking
                        (startDate <= b.ActualStartDate && endDate >= b.ActualEndDate) // New booking completely covers existing booking
                    )
                );

                return !overlappingBookings.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
