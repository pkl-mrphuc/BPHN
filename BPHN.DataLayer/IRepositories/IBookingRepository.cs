using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingRepository
    {
        Booking? GetById(string id);
        bool CheckFreeTimeFrame(Booking data);
        bool Insert(Booking data);
    }
}
