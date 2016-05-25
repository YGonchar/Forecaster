namespace Forecaster.Contracts
{
    public interface IStatelessViewModel<TModel> where TModel : class, new()
    {
        TModel Model { get; }
    }
}