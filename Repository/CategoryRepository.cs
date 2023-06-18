using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MusicDbContext _dbContext;
        public CategoryRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Category> GetAll(string keyword = "")
        {
            return _dbContext.Category;
        }
    }
}
