using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll(string keyword = "");
    }
}
