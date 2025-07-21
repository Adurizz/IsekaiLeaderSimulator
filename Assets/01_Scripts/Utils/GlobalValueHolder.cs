public static class GlobalValueHolder
{
    #region SceneIndexes
    public readonly static int introSceneIndex = 0;
    public readonly static int lobbySceneIndex = 1;
    public readonly static int ingameSceneIndex = 2;
    #endregion
    public readonly static int maxStageNum = 5;
    public readonly static int maxCompanionLevel = 5;
    public readonly static string objectPoolName = "ObjectPool";
    #region MapMakingValues
    /// <summary>
    /// -245
    /// </summary>
    public readonly static int minMapIdx = -245;
    /// <summary>
    /// 245
    /// </summary>
    public readonly static int maxMapIdx = 245;
    /// <summary>
    /// 491
    /// </summary>
    public readonly static int mapSize = maxMapIdx * 2 + 1;
    #endregion
    public readonly static int companionHireCost = 500;
}
