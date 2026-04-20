namespace RentMaster.Models;

public class Rental
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Legătura cu mașina
    public int CarId { get; set; }
    public Car? Car { get; set; }

    // Legătura cu clientul
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}