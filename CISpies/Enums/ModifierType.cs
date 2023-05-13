namespace SCPPlugins.CISpies.Enums
{
    /// <summary>
    ///     Specifies the spy spawn chance modifier types
    /// </summary>
    public enum ModifierType
    {
        /// <summary>
        ///     Adds modifier to spy respawn chance per player on the server
        /// </summary>
        PerPlayerOnline,

        /// <summary>
        ///     Adds modifier to spy respawn chance per player respawning in the wave
        /// </summary>
        PerPlayerRespawning,

        /// <summary>
        ///     Disables the modifier
        /// </summary>
        Disabled
    }
}