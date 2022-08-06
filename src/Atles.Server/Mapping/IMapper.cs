namespace Atles.Server.Mapping;

public interface IMapper<in TFrom, out TO>
{
    TO Map(TFrom model, Guid userId);
}