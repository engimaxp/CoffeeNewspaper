namespace CN_Presentation.Utilities
{
    public static class ColorTypeExtensions
    {
        public static string ToColorText(this UserColorType type)
        {
            switch (type)
            {
                case UserColorType.WordGreen: return "00c541";
                case UserColorType.WordRed:
                    return "ff4747";
                case UserColorType.WordLightBlue:
                    return "45b6e5";
                case UserColorType.WordOrange:
                    return "ffa800";
                case UserColorType.ForegroundDark:
                    return "6a6a6a";
                case UserColorType.ForegroundSuperDark:
                    return "000000";
                default: return "ffffff";
            }
        }
    }
}