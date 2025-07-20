using Core.Utilities.Results.Abstract;

namespace Core.Utilities.Results.Concrete;

public class Result : IResult
{
    public bool Success { get; }
    public string? Message { get; }

    // Constructor, sadece başarı durumunu alır. Mesaj varsayılan olarak boş kalır.
    public Result(bool success) => Success = success;
    // Constructor, hem başarı durumunu hem de mesajı alır.
    public Result(bool success, string message) : this(success) => Message = message;
}

/*
İki adet constructor (yapıcı metot) tanımladık. Biri sadece success durumunu alırken, 
diğeri hem success hem de message alır. : this(success) kullanımı, bir constructor'dan başka bir 
constructor'ı çağırmamızı sağlar, bu da kod tekrarını önler.
*/

