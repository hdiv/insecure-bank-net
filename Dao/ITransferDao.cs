using System.Threading.Tasks;
using insecure_bank_net.Bean;

namespace insecure_bank_net.Dao
{
    public interface ITransferDao
    {
        int InsertTransfer(Transfer transfer);

    }
}