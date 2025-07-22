using Core.Utilities.Results.Abstract;

namespace Core.Utilities.Results.Concrete;

    // Veri taşıyan IDataResult<T> arayüzünün temel somut uygulaması.
    public class DataResult<T> : Result, IDataResult<T>
    {
        // İşlem sonucunda dönen veri. Nullable olarak işaretliyoruz.
        public T? Data { get; protected set; } // protected set: Constructor'da ayarlandıktan sonra değişmez.

        // Üç constructor tanımlıyoruz:

        // 1. Veri, başarı durumu ve mesajı alan constructor:
        public DataResult(T? data, bool success, string? message) : base(success, message)
        {
            Data = data;
        }

        // 2. Veri ve başarı durumu alan constructor (mesajsız):
        public DataResult(T? data, bool success) : base(success)
        {
            Data = data;
        }

        // 3. Sadece başarı durumu ve mesajı alan constructor (veri olmadan, veri null olacak):
        // Bazı durumlarda işlem başarılı olur ama dönecek bir veri olmayabilir (örneğin sadece mesaj yeterlidir).
        public DataResult(bool success, string? message) : base(success, message)
        {
            Data = default; // T'nin varsayılan değeri (referans tipler için null, değer tipler için 0/false)
        }
    }