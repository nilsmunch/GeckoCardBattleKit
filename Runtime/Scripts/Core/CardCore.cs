using LizardCards.Network;

namespace LizardCards
{
    public static class CardCore
    {
        public static bool Initialized( )
        {
            return (Network == null);
        }

        public static void Register(INetworkController controller)
        {
            Network = controller;
        }

        public static INetworkController Network { get; private set; }

        public static void Initialize()
        {
            Network.Initialize();
        }
    }
}