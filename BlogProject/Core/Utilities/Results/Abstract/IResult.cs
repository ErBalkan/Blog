namespace Core.Utilities.Results.Abstract;

public interface IResult
{
    bool Success { get; } // İşlemin başarılı olup olmadığını belirtir.
    string? Message { get; } // İşlemle ilgili mesajı içerir (başarı veya hata mesajı). 
}

/*
Success (bool): İşlemin başarı durumunu gösteren bir boolean değer. 
true ise başarılı, false ise başarısız.

Message (string): İşlemle ilgili bir mesaj 
(örneğin "Kullanıcı başarıyla eklendi" veya "Geçersiz parola").
*/

