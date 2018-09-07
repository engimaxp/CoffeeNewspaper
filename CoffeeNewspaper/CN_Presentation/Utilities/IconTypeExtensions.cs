namespace CN_Presentation.Utilities
{
    /// <summary>
    ///     Helper functions for <see cref="IconType" />
    /// </summary>
    public static class IconTypeExtensions
    {
        /// <summary>
        ///     Converts <see cref="IconType" /> to a FontAwesome string
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns></returns>
        public static string ToFontAwesomeText(this IconType type)
        {
            // Return a FontAwesome string based on the icon type
            switch (type)
            {
                case IconType.File:
                    return "\uf0f6";

                case IconType.Picture:
                    return "\uf1c5";

                case IconType.BreifCase:
                    return "\uf0b1";
                case IconType.Tasks:
                    return "\uf0ae";
                case IconType.Notes:
                    return "\uf249";
                case IconType.ChartArea:
                    return "\uf1fe";
                case IconType.GraduationCap:
                    return "\uf19d";
                case IconType.Cog:
                    return "\uf013";
                case IconType.Play:
                    return "\uf04b";
                case IconType.Check:
                    return "\uf00c";
                case IconType.Times:
                    return "\uf00d";
                case IconType.Pause:
                    return "\uf04c";
                case IconType.Star:
                    return "\uf005";
                case IconType.RegularFire:
                    return "\ue616";
                case IconType.SolidFire:
                    return "\ue617";
                case IconType.Add:
                    return "\uf067";
                case IconType.HourGlass:
                    return "\uf252";
                case IconType.Filter:
                    return "\uf0b0";
                case IconType.SortAmount:
                    return "\uf160";
                case IconType.ThLarge:
                    return "\uf009";
                case IconType.SortCreateTime:
                    return "\ue618";
                case IconType.SortCreateTimeReverse:
                    return "\ue619";
                case IconType.Trophy:
                    return "\uf091";
                case IconType.Clock:
                    return "\uf017";
                // If none found, return null
                default:
                    return null;
            }
        }
    }
}