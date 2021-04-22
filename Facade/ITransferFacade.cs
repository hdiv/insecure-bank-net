using insecure_bank_net.Bean;

namespace insecure_bank_net.Facade
{
    public interface ITransferFacade
    {
        void CreateNewTransfer(Transfer transfer);
    }
}