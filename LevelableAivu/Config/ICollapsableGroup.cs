namespace LevelableAivu.Config
{
    public interface ICollapseableGroup
    {
        ref bool IsExpanded();
        void SetExpanded(bool value);
    }
}
