using BlogProject.Business.Abstract; // ICategoryService için
using Core.Utilities.Results.Abstract; // IResult ve IDataResult için
using BlogProject.Entities; // Category entity'si için
using Microsoft.AspNetCore.Http; // IActionResult, StatusCode gibi tipler için
using Microsoft.AspNetCore.Mvc; // ApiController, Route, HttpGet vb. nitelikler için
using System.Collections.Generic; // List için
using System.Threading.Tasks;


namespace BlogProject.API.Controllers;
    // [Route("api/[controller]")] niteliği, bu controller'ın erişileceği URL yolunu belirler.
    // Örneğin, bu controller'a "/api/categories" yoluyla erişilebilir.
    [Route("api/[controller]")]
    // [ApiController] niteliği, API davranışlarına özgü varsayılan ayarları (model doğrulama, HTTP 400 yanıtları vb.) etkinleştirir.
    [ApiController]
    public class CategoriesController : ControllerBase // ControllerBase, API controller'ları için temel sınıfımızdır.
    {
        private readonly ICategoryService _categoryService; // Kategori iş mantığı servisimiz

        // Constructor (Yapıcı Metot): ICategoryService bağımlılığı enjekte edilir.
        // Bağımlılık enjeksiyonu sayesinde, controller bu servisin somut implementasyonunu bilmeden işini yapabilir.
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET api/categories
        // Tüm kategorileri getiren endpoint.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Business katmanındaki GetAllAsync metodunu çağırarak tüm kategorileri alırız.
            IDataResult<List<Category>> result = await _categoryService.GetAllAsync();

            // İşlem başarılıysa HTTP 200 OK ve veriyi döndür.
            if (result.Success)
            {
                return Ok(result.Data); // result.Data, List<Category> tipindedir.
            }
            // İşlem başarısızsa HTTP 400 Bad Request ve hata mesajını döndür.
            return BadRequest(result.Message); // result.Message, hata mesajını içerir.
        }

        // GET api/categories/{id}
        // Belirli bir ID'ye sahip kategoriyi getiren endpoint.
        [HttpGet("{id}")] // {id} parametresi URL'den alınır.
        public async Task<IActionResult> GetById(int id)
        {
            IDataResult<Category> result = await _categoryService.GetByIdAsync(id);

            if (result.Success)
            {
                // Eğer kategori bulunamazsa (result.Data null ise ama Success true ise, bu senaryo olasıdır), NotFound döndürebiliriz.
                // Ancak CategoryManager'ımız zaten bu durumu ErrorDataResult olarak işliyor.
                if (result.Data == null)
                {
                    return NotFound(result.Message); // HTTP 404 Not Found
                }
                return Ok(result.Data); // HTTP 200 OK
            }
            return BadRequest(result.Message); // HTTP 400 Bad Request
        }

        // POST api/categories
        // Yeni bir kategori ekleyen endpoint.
        [HttpPost]
        public async Task<IActionResult> Add(Category category) // İstek gövdesinden Category nesnesi alınır.
        {
            IMyResult result = await _categoryService.AddAsync(category);

            if (result.Success)
            {
                // İşlem başarılıysa HTTP 201 Created (başarıyla oluşturuldu) döndürür.
                // Genellikle Created, oluşturulan kaynağın URL'sini ve kendisini döndürmelidir.
                // Burada sadece OK döndürüyoruz, daha karmaşık API'ler CreatedAtAction kullanabilir.
                return StatusCode(201, result.Message); // HTTP 201 Created
            }
            return BadRequest(result.Message); // HTTP 400 Bad Request (doğrulama veya iş kuralı hatası)
        }

        // PUT api/categories
        // Mevcut bir kategoriyi güncelleyen endpoint.
        // Genellikle PUT isteklerinde güncellenecek kaynağın ID'si de URL'de belirtilir: [HttpPut("{id}")]
        // Ancak biz Category nesnesinin içindeki ID'yi kullanacağımız için bu şekilde bırakabiliriz.
        [HttpPut]
        public async Task<IActionResult> Update(Category category) // İstek gövdesinden Category nesnesi alınır.
        {
            IMyResult result = await _categoryService.UpdateAsync(category);

            if (result.Success)
        {
            return Ok(result.Message); // HTTP 200 OK
        }
            return BadRequest(result.Message); // HTTP 400 Bad Request
        }

        // DELETE api/categories/{id}
        // Belirli bir ID'ye sahip kategoriyi silen endpoint.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Silmeden önce kategorinin varlığını kontrol etmek iyi bir pratiktir,
            // ancak Business katmanındaki DeleteAsync metodu zaten bunu yapıyor.
            IMyResult result = await _categoryService.DeleteAsync(id);

            if (result.Success)
            {
                return Ok(result.Message); // HTTP 200 OK
            }
            return BadRequest(result.Message); // HTTP 400 Bad Request (kategori bulunamadı gibi)
        }
    }