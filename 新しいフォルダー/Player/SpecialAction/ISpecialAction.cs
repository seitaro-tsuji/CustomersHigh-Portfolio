public interface ISpecialAction
{
    void OnPressed();
    void OnHeld();
    void OnReleased();
    void Tick(float deltaTime);
}
