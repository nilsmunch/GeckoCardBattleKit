using LizardCards.Network;

namespace LizardCards
{
    public static class CardCore
    {
        private static INetworkController _networkController;

        public static void Register(INetworkController controller)
        {
            _networkController = controller;
        }

        public static INetworkController Network()
        {
            return _networkController;
        }
    }
}