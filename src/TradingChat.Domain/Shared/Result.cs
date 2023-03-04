namespace TradingChat.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    protected Result(bool isSuccess, Error error) : this(isSuccess)
    {
        _errors.Add(error);
    }

    protected Result(bool isSuccess, IEnumerable<Error> errors) : this(isSuccess)
    {
        if (isSuccess && errors.Any() ||
            !isSuccess && !errors.Any())
        {
            throw new InvalidOperationException();
        }

        _errors.AddRange(errors);
    }

    public bool IsSuccess { get; }

    private readonly List<Error> _errors = new();
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    public static Result Success() => new(true);
    public static Result WithError(Error error) => new(false, error);
    public static Result WithErrors(IEnumerable<Error> errors) => new(false, errors.ToArray());
    public static Result<TValue> Success<TValue>(TValue? value) => new(value, true);
    public static Result<TValue> WithError<TValue>(Error error) => new(default, false, error);
    public static Result<TValue> WithErrors<TValue>(IEnumerable<Error> errors) => new(default, false, errors);

    public static implicit operator Result(Error error) => WithError(error);
    public static implicit operator Result(Error[] errors) => WithErrors(errors);
}
