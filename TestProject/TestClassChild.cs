namespace TestProject;

public class TestClassChild : TestClass
{
    public int x2;
    public void ChildMethod()
    {
    }

    public override void Test()
    {
        base.Test();
    }
}

public class TestClassChild2 : TestClassChild {}