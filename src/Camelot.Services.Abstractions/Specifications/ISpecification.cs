namespace Camelot.Services.Abstractions.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T nodeModel);
    }
}