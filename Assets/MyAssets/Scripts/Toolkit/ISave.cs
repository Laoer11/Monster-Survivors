namespace MonsterSurvivors
{
    /// <summary>
    /// 存档数据接口 —— 所有需要存档的类都要实现它
    /// </summary>
    public interface ISave
    {
        /// <summary>
        /// 存档首次创建时调用（设置默认值）
        /// </summary>
        void Init();

        /// <summary>
        /// 保存前调用（把运行时数据写回存档对象）
        /// </summary>
        void Flush();
    }
}