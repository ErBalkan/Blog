namespace Core.Utilities.Results.Abstract;

// T, döndürülecek verinin tipini temsil eder (örneğin, bir kullanıcı nesnesi, bir liste vb.).
public interface IDataResult<T> : IResult
{
    T Data { get; } // İşlem sonucunda döndürülecek veriyi içerir.
}

/*
IDataResult<T> generic bir arayüzdür. <T> ifadesi, bu arayüzün herhangi bir veri tipiyle 
(T) çalışabileceği anlamına gelir. Örneğin, IDataResult<List<Product>> veya IDataResult<User>.

Data özelliği, işlem sonucunda döndürülmesi beklenen veriyi tutar.
*/