namespace GClaims.Core.Helpers
{
     public class ProcessEventArgs : EventArgs
    {
        public bool IsSuccessful { get; set; }
        public DateTime CompletionTime { get; set; } 
        public object UserWizardCreate { get; set; }
    }

    public interface IUpdateDataServices
    {
        event Action UpdateData;
        event EventHandler<ProcessEventArgs> ProcessCompleted;
        void CallUpdateData();
        void OnProcessCompleted(ProcessEventArgs e);
    }

    public class UpdateDataServices : IUpdateDataServices
    {
        public event Action UpdateData;
        public event EventHandler<ProcessEventArgs> ProcessCompleted;

        public void CallUpdateData()
        {
            UpdateData?.Invoke();
        }

        public void OnProcessCompleted(ProcessEventArgs e)
        {
            ProcessCompleted?.Invoke(this, e);
        }
    }
    // public interface IUpdateDataServices
    // {
    //     event Action AtualizarGridAppUser;
    //     void CallAtualizarGridAppUser();
    // }
    // public class UpdateDataServices : IUpdateDataServices
    // {
    //     public event Action AtualizarGridAppUser;

    //     public void CallAtualizarGridAppUser()
    //     {
    //         AtualizarGridAppUser?.Invoke();
    //     }
      
    // }
    //pode ser chamado de qualquer parte do sistema atraves da chamada:UpdateDataServices.CallAtualizarGridAppUser()
    // UpdateDataServices.AtualizarGridAppUser += PreencherTabela;
}
