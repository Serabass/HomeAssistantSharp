namespace Tests;

public class Tests
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void Test1()
  {
    Assert.That(HomeAssistantSharp.Sandbox.Add(1, 2), Is.EqualTo(3)); //
  }
}
