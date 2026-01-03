namespace ReunionMovementDLL
{
    /// <summary>
    /// 版本号类。
    /// </summary>
    public static partial class Version
    {
        private const string ReunionMovementVersionString = "2026.01.01";

        private static IVersionHelper versionHelper = null;

        /// <summary>
        /// 获取游戏框架版本号。
        /// </summary>
        public static string ReunionMovementVersion
        {
            get
            {
                return ReunionMovementVersionString;
            }
        }

        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public static string GameVersion
        {
            get
            {
                if (versionHelper == null)
                {
                    return string.Empty;
                }

                return versionHelper.GameVersion;
            }
        }

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public static int InternalGameVersion
        {
            get
            {
                if (versionHelper == null)
                {
                    return 0;
                }

                return versionHelper.InternalGameVersion;
            }
        }

        /// <summary>
        /// 设置版本号辅助器。
        /// </summary>
        /// <param name="versionHelper">要设置的版本号辅助器。</param>
        public static void SetVersionHelper(IVersionHelper versionHelper)
        {
            Version.versionHelper = versionHelper;
        }
    }
}
