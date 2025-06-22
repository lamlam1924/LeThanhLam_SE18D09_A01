using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private static RoomRepository _instance;
        private static readonly object _lock = new object();
        private List<RoomInformation> _rooms;
        private List<RoomType> _roomTypes;

        private RoomRepository()
        {
            InitializeData();
        }

        public static RoomRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new RoomRepository();
                    }
                }
                return _instance;
            }
        }

        private void InitializeData()
        {
            _roomTypes = new List<RoomType>
            {
                new RoomType { RoomTypeID = 1, RoomTypeName = "Standard", TypeDescription = "Standard room", TypeNote = "Basic amenities" },
                new RoomType { RoomTypeID = 2, RoomTypeName = "Superior", TypeDescription = "Superior room", TypeNote = "Vip amenities" },
                new RoomType { RoomTypeID = 3, RoomTypeName = "Deluxe", TypeDescription = "Deluxe room", TypeNote = "Premium amenities" },
                new RoomType { RoomTypeID = 4, RoomTypeName = "Suite", TypeDescription = "Suite room", TypeNote = "Luxury amenities" }
            };

            _rooms = new List<RoomInformation>
            {
                new RoomInformation
                {
                    RoomID = 1,
                    RoomNumber = "101",
                    RoomDescription = "Standard room with city view",
                    RoomMaxCapacity = 2,
                    RoomStatus = 1,
                    RoomPricePerDate = 100,
                    RoomTypeID = 1,
                    RoomType = _roomTypes[0]
                },
                new RoomInformation
                {
                    RoomID = 2,
                    RoomNumber = "201",
                    RoomDescription = "Deluxe room with ocean view",
                    RoomMaxCapacity = 4,
                    RoomStatus = 1,
                    RoomPricePerDate = 200,
                    RoomTypeID = 2,
                    RoomType = _roomTypes[1]
                }
            };
        }

        public List<RoomInformation> GetAllRooms()
        {
            return _rooms.Where(r => r.RoomStatus == 1).ToList();
        }

        public RoomInformation GetRoomById(int id)
        {
            return _rooms.FirstOrDefault(r => r.RoomID == id && r.RoomStatus == 1);
        }

        public void AddRoom(RoomInformation room)
        {
            room.RoomID = _rooms.Count > 0 ? _rooms.Max(r => r.RoomID) + 1 : 1;
            room.RoomStatus = 1;
            room.RoomType = _roomTypes.FirstOrDefault(rt => rt.RoomTypeID == room.RoomTypeID);
            _rooms.Add(room);
        }

        public void UpdateRoom(RoomInformation room)
        {
            var existingRoom = _rooms.FirstOrDefault(r => r.RoomID == room.RoomID);
            if (existingRoom != null)
            {
                existingRoom.RoomNumber = room.RoomNumber;
                existingRoom.RoomDescription = room.RoomDescription;
                existingRoom.RoomMaxCapacity = room.RoomMaxCapacity;
                existingRoom.RoomPricePerDate = room.RoomPricePerDate;
                existingRoom.RoomTypeID = room.RoomTypeID;
                existingRoom.RoomType = _roomTypes.FirstOrDefault(rt => rt.RoomTypeID == room.RoomTypeID);
            }
        }

        public void DeleteRoom(int id)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomID == id);
            if (room != null)
            {
                room.RoomStatus = 2;
            }
        }

        public List<RoomInformation> SearchRooms(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return GetAllRooms();

            return _rooms.Where(r => r.RoomStatus == 1 &&
                (r.RoomNumber.ToLower().Contains(searchTerm.ToLower()) ||
                 r.RoomDescription.ToLower().Contains(searchTerm.ToLower()))).ToList();
        }

        public List<RoomType> GetAllRoomTypes()
        {
            return _roomTypes;
        }
    }
}
