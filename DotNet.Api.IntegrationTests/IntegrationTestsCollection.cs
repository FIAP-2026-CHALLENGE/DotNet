using Xunit;

namespace DotNet.Api.IntegrationTests;

/// <summary>
/// Define uma Collection Fixture: garante que a mesma instância de
/// CustomWebApplicationFactory (e, portanto, o mesmo host em memória e o mesmo
/// banco InMemory) seja compartilhada entre TODAS as classes de teste marcadas
/// com [Collection("Integration Tests")], evitando subir a aplicação mais de uma vez.
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestsCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}