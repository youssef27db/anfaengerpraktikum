using GdUnit4;
using Godot;


[TestSuite]
public class TestTests {

    [TestCase]
    public void Test(){
        Assertions.AssertBool(true).IsEqual(true);
    }
}
