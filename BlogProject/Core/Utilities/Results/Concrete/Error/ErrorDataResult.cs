namespace Core.Utilities.Results.Concrete.Success;

public class ErrorDataResult<T> : DataResult<T>
{
    public ErrorDataResult(T data, bool success) : base(data, false){}
    public ErrorDataResult(T data, bool success, string message) : base(data, false, message){}
}