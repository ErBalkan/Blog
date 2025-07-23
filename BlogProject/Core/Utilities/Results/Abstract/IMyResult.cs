namespace Core.Utilities.Results.Abstract;

    // Bir işlemin sonucunu temsil eden temel arayüz.
    public interface IMyResult
    {
        // İşlemin başarılı olup olmadığını gösterir. (true: başarılı, false: başarısız)
        bool Success { get; }

        // İşlemle ilgili bir mesaj (örneğin "Kategori başarıyla eklendi." veya "Kategori bulunamadı.").
        string? Message { get; } // Nullable string olarak işaretledik. Mesajın boş olması da bir senaryo olabilir.
    }

