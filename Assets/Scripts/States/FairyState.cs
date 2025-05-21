public abstract class FairyState
{
    protected Fairy fairy;
    public FairyState(Fairy fairy) { this.fairy = fairy; }

    public virtual bool IsTerminal => false; 
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}