using Core.Utilities.Results.Abstract;

namespace Core.Utilities.Results.Concrete;

    // IResult arayüzünün temel somut uygulaması.
    public class Result : IResult
    {
        // protected set: Sadece bu sınıf ve ondan türeyen sınıflar tarafından set edilebilir.
        // Constructor'da belirlendikten sonra dışarıdan değiştirilemez.
        public bool Success { get; protected set; }
        public string? Message { get; protected set; } // Nullable string

        // İki constructor (yapıcı metot) tanımlıyoruz:

        // 1. Sadece başarı durumunu ve mesajı alan constructor:
        // Varsayılan olarak mesajı null kabul edebiliriz.
        public Result(bool success, string? message)
        {
            Success = success;
            Message = message;
        }

        // 2. Sadece başarı durumunu alan constructor (mesaj olmadan):
        // Bu constructor, içinde yukarıdaki constructor'ı çağırır ve mesajı null olarak geçer.
        public Result(bool success) : this(success, null){}
    }

