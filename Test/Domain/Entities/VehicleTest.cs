using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public sealed class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var vehicle = new Vehicle();

        //Act
        vehicle.Id = 1;
        vehicle.Name = "Fusca";
        vehicle.Brand = "VW";
        vehicle.Year = 1979;

        //Assert
        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Fusca", vehicle.Name);
        Assert.AreEqual("VW", vehicle.Brand);
        Assert.AreEqual(1979, vehicle.Year);
        
    }
}
