using Portfolio.Core.Types.Models.MetaData;

namespace Portfolio.Core.Interfaces.Context.MetaData
{
    public interface IMetaDataRepository: IRepository<MetaDataModel>
    {
        Task<IEnumerable<MetaDataModel>> GetAllMetaDataAsync();
        Task<MetaDataModel> GetMetaDataAsync(string email);
        Task<MetaDataModel> GetMetaDataAsyncById(Guid id);
        Task<bool> AddMetaDataAsync(MetaDataModel MetaData);
        Task<bool> UpdateMetaDataAsync(MetaDataModel MetaData);
        Task<bool> DeleteMetaDataAsync(string email);
    }
}