namespace RestaurantAPI.Exceptions
{
    public class BadRequestException : Exception
    {
        //ten konstruktor wywola konstruktor bazowy z wiadomoscia w argumencie (invalid username or password)
        public BadRequestException(string message) : base(message)
        {

        }
    }
}
