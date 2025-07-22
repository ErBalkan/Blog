namespace Core.Utilities.Results.Concrete.Success;

    // Başarısız bir işlemi ve veri taşıyan DataResult<T> türevi.
    // Temel DataResult sınıfının success durumunu false olarak ayarlar.
    public class ErrorDataResult<T> : DataResult<T>
    {
        // 1. Veri ve mesajı alan constructor: Success false olarak ayarlanır.
        public ErrorDataResult(T? data, string? message) : base(data, false, message){}

        // 2. Sadece veriyi alan constructor (mesajsız): Success false olarak ayarlanır.
        public ErrorDataResult(T? data) : base(data, false){}

        // 3. Sadece mesajı alan constructor (veri null): Success false olarak ayarlanır.
        public ErrorDataResult(string? message) : base(default, false, message){}

        // 4. Hiçbir şey almayan constructor (veri null, mesaj null): Success false olarak ayarlanır.
        public ErrorDataResult() : base(default, false){}
    }