using IdGen;

public static class IdGeneratorUtils
{

    public static long GetNextId()
    {
        var generator = new IdGenerator(0); 
        long id = generator.CreateId();
        Console.WriteLine("Vector Id: "+id); 
        return id;
    }
}