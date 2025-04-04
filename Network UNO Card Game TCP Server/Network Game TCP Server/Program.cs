// See https://aka.ms/new-console-template for more information
using MyNetworkGame.TCPServer;

LogWriterUtil.Init();

NetManager.Main();

#region demo
void test()
{
    B b1 = new B();
    b1.list = new List<A>()
    {
        new A(){aa = 1},
        new A(){aa = 2},
        new A(){aa = 3},
    };

    B b2 = new B();
    b2.list = new List<A>()
    {
        new A(){aa = 4},
        new A(){aa = 5},
        new A(){aa = 6},
    };

    List<B> roles = new List<B>() { b1, b2 };
    Debug.Log(string.Join("|", roles.Select(r => string.Join(",", r.list.Select(c => c.aa)))));
}

//test();

public class Debug
{
    public static void Log(string info)
    {
        Console.WriteLine(info);
    }
}

class A
{
    public int aa;
}
class B
{
    public List<A> list;
}
#endregion