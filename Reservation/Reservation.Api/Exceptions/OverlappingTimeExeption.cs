namespace Reservation.Api.Exceptions
{
    public class OverlappingTimeExeption : Exception
    {
        public OverlappingTimeExeption(string message) : base(message) { }
    }
}
