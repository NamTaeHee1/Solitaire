public class MoveCardCommand : ICommand
{
    private Card moveCard;
    private Point from, to;

    public MoveCardCommand(Card _moveCard, Point _from, Point _to)
    {
        moveCard = _moveCard;
        from = _from;
        to = _to;
    }

    public void Excute()
    {
        moveCard.Move(to);

        Recorder.Push(this);
    }

    public void Undo()
    {
        moveCard.Move(from);
    }
}
