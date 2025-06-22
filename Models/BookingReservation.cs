namespace Models
{
    public class BookingReservation
    {
        public int BookingReservationID { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerID { get; set; }
        public int RoomID { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public int BookingStatus { get; set; }

        public Customer Customer { get; set; }
        public RoomInformation RoomInformation { get; set; }

        public BookingReservation()
        {
            Customer = new Customer();
            RoomInformation = new RoomInformation();
            BookingDate = DateTime.Now;
            ActualStartDate = DateTime.Now;
            ActualEndDate = DateTime.Now;
        }
    }
}
