namespace CmsrLogin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var net = new NetCheck();
            if (args.Length > 0 )
            {
                if (!net.CheckTime().Result)
                    net.CmsrLogin().Wait();
            }
            else
            {
                System.Timers.Timer timer = new System.Timers.Timer
                {
                    Interval = 5000,
                    AutoReset = true
                };
                timer.Elapsed += async (o, e) =>
                {
                    if (!await net.CheckTime())
                        await net.CmsrLogin();
                };
                timer.Start();
                while (true)
                {
                    Console.ReadKey();
                }
            }
        }
    }
}