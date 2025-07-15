using GdUnit4;
using Godot;


[TestSuite]
public class TestTests {

    [TestCase]
    public void TestDieTests(){
        Assertions.AssertBool(true).IsEqual(true);
    }
}
