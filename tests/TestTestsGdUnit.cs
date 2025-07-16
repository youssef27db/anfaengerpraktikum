using GdUnit4;
using Godot;


[TestSuite]
public class TestTestsGdUnit {

    [TestCase]
    public void Test(){
        Assertions.AssertBool(true).IsEqual(true);
    }
}
