using IdGen;

public static class IdGeneratorUtil
{
    public static long GetNextId()
    {
        var generator = new IdGenerator(0);
        var id = generator.CreateId();
        Console.WriteLine("Vector Id: " + id);
        return id;
    }
}