namespace Core.Utilities.Results.Concrete.Success;

    // Başarılı bir işlemi ve veri taşıyan DataResult<T> türevi.
    // Temel DataResult sınıfının success durumunu true olarak ayarlar.
    public class SuccessDataResult<T> : DataResult<T>
    {
        // 1. Veri ve mesajı alan constructor: Success true olarak ayarlanır.
        public SuccessDataResult(T? data, string? message) : base(data, true, message){}

        // 2. Sadece veriyi alan constructor (mesajsız): Success true olarak ayarlanır.
        public SuccessDataResult(T? data) : base(data, true){}

        // 3. Sadece mesajı alan constructor (veri null): Success true olarak ayarlanır.
        public SuccessDataResult(string? message) : base(default, true, message){}

        // 4. Hiçbir şey almayan constructor (veri null, mesaj null): Success true olarak ayarlanır.
        public SuccessDataResult() : base(default, true){}
    }