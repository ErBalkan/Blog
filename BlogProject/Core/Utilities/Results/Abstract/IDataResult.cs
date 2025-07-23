namespace Core.Utilities.Results.Abstract;

    // Veri taşıyan bir işlemin sonucunu temsil eden arayüz.
    // T: Taşınacak verinin tipi (Category, List<Post> vb.).
    public interface IDataResult<T> : IMyResult
    {
        // İşlem sonucunda dönen veri.
        // Başarısız durumlarda bu veri null olabilir, bu yüzden '?' ile nullable olarak işaretliyoruz.
        T? Data { get; } 
    }