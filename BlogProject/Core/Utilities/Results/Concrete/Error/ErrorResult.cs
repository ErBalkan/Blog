namespace Core.Utilities.Results.Concrete.Error;
    // Başarısız bir işlemi temsil eden Result türevi.
    // Temel Result sınıfının success durumunu false olarak ayarlar.
    public class ErrorResult : Result
    {
        // 1. Sadece mesajı alan constructor: Success durumunu false, mesajı aldığı değer olarak ayarlar.
        public ErrorResult(string? message) : base(false, message){}

        // 2. Mesaj almayan constructor: Success durumunu false, mesajı null olarak ayarlar.
        public ErrorResult() : base(false){}
    }