namespace Core.Utilities.Results.Concrete.Success;

public class SuccessResult : Result
{
    public SuccessResult(bool success) : base(true) { }
    public SuccessResult(bool success, string message) : base(true,message){}
}