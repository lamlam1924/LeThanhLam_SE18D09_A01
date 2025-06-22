﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Repositories;

namespace Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService()
        {
            _roomRepository = RoomRepository.Instance;
        }

        public List<RoomInformation> GetAllRooms()
        {
            return _roomRepository.GetAllRooms();
        }

        public RoomInformation GetRoomById(int id)
        {
            return _roomRepository.GetRoomById(id);
        }

        public void AddRoom(RoomInformation room)
        {
            if (ValidateRoom(room))
            {
                _roomRepository.AddRoom(room);
            }
        }

        public void UpdateRoom(RoomInformation room)
        {
            if (ValidateRoom(room))
            {
                _roomRepository.UpdateRoom(room);
            }
        }

        public void DeleteRoom(int id)
        {
            _roomRepository.DeleteRoom(id);
        }

        public List<RoomInformation> SearchRooms(string searchTerm)
        {
            return _roomRepository.SearchRooms(searchTerm);
        }

        public List<RoomType> GetAllRoomTypes()
        {
            return _roomRepository.GetAllRoomTypes();
        }

        public bool ValidateRoom(RoomInformation room)
        {
            if (room == null) return false;
            if (string.IsNullOrEmpty(room.RoomNumber)) return false;
            if (room.RoomMaxCapacity <= 0) return false;
            if (room.RoomPricePerDate <= 0) return false;
            if (room.RoomTypeID <= 0) return false;

            return true;
        }
    }
}
