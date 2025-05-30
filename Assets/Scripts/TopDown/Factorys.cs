namespace TopDown
{
    public static class GameModeFactory
    {
        public static IGameMode CreateMode(MapType type)
        {
            return type switch
            {
                MapType.Lobby => new LobbyMode(),
                _ => null
            };
        }
    }
    public static class UIManagerFactory
    {
        private static MyLoader<ILoadManager> resources;

        public static ILoadManager CreateUIManager(MapType type)
        {
            resources = new();
            var name = type.ToString() + "Loader";
            return type switch
            {
                _ => resources.Load(name, name)
            };
        }
    }
    public static class ControllerManagerFactory
    {
        public static IControllerManager CreateControllerManager(MapType type)
        {
            return type switch
            {
                _ => null
            };
        }
    }
}