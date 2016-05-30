namespace Forecaster.Contracts
{
    public interface IStatelessViewModel<out TModel> where TModel : class, new()
    {
        TModel Model { get; }
    }
}