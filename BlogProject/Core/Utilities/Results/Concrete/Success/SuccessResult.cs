namespace Core.Utilities.Results.Concrete.Success;

    // Başarılı bir işlemi temsil eden Result türevi.
    // Temel Result sınıfının success durumunu true olarak ayarlar.
    public class SuccessResult : Result
    {
        // 1. Sadece mesajı alan constructor: Success durumunu true, mesajı aldığı değer olarak ayarlar.
        public SuccessResult(string? message) : base(true, message){}

        // 2. Mesaj almayan constructor: Success durumunu true, mesajı null olarak ayarlar.
        public SuccessResult() : base(true){}
    }