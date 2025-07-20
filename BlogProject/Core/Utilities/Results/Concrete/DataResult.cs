using Core.Utilities.Results.Abstract;

namespace Core.Utilities.Results.Concrete;

public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; }

    // Constructor, sadece başarı durumunu ve veriyi alır. Mesaj varsayılan olarak boş kalır.
    public DataResult(T data, bool success) : base(success) => Data = data;
    // success parametresini base (Result) sınıfına gönderir.

    // Constructor, başarı durumunu, mesajı ve veriyi alır.
    public DataResult(T data, bool success, string message) : base(success, message) => Data = data;
    // success ve message'ı base sınıfına gönderir.
}

/*
DataResult<T>, Result sınıfından türediği için Success ve Message özelliklerine zaten sahiptir.

Burada da iki adet constructor tanımladık, biri sadece veri ve başarı durumunu alırken, 
diğeri veri, başarı durumu ve mesajı alır. : base(...) kullanımı, 
türediği Result sınıfının constructor'larını çağırmamızı sağlar.
*/