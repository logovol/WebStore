namespace WebStore.Interfaces.TestAPI;

public interface IValuesService
{
    IEnumerable<string> GetValues();

    string? GetById(int id);

    void Add(string Value);

    void Edit(int Id, string Value);

    bool Delete(int Id);
}
