using DotNet.Domain.Entidades;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace DotNet.Infrastructure.Mongo;

public static class RelatoTutorMapeamento
{
    public static void Registrar()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(RelatoTutor)))
        {
            return;
        }

        ConventionRegistry.Register(
            "camelCaseRelatos",
            new ConventionPack { new CamelCaseElementNameConvention(), new IgnoreExtraElementsConvention(true) },
            tipo => tipo == typeof(RelatoTutor));

        BsonClassMap.RegisterClassMap<RelatoTutor>(mapa =>
        {
            mapa.AutoMap();
            mapa.MapIdMember(r => r.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
        });
    }
}