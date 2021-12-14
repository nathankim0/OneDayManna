using System.Threading.Tasks;

namespace OneDayManna.Interfaces
{
    public interface IPhotoLibrary
    {
        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}
