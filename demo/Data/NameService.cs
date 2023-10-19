using Bogus;

namespace demo.Data;

public class NameService
{
    private string _name = new Faker().Commerce.ProductName();
    public string GetName()
        => _name;
}
