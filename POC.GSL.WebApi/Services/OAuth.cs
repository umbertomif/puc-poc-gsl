using POC.GSL.Data;
using POC.GSL.Domain;

namespace POC.GSL.WebApi.Services
{
    public class OAuth
    {
        private UnitOfWork _unitOfWork;

        public OAuth(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool ValidatAccess(string id, List<string> role)
        {
            var user = _unitOfWork.GetRepository<User>().Find(id);

            return user != null? role.Where(x => x.Contains(user.Profile)).FirstOrDefault() != null? true: false:false;
        }
    }
}
