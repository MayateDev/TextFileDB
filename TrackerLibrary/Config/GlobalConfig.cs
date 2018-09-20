using DataLibrary;

namespace TrackerLibrary.Config
{
    public static class GlobalConfig
    {
        public static void InitializeConnection()
        {
            var db = new DbContext();
            db.InitializeTextDb();
        }
    }
}
