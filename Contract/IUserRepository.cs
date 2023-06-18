using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface IUserRepository
    {
        IEnumerable<Users> GetAll(string keyword = "");
        Users GetById(int id);
        Users GetByName(string name);
        void Update(Users user);
        void Delete(int id);
        void Add(Users user);
    }
}
