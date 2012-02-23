using System;
using Fixturizer;

public class Runner
{
        public static void Main()
        {
                var loader = new Loader("fixtures");
                dynamic fixture = loader.Load("element");

                Console.WriteLine(fixture[0]["id"]);
        }
}
