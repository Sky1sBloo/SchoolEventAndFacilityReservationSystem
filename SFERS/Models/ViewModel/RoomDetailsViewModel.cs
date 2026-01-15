using System.Collections.Generic;

namespace SFERS.Models.ViewModel
{
    public class RoomDetailsViewModel
    {
        public RoomViewModel Room { get; set; } = new RoomViewModel();
        public List<ReservationViewModel> UpcomingReservations { get; set; } = new List<ReservationViewModel>();
    }
}
