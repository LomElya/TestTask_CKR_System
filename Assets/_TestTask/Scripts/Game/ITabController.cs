namespace TestTask_CKR_System.Game
{
    public interface ITabController
    {
        AppTab Tab { get; }
        void Activate();
        void Deactivate();
    }
}