namespace ChatSupportSystem.CQRS;

public interface IQueryHandler<TQuery, TResult>
{
    TResult Handle(TQuery query);
}
