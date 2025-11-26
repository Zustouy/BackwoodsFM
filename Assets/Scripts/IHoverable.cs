public interface IHoverable
{
    void OnHoverEnter();
    void OnHoverExit();
    bool LockHover { get; }
}
