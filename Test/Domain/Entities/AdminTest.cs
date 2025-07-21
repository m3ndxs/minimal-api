using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public sealed class AdminTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var adm = new Admin();

        //Act
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        //Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
        
    }
}
